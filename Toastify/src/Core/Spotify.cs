using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using log4net;
using log4net.Util;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Timer = System.Timers.Timer;

namespace Toastify.Core
{
    internal class Spotify : IDisposable
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Spotify));

        #region Singleton

        private static Spotify _instance;

        public static Spotify Instance
        {
            get { return _instance ?? (_instance = new Spotify()); }
        }

        #endregion Singleton

        #region Private fields

        private readonly BackgroundWorker spotifyLauncher = new BackgroundWorker { WorkerSupportsCancellation = true };

        private readonly EventWaitHandle spotifyLauncherWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Toastify_SpotifyLauncherWaitHandle");

        private readonly SpotifyLocalAPI localAPI;

        private readonly SpotifyLocalAPIConfig localAPIConfig;

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
                return Win32API.IsWindowMinimized(hWnd);
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

            // SpotifyLocalAPI
            this.localAPIConfig = new SpotifyLocalAPIConfig();
            this.localAPI = new SpotifyLocalAPI(this.localAPIConfig);

            // Subscribe to SpotifyLocalAPI's events.
            this.localAPI.OnTrackChange += this.SpotifyLocalAPI_OnTrackChange;
            this.localAPI.OnPlayStateChange += this.SpotifyLocalAPI_OnPlayStateChange;
            this.localAPI.OnTrackTimeChange += this.SpotifyLocalAPI_OnTrackTimeChange;
            this.localAPI.OnVolumeChange += this.SpotifyLocalAPI_OnVolumeChange;
            this.localAPI.ListenForEvents = true;
        }

        public void StartSpotify()
        {
            this.spotifyLauncher.DoWork += this.StartSpotify_WorkerTask;
            this.spotifyLauncher.RunWorkerCompleted += this.StartSpotify_WorkerTaskCompleted;

            if (Settings.Current.StartupWaitTimeout < 60000)
                Settings.Current.StartupWaitTimeout = 60000;
            Timer timeoutTimer = new Timer(Settings.Current.StartupWaitTimeout) { AutoReset = false };
            timeoutTimer.Elapsed += (sender, e) => this.spotifyLauncher.CancelAsync();

            this.spotifyLauncher.RunWorkerAsync();
            timeoutTimer.Start();
        }

        #region StartSpotify background worker

        private void StartSpotify_WorkerTask(object sender, DoWorkEventArgs e)
        {
            this.spotifyProcess = !this.IsRunning ? this.LaunchSpotifyAndWaitForInputIdle(e) : this.FindSpotifyProcess();
            if (e.Cancel)
                return;
            if (this.spotifyProcess == null)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_PROCESS);

            this.spotifyProcess.EnableRaisingEvents = true;
            this.spotifyProcess.Exited += this.Spotify_Exited;

            this.ConnectWithSpotify(e);
        }

        private void StartSpotify_WorkerTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                if (e.Error != null)
                {
                    if (e.Error is ApplicationStartupException applicationStartupException)
                    {
                        logger.ErrorInvariantCulture("Error while starting Spotify.", applicationStartupException);

                        string errorMsg = Properties.Resources.ERROR_STARTUP_SPOTIFY;
                        string techDetails = $"Technical details\n{applicationStartupException.Message}";
                        MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(applicationStartupException, true);
                    }
                    else if (e.Error is WebException webException)
                    {
                        logger.ErrorInvariantCulture("Web exception while starting Spotify.", webException);

                        string errorMsg = Properties.Resources.ERROR_STARTUP_RESTART;
                        string status = $"{webException.Status}";
                        if (webException.Status == WebExceptionStatus.ProtocolError)
                            status += $" ({(webException.Response as HttpWebResponse)?.StatusCode}, \"{(webException.Response as HttpWebResponse)?.StatusDescription}\")";
                        string techDetails = $"Technical details: {webException.Message}\n{webException.HResult}, {status}";
                        MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(webException, true);
                    }
                    else
                    {
                        logger.ErrorInvariantCulture("Unknown error while starting Spotify.", e.Error);

                        string errorMsg = Properties.Resources.ERROR_UNKNOWN;
                        string techDetails = $"Technical Details: {e.Error.Message}\n{e.Error.StackTrace}";
                        MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(e.Error, true);
                    }
                }
                else // e.Cancelled
                {
                    logger.ErrorExt("Toastify was not able to find Spotify within the timeout interval.");

                    string errorMsg = Properties.Resources.ERROR_STARTUP_SPOTIFY;
                    const string techDetails = "Technical Details: timeout";
                    MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Terminate Toastify
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() => Application.Current.Shutdown()));
            }
            else
                this.Spotify_Connected(this, new SpotifyStateEventArgs(this.localAPI.GetStatus()));

            this.spotifyLauncherWaitHandle.Close();
        }

        /// <summary>
        /// Starts the Spotify process and waits for it to enter an idle state.
        /// </summary>
        /// <returns> The started process. </returns>
        private Process LaunchSpotifyAndWaitForInputIdle(DoWorkEventArgs e)
        {
            logger.InfoExt("Launching Spotify...");

            // Launch Spotify.
            this.spotifyProcess = Process.Start(this.spotifyPath);

            // If it is an UWP app, then Process.Start should return null: we need to look for the process.
            bool signaled = false;
            while (this.spotifyProcess == null && !signaled)
            {
                this.spotifyProcess = this.FindSpotifyProcess();
                signaled = this.spotifyLauncherWaitHandle.WaitOne(1000);
                if (this.spotifyLauncher.CheckCancellation(e))
                    return this.spotifyProcess;
            }

            logger.InfoExt($"Spotify process started: {this.spotifyProcess}");

            // We need to let Spotify start-up before interacting with it.
            this.spotifyProcess?.WaitForInputIdle();

            if (Settings.Current.MinimizeSpotifyOnStartup)
                this.Minimize(1000);

            return this.spotifyProcess;
        }

        /// <summary>
        /// Connect with Spotify.
        /// </summary>
        /// <exception cref="ApplicationStartupException">
        ///   if Toastify was not able to connect with Spotify or
        ///   if Spotify returns a null status after connection.
        /// </exception>
        private void ConnectWithSpotify(DoWorkEventArgs e)
        {
            // Sometimes (specially with a lot of active processes), the WaitForInputIdle method (used in LaunchSpotifyAndWaitForInputIdle)
            // does not seem to wait long enough to let us connect to Spotify successfully on the first try; so we wait and re-try.

            logger.InfoExt("Connecting to Spotify's local APIs endpoint...");

            this.spotifyLauncherWaitHandle.Reset();
            bool signaled;

            bool connected = false;
            do
            {
                signaled = this.spotifyLauncherWaitHandle.WaitOne(500);
                if (this.spotifyLauncher.CheckCancellation(e))
                    return;

                try
                {
                    connected = this.localAPI.Connect();
                }
                catch (WebException ex)
                {
                    logger.WarnInvariantCulture("WebException while connecting to Spotify.", ex);
                }
            } while (!connected && !signaled);

            if (!connected)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_CONNECT);
            logger.DebugExt($"Connected with Spotify on \"{this.localAPIConfig.HostUrl}:{this.localAPIConfig.Port}/\".");

            // Try to get a status report from Spotify
            var status = this.localAPI.GetStatus();
            if (status == null)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_STATUS_NULL);
        }

        #endregion StartSpotify background worker

        private Process FindSpotifyProcess()
        {
            logger.DebugExt("Looking for Spotify process...");

            var processes = Process.GetProcessesByName("spotify");
            var process = processes.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero && p.MainWindowHandle == this.GetMainWindowHandle());

            // If none of the Spotify processes found has a valid MainWindowHandle,
            // then Spotify has probably been minimized to the tray: we need to check every thread's window.
            if (process == null && processes.Length > 0)
            {
                foreach (var p in processes)
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (ProcessThread t in p.Threads)
                    {
                        IntPtr hWnd = Win32API.FindThreadWindowByClassName((uint)t.Id, "SpotifyMainWindow");
                        if (hWnd != IntPtr.Zero)
                            return p;
                    }
                }
            }

            return process;
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
                Win32API.ShowWindow(hWnd, Win32API.ShowWindowCmd.SW_SHOWMINIMIZED);
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

                var showCommand = Win32API.ShowWindowCmd.SW_SHOW;

                // if Spotify is minimzed we need to send a restore so that the window
                // will come back exactly like it was before being minimized (i.e. maximized
                // or otherwise) otherwise if we call SW_RESTORE on a currently maximized window
                // then instead of staying maximized it will return to normal size.
                if (placement.showCmd == Win32API.ShowWindowCmd.SW_SHOWMINIMIZED)
                    showCommand = Win32API.ShowWindowCmd.SW_RESTORE;

                Win32API.ShowWindow(hWnd, showCommand);

                Win32API.SetForegroundWindow(hWnd);
                Win32API.SetFocus(hWnd);
            }
        }

        private IntPtr GetMainWindowHandle()
        {
            IntPtr spotifyWindow = this.spotifyProcess?.MainWindowHandle ?? IntPtr.Zero;
            return spotifyWindow != IntPtr.Zero ? spotifyWindow : Win32API.FindWindow("SpotifyMainWindow", null);
        }

        /// <summary>
        /// Returns the handle to the only visible windows whose class is "Chrome_WidgetWin_0".
        /// (This window is child of "CefBrowserWindow", who in turn is child of "SpotifyMainWindow")
        /// </summary>
        /// <returns></returns>
        private IntPtr GetCefWidgetWindowHandle()
        {
            IntPtr mainHWnd = this.GetMainWindowHandle();
            IntPtr cefHWnd = Win32API.FindWindowEx(mainHWnd, IntPtr.Zero, "CefBrowserWindow", null);
            IntPtr wdgtHWnd = Win32API.FindWindowEx(cefHWnd, IntPtr.Zero, "Chrome_WidgetWin_0", null);
            return wdgtHWnd;
        }

        public void SendAction(SpotifyAction action)
        {
            if (!this.IsRunning)
                return;

            bool sendAppCommandMessage = false;

            switch (action)
            {
                case SpotifyAction.CopyTrackInfo:
                case SpotifyAction.ShowToast:
                    break;

                case SpotifyAction.ShowSpotify:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.ShowSpotify);
                    if (this.IsMinimized)
                        this.ShowSpotify();
                    else
                        this.Minimize();
                    break;

                case SpotifyAction.FastForward:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.FastForward);
                    this.SendShortcut(action);
                    break;

                case SpotifyAction.Rewind:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.Rewind);
                    this.SendShortcut(action);
                    break;

                case SpotifyAction.VolumeUp:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeUp);
                    switch (Settings.Current.VolumeControlMode)
                    {
                        case ToastifyVolumeControlMode.Spotify:
                            this.SendShortcut(action);
                            break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.IncrementVolume();
                            break;

                        default:
                            sendAppCommandMessage = true;
                            break;
                    }
                    break;

                case SpotifyAction.VolumeDown:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeDown);
                    switch (Settings.Current.VolumeControlMode)
                    {
                        case ToastifyVolumeControlMode.Spotify:
                            this.SendShortcut(action);
                            break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.DecrementVolume();
                            break;

                        default:
                            sendAppCommandMessage = true;
                            break;
                    }
                    break;

                case SpotifyAction.Mute:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.Mute);
                    switch (Settings.Current.VolumeControlMode)
                    {
                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.ToggleMute();
                            break;

                        default:
                            sendAppCommandMessage = true;
                            break;
                    }
                    break;

                default:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, $"{Analytics.ToastifyEvent.Action.Default}{action}");
                    sendAppCommandMessage = true;
                    break;
            }

            if (sendAppCommandMessage)
                Win32API.SendAppCommandMessage(this.GetMainWindowHandle(), (IntPtr)action);
        }

        /// <summary>
        ///   Sends a series of messages to the Spotify process to simulate a keyboard shortcut inside the app itself.
        ///   Works also if Spotify is minimized (normally or to the tray).
        ///   <para> See https://gist.github.com/aleab/9efa67e5b1a885c2c72cfbe7cf012249 for details. </para>
        /// </summary>
        /// <param name="action"> The action to simulate. </param>
        private void SendShortcut(SpotifyAction action)
        {
            IntPtr mainWindow = this.GetMainWindowHandle();
            IntPtr cefWidgetWindow = this.GetCefWidgetWindowHandle();

            if (mainWindow == IntPtr.Zero)
                logger.ErrorExt("Main window is null.");
            else if (cefWidgetWindow == IntPtr.Zero)
                logger.ErrorExt("CefWidget window is null.");
            else
            {
                // The 'Playback' sub-menu
                const int subMenuPos = 3;
                // The base accelerator id: each item's id is obtained adding its zero-based position in the drop-down menu.
                const uint baseAcceleratorId = 0x00000072;

                List<Key> modifierKeys;
                Key key;
                int menuItemPos;

                switch (action)
                {
                    case SpotifyAction.FastForward:
                        modifierKeys = new List<Key> { Key.LeftShift };
                        key = Key.Right;
                        menuItemPos = 3;
                        break;

                    case SpotifyAction.Rewind:
                        modifierKeys = new List<Key> { Key.LeftShift };
                        key = Key.Left;
                        menuItemPos = 4;
                        break;

                    case SpotifyAction.VolumeUp:
                        modifierKeys = new List<Key> { Key.LeftCtrl };
                        key = Key.Up;
                        menuItemPos = 7;
                        break;

                    case SpotifyAction.VolumeDown:
                        modifierKeys = new List<Key> { Key.LeftCtrl };
                        key = Key.Down;
                        menuItemPos = 8;
                        break;

                    default:
                        return;
                }

                // WM_KEYDOWN: hold down the keys
                foreach (var k in modifierKeys)
                    Win32API.SendKeyDown(cefWidgetWindow, k, true);
                Win32API.SendKeyDown(cefWidgetWindow, key, true, true);
                Thread.Sleep(30);

                // WM_INITMENU: select menu
                IntPtr hMenu = Win32API.GetMenu(mainWindow);
                Win32API.SendWindowMessage(mainWindow, Win32API.WindowsMessagesFlags.WM_INITMENU, hMenu, IntPtr.Zero);

                // WM_INITMENUPOPUP: select sub-menu ('Playback')
                IntPtr hSubMenu = Win32API.GetSubMenu(hMenu, subMenuPos);
                Win32API.SendWindowMessage(mainWindow, Win32API.WindowsMessagesFlags.WM_INITMENUPOPUP, hSubMenu, (IntPtr)subMenuPos);

                // WM_COMMAND: accelerator keystroke (shortcut)
                Win32API.SendWindowMessage(mainWindow, Win32API.WindowsMessagesFlags.WM_COMMAND, (IntPtr)(0x00010000 | (baseAcceleratorId + menuItemPos)), IntPtr.Zero);

                // WM_UNINITMENUPOPUP: destroy the sub-menu
                Win32API.SendWindowMessage(mainWindow, Win32API.WindowsMessagesFlags.WM_UNINITMENUPOPUP, hSubMenu, IntPtr.Zero);

                // The following message is not needed: it will be sent automatically by <something> to the main window to notify the event (to change the UI?).
                // WM_COMMAND: notification that the keystroke has been translated
                //Win32API.PostMessage(mainWindow, (uint)Win32API.WindowsMessagesFlags.WM_COMMAND, (IntPtr)0x00007D01, IntPtr.Zero);

                // WM_KEYUP: release the keys
                Win32API.SendKeyUp(cefWidgetWindow, key, true, true);
                Thread.Sleep(30);
                foreach (var k in modifierKeys)
                    Win32API.SendKeyDown(cefWidgetWindow, k, true);
            }
        }

        private string GetSpotifyPath()
        {
            string spotifyPath = null;
            try
            {
                spotifyPath = ToastifyAPI.Spotify.GetSpotifyPath();
            }
            catch (Exception e)
            {
                logger.ErrorInvariantCulture("Error while getting Spotify executable path.", e);
            }
            return spotifyPath;
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

        private void Spotify_Connected(object sender, SpotifyStateEventArgs e)
        {
            this.Connected?.Invoke(sender, e);
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
            this.TrackTimeChanged?.Invoke(this, new SpotifyTrackTimeChangedEventArgs(e.TrackTime, this.Status?.Track, this.Status?.Playing ?? true));
        }

        private void SpotifyLocalAPI_OnVolumeChange(object sender, VolumeChangeEventArgs e)
        {
            this.VolumeChanged?.Invoke(this, new SpotifyVolumeChangedEventArgs(e.OldVolume, e.NewVolume));
        }

        #endregion Event handlers
    }
}