using AutoHotkey.Interop;
using Microsoft.Win32;
using Newtonsoft.Json;
using SpotifyAPI.Local;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Services;

namespace Toastify.Core
{
    internal class Spotify : IDisposable
    {
        #region Singleton

        private static Spotify _instance;

        public static Spotify Instance
        {
            get { return _instance ?? (_instance = new Spotify()); }
        }

        #endregion Singleton

        #region Private fields

        private AutoHotkeyEngine ahk;

        private SpotifyLocalAPI localAPI;

        private readonly string spotifyPath;

        private Process spotifyProcess;

        private Song _currentSong;

        #endregion Private fields

        #region Public properties

        public bool IsRunning { get { return this.GetMainWindowHandle() != IntPtr.Zero; } }

        public bool IsMinimized
        {
            get
            {
                if (!this.IsRunning)
                    return false;

                var hWnd = this.GetMainWindowHandle();

                // check Spotify's current window state
                var placement = new Win32API.WindowPlacement();
                Win32API.GetWindowPlacement(hWnd, ref placement);

                return placement.showCmd == Win32API.Constants.SW_SHOWMINIMIZED;
            }
        }

        public Song CurrentSong
        {
            get { return this._currentSong ?? (this._currentSong = this.localAPI?.GetStatus()?.Track); }
            private set { this._currentSong = value; }
        }

        #endregion Public properties

        #region Events

        public event EventHandler<SpotifyStateEventArgs> Connected;

        public event EventHandler<SpotifyTrackChangedEventArgs> SongChanged;

        public event EventHandler<SpotifyPlayStateChangedEventArgs> PlayStateChanged;

        public event EventHandler<SpotifyTrackTimeChangedEventArgs> TrackTimeChanged;

        public event EventHandler<SpotifyVolumeChangedEventArgs> VolumeChanged;

        #endregion Events

        protected Spotify()
        {
            this.spotifyPath = this.GetSpotifyPath();

            // Connect with Spotify to use the local API.
            this.localAPI = new SpotifyLocalAPI();

            // Subscribe to SpotifyLocalAPI's events.
            this.localAPI.OnTrackChange += this.SpotifyLocalAPI_OnTrackChange;
            this.localAPI.OnPlayStateChange += this.SpotifyLocalAPI_OnPlayStateChange;
            this.localAPI.OnTrackTimeChange += this.SpotifyLocalAPI_OnTrackTimeChange;
            this.localAPI.OnVolumeChange += this.SpotifyLocalAPI_OnVolumeChange;
            this.localAPI.ListenForEvents = true;
        }

        public void StartSpotify()
        {
            if (this.IsRunning)
            {
                this.localAPI.Connect();
                this.Connected?.Invoke(this, new SpotifyStateEventArgs(this.localAPI.GetStatus()));
                return;
            }

            // Launch Spotify.
            var spotifyFilePath = Path.Combine(this.spotifyPath, "spotify.exe");
            this.spotifyProcess = Process.Start(spotifyFilePath);

            if (SettingsXml.Instance.MinimizeSpotifyOnStartup)
                this.Minimize();
            else
            {
                // We need to let Spotify start up before interacting with it fully.
                Thread.Sleep(3000);
            }

            this.localAPI.Connect();
            this.Connected?.Invoke(this, new SpotifyStateEventArgs(this.localAPI.GetStatus()));
        }

        private void Minimize()
        {
            int remainingSleep = 2000;

            IntPtr hWnd;

            // Since Minimize is often called during startup, the hWnd is often not created yet
            // wait a maximum of remainingSleep for it to appear and then minimize it if it did.
            while ((hWnd = this.GetMainWindowHandle()) == IntPtr.Zero && remainingSleep > 0)
            {
                Thread.Sleep(100);
                remainingSleep -= 100;
            }

            if (hWnd != IntPtr.Zero)
            {
                // disgusting but sadly neccessary. Let Spotify initialize a bit before minimizing it
                // otherwise the window hides itself and doesn't respond to taskbar clicks.
                // I tried to work around this by waiting for the window size to initialize (via GetWindowRect)
                // but that didn't work, there is some internal initialization that needs to occur.
                Thread.Sleep(500);
                Win32API.ShowWindow(hWnd, Win32API.Constants.SW_SHOWMINIMIZED);
            }
        }

        public void Kill()
        {
            if (this.spotifyProcess != null)
            {
                this.spotifyProcess.Close();
                Thread.Sleep(1000);
            }
            Win32API.KillProc("spotify");

            this.localAPI.Dispose();
        }

        private void ShowSpotify()
        {
            if (this.IsRunning)
            {
                var hWnd = this.GetMainWindowHandle();

                // check Spotify's current window state
                var placement = new Win32API.WindowPlacement();
                Win32API.GetWindowPlacement(hWnd, ref placement);

                int showCommand = Win32API.Constants.SW_SHOW;

                // if Spotify is minimzed we need to send a restore so that the window
                // will come back exactly like it was before being minimized (i.e. maximized
                // or otherwise) otherwise if we call SW_RESTORE on a currently maximized window
                // then instead of staying maximized it will return to normal size.
                if (placement.showCmd == Win32API.Constants.SW_SHOWMINIMIZED)
                    showCommand = Win32API.Constants.SW_RESTORE;

                Win32API.ShowWindow(hWnd, showCommand);

                Win32API.SetForegroundWindow(hWnd);
                Win32API.SetFocus(hWnd);
            }
        }

        private IntPtr GetMainWindowHandle()
        {
            return this.spotifyProcess?.MainWindowHandle ?? Win32API.FindWindow("SpotifyMainWindow", null);
        }

        public void SendAction(SpotifyAction action)
        {
            if (!this.IsRunning)
                return;

            // bah. Because control cannot fall through cases we need to special case volume
            if (SettingsXml.Instance.ChangeSpotifyVolumeOnly)
            {
                switch (action)
                {
                    case SpotifyAction.VolumeUp:
                        Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeUp);
                        this.localAPI.IncrementVolume();
                        return;

                    case SpotifyAction.VolumeDown:
                        Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeDown);
                        this.localAPI.DecrementVolume();
                        return;

                    case SpotifyAction.Mute:
                        Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Mute);
                        this.localAPI.ToggleMute();
                        return;
                }
            }

            switch (action)
            {
                case SpotifyAction.CopyTrackInfo:
                case SpotifyAction.ShowToast:
                    break;

                case SpotifyAction.ShowSpotify:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.ShowSpotify);
                    if (this.IsMinimized)
                        this.ShowSpotify();
                    else
                        this.Minimize();
                    break;

                case SpotifyAction.FastForward:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.FastForward);
                    this.SendComplexKeys("+{Right}");
                    break;

                case SpotifyAction.Rewind:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Rewind);
                    this.SendComplexKeys("+{Left}");
                    break;

                default:
                    Telemetry.TrackEvent(TelemetryCategory.Action, $"{Telemetry.TelemetryEvent.Action.Default}{action}");
                    Win32API.SendMessage(this.GetMainWindowHandle(), Win32API.Constants.WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)action));
                    break;
            }
        }

        /// <summary>
        /// Some commands require sending keys directly to Spotify (for example, Fast Forward and Rewind which
        /// are not handled by Spotify). We can't inject keys directly with WM_KEYDOWN/UP since we need a keyboard
        /// hook to actually change the state of various modifier keys (for example, Shift + Right for Fast Forward).
        ///
        /// AutoHotKey has that hook and can modify the state for us, so let's take advantge of it.
        /// </summary>
        /// <param name="keys"></param>
        private void SendComplexKeys(string keys)
        {
            // only initialize AHK when needed as it can be expensive (dll copy etc) if not actually needed
            if (this.ahk == null)
                this.ahk = new AutoHotkeyEngine();

            this.ahk.ExecRaw("SetTitleMatchMode 2");

            this.ahk.ExecRaw("DetectHiddenWindows, On");
            this.ahk.ExecRaw($"ControlSend, ahk_parent, {keys}, ahk_class SpotifyMainWindow");

            this.ahk.ExecRaw("DetectHiddenWindows, Off");
        }

        private string GetSpotifyPath()
        {
            string spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;

            // Try in the secondary location.
            if (string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;

            if (string.IsNullOrEmpty(spotifyPath))
                throw new ArgumentException("Could not find spotify path in registry");

            return spotifyPath;
        }

        public void Dispose()
        {
            this.localAPI?.Dispose();
        }

        #region Event handlers

        private void SpotifyLocalAPI_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            this.CurrentSong = e.NewTrack;
            this.SongChanged?.Invoke(this, new SpotifyTrackChangedEventArgs(e.OldTrack, this.CurrentSong, this.localAPI.GetStatus().Playing));
        }

        private void SpotifyLocalAPI_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            this.PlayStateChanged?.Invoke(this, new SpotifyPlayStateChangedEventArgs(e.Playing));
        }

        private void SpotifyLocalAPI_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            this.TrackTimeChanged?.Invoke(this, new SpotifyTrackTimeChangedEventArgs(e.TrackTime));
        }

        private void SpotifyLocalAPI_OnVolumeChange(object sender, VolumeChangeEventArgs e)
        {
            this.VolumeChanged?.Invoke(this, new SpotifyVolumeChangedEventArgs(e.OldVolume, e.NewVolume));
        }

        #endregion Event handlers
    }
}