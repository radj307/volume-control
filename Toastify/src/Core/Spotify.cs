using AutoHotkey.Interop;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using System;
using System.Diagnostics;
using System.Linq;
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

        private readonly SpotifyLocalAPI localAPI;

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

        public StatusResponse Status { get { return this.localAPI?.GetStatus(); } }

        #endregion Public properties

        #region Events

        public event EventHandler Exited;

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
            int timeout = SettingsXml.Instance.StartupWaitTimeout;
            this.spotifyProcess = !this.IsRunning ? this.LaunchSpotifyAndWaitForInputIdle(timeout) : this.FindSpotifyProcess();

            if (this.spotifyProcess == null)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_PROCESS);

            this.spotifyProcess.EnableRaisingEvents = true;
            this.spotifyProcess.Exited += this.Spotify_Exited;

            this.ConnectWithSpotify();
        }

        /// <summary>
        /// Starts the Spotify process and waits for it to enter an idle state.
        /// </summary>
        /// <param name="timeoutMilliseconds"> Specifies the maximum amount of time to wait for the process to enter an idle state. </param>
        /// <returns> The started process. </returns>
        private Process LaunchSpotifyAndWaitForInputIdle(int timeoutMilliseconds = 10000)
        {
            int maxWait = timeoutMilliseconds;

            // Launch Spotify.
            Process spotifyProcess = Process.Start(this.spotifyPath);

            // If it is an UWP app, then Process.Start should return null: we need to look for the process.
            while (spotifyProcess == null && timeoutMilliseconds > 0)
            {
                spotifyProcess = this.FindSpotifyProcess();
                timeoutMilliseconds -= 250;
                Thread.Sleep(250);
            }

            // We need to let Spotify start-up before interacting with it.
            spotifyProcess?.WaitForInputIdle(maxWait);

            if (SettingsXml.Instance.MinimizeSpotifyOnStartup)
                this.Minimize(1000);

            return spotifyProcess;
        }

        /// <summary>
        /// Connect with Spotify.
        /// </summary>
        /// <exception cref="ApplicationStartupException">
        ///   if Toastify was not able to connect with Spotify or
        ///   if Spotify returns a null status after connection.
        /// </exception>
        private void ConnectWithSpotify()
        {
            // Sometimes (specially with a lot of active processes), the WaitForInputIdle method (used in LaunchSpotifyAndWaitForInputIdle)
            // does not seem to wait long enough to let us connect to Spotify successfully on the first try; so we wait and re-try.

            // Pre-emptive wait, in case some fool set SpotifyConnectionAttempts to 1! ;)
            Thread.Sleep(500);

            int maxAttempts = SettingsXml.Instance.SpotifyConnectionAttempts;
            bool connected;
            int attempts = 1;
            while (!(connected = this.localAPI.Connect()) && attempts < maxAttempts)
            {
                attempts++;
                Thread.Sleep(1000);
            }

            if (!connected)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_CONNECT);
            Debug.WriteLine($"Connected with Spotify after {attempts} attempt(s).");

            var status = this.localAPI.GetStatus();
            if (status == null)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_STATUS_NULL);

            this.Connected?.Invoke(this, new SpotifyStateEventArgs(status));
        }

        private Process FindSpotifyProcess()
        {
            var processes = Process.GetProcessesByName("spotify");
            return processes.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero && p.MainWindowHandle == this.GetMainWindowHandle());
        }

        private void Minimize(int delay = 0)
        {
            int remainingSleep = 2000;

            IntPtr hWnd;

            // The window handle should have already been created, but just in case it has not, we wait for it to show up.
            do
            {
                remainingSleep -= 100;
                Thread.Sleep(100);
                hWnd = this.GetMainWindowHandle();
            } while (hWnd == IntPtr.Zero && remainingSleep > 0);

            if (hWnd != IntPtr.Zero)
            {
                // We also need to wait a little more before minimizing the window;
                // if we don't, the toast will not show the current track until 'something' happens (track change, play state change...).
                Thread.Sleep(delay);
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

            Debug.WriteLine($"SendAction: {action}");

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

                case SpotifyAction.VolumeUp:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeUp);
                    if (SettingsXml.Instance.UseSpotifyVolumeControl)
                        this.SendComplexKeys("^{Up}");
                    else
                        this.localAPI.IncrementVolume();
                    return;

                case SpotifyAction.VolumeDown:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeDown);
                    if (SettingsXml.Instance.UseSpotifyVolumeControl)
                        this.SendComplexKeys("^{Down}");
                    else
                        this.localAPI.DecrementVolume();
                    return;

                case SpotifyAction.Mute:
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Mute);
                    this.localAPI.ToggleMute();
                    return;

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
            Debug.WriteLine($"Complex keys: {keys}");

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
            return ToastifyAPI.Spotify.GetSpotifyPath();
        }

        public void Dispose()
        {
            this.localAPI?.Dispose();
        }

        #region Event handlers

        private void Spotify_Exited(object sender, EventArgs e)
        {
            this.Exited?.Invoke(sender, e);
        }

        private void SpotifyLocalAPI_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            this.CurrentSong = e.NewTrack;
            this.SongChanged?.Invoke(this, new SpotifyTrackChangedEventArgs(e.OldTrack, this.CurrentSong, this.localAPI?.GetStatus()?.Playing ?? false));
        }

        private void SpotifyLocalAPI_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            this.PlayStateChanged?.Invoke(this, new SpotifyPlayStateChangedEventArgs(e.Playing, this.localAPI?.GetStatus()?.Track));
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