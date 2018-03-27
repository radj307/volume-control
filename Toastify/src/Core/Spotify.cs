using log4net;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;
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

        #region Spotify Launcher

        private BackgroundWorker spotifyLauncher;

        private Timer spotifyLauncherTimeoutTimer;

        private readonly EventWaitHandle spotifyLauncherWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Toastify_SpotifyLauncherWaitHandle");

        #endregion Spotify Launcher

        private SpotifyLocalAPI localAPI;

        private SpotifyLocalAPIConfig localAPIConfig;

        private readonly string spotifyPath;

        private Process spotifyProcess;

        private Song _currentSong;

        private bool? _isPlaying;

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
                WindowStylesFlags windowStyles = (WindowStylesFlags)Windows.GetWindowLongPtr(hWnd, GWL.GWL_STYLE);
                return (windowStyles & WindowStylesFlags.WS_MINIMIZE) != 0L || this.IsMinimizedToTray;
            }
        }

        public bool IsMinimizedToTray
        {
            get
            {
                if (!this.IsRunning)
                    return false;

                var hWnd = this.GetMainWindowHandle();
                WindowStylesFlags windowStyles = (WindowStylesFlags)Windows.GetWindowLongPtr(hWnd, GWL.GWL_STYLE);
                return (windowStyles & WindowStylesFlags.WS_MINIMIZE) == 0L && (windowStyles & WindowStylesFlags.WS_VISIBLE) == 0L;
            }
        }

        public StatusResponse Status { get { return this.localAPI?.GetStatus(); } }

        public Song CurrentSong
        {
            get { return this._currentSong ?? (this._currentSong = this.Status.Track); }
            private set { this._currentSong = value; }
        }

        public bool IsPlaying
        {
            get { return this._isPlaying ?? (this._isPlaying = this.Status?.Playing ?? false).Value; }
            private set { this._isPlaying = value; }
        }

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
            this.spotifyPath = GetSpotifyPath();
            this.InitLocalAPI();
        }

        public void InitLocalAPI()
        {
            this.DisposeLocalAPI();

            this.localAPIConfig = new SpotifyLocalAPIConfig { TimerInterval = 500 };
            this.localAPI = new SpotifyLocalAPI(this.localAPIConfig);

            this.localAPI.OnTrackChange += this.SpotifyLocalAPI_OnTrackChange;
            this.localAPI.OnPlayStateChange += this.SpotifyLocalAPI_OnPlayStateChange;
            this.localAPI.OnTrackTimeChange += this.SpotifyLocalAPI_OnTrackTimeChange;
            this.localAPI.OnVolumeChange += this.SpotifyLocalAPI_OnVolumeChange;
            this.localAPI.ListenForEvents = true;
        }

        public void StartSpotify()
        {
            this.spotifyLauncher = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.spotifyLauncher.DoWork += this.StartSpotify_WorkerTask;
            this.spotifyLauncher.RunWorkerCompleted += this.StartSpotify_WorkerTaskCompleted;

            if (Settings.Current.StartupWaitTimeout < 60000)
                Settings.Current.StartupWaitTimeout = 60000;
            this.spotifyLauncherTimeoutTimer = new Timer(Settings.Current.StartupWaitTimeout) { AutoReset = false };
            this.spotifyLauncherTimeoutTimer.Elapsed += this.SpotifyLauncherTimeoutTimer_Elapsed;

            this.spotifyLauncher.RunWorkerAsync();
            this.spotifyLauncherTimeoutTimer.Start();
        }

        #region Spotify Launcher background worker

        private void SpotifyLauncherTimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.spotifyLauncher.CancelAsync();
        }

        private void StartSpotify_WorkerTask(object sender, DoWorkEventArgs e)
        {
            this.spotifyProcess = !this.IsRunning ? this.LaunchSpotifyAndWaitForInputIdle(e) : ToastifyAPI.Spotify.FindSpotifyProcess();
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
                        logger.Error("Error while starting Spotify.", applicationStartupException);

                        string errorMsg = Properties.Resources.ERROR_STARTUP_SPOTIFY;
                        MessageBox.Show($"{errorMsg}\n{applicationStartupException.Message}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(applicationStartupException, true);
                    }
                    else if (e.Error is WebException webException)
                    {
                        logger.Error("Web exception while starting Spotify.", webException);

                        string errorMsg = Properties.Resources.ERROR_STARTUP_RESTART;
                        string status = $"{webException.Status}";
                        if (webException.Status == WebExceptionStatus.ProtocolError)
                            status += $" ({(webException.Response as HttpWebResponse)?.StatusCode}, \"{(webException.Response as HttpWebResponse)?.StatusDescription}\")";
                        string techDetails = $"Technical details: {webException.Message}\n{webException.HResult}, {status}";
                        MessageBox.Show($"{errorMsg}\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(webException, true);
                    }
                    else
                    {
                        logger.Error("Unknown error while starting Spotify.", e.Error);

                        string errorMsg = Properties.Resources.ERROR_UNKNOWN;
                        string techDetails = $"Technical Details: {e.Error.Message}\n{e.Error.StackTrace}";
                        MessageBox.Show($"{errorMsg}\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(e.Error, true);
                    }
                }
                else // e.Cancelled
                {
                    logger.Error("Toastify was not able to find Spotify within the timeout interval.");

                    string errorMsg = Properties.Resources.ERROR_STARTUP_SPOTIFY;
                    const string techDetails = "Technical Details: timeout";
                    MessageBox.Show($"{errorMsg}\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Terminate Toastify
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() => Application.Current.Shutdown()));
            }
            else
                this.Spotify_Connected(this, new SpotifyStateEventArgs(this.Status));

            this.DisposeSpotifyLauncher();
            this.DisposeSpotifyLauncherTimeoutTimer();

            this.spotifyLauncherWaitHandle.Close();
        }

        /// <summary>
        /// Starts the Spotify process and waits for it to enter an idle state.
        /// </summary>
        /// <returns> The started process. </returns>
        private Process LaunchSpotifyAndWaitForInputIdle(DoWorkEventArgs e)
        {
            logger.Info("Launching Spotify...");

            // Launch Spotify.
            this.spotifyProcess = Process.Start(this.spotifyPath, App.SpotifyParameters);

            // If it is an UWP app, then Process.Start should return null: we need to look for the process.
            bool signaled = false;
            while (this.spotifyProcess == null && !signaled)
            {
                this.spotifyProcess = ToastifyAPI.Spotify.FindSpotifyProcess();
                signaled = this.spotifyLauncherWaitHandle.WaitOne(1000);
                if (this.spotifyLauncher.CheckCancellation(e))
                    return this.spotifyProcess;
            }

            // ReSharper disable once RedundantToStringCall
            if (this.spotifyProcess != null)
                logger.Info($"Spotify process started with ID {this.spotifyProcess.Id}{(!string.IsNullOrWhiteSpace(App.SpotifyParameters) ? $" and arguments \"{App.SpotifyParameters}\"" : string.Empty)}");

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

            logger.Info("Connecting to Spotify's local APIs endpoint...");

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
                    if (ex.InnerException is SocketException socketException && socketException.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        bool hostRedirected = Regex.IsMatch(socketException.Message, @"(127\.0\.0\.1|localhost|0\.0\.0\.0):80(?![0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                        // Check if "open.spotify.com" is blocked by the firewall or redirected in the hosts file
                        using (HttpClient http = new HttpClient())
                        {
                            using (HttpRequestMessage request = new HttpRequestMessage())
                            {
                                request.Method = HttpMethod.Head;
                                request.RequestUri = new Uri("http://open.spotify.com");
                                request.Headers.Add("User-Agent", "Spotify (1.0.50.41368.gbd68dbef)");

                                try
                                {
                                    using (http.SendAsync(request).Result) { }
                                }
                                catch
                                {
                                    logger.Error("Couldn't access \"open.spotify.com\": the client blocked the connection to the host.");
                                    throw new ApplicationStartupException(hostRedirected
                                        ? Properties.Resources.ERROR_STARTUP_SPOTIFY_API_CONNECTION_BLOCKED_HOSTS
                                        : Properties.Resources.ERROR_STARTUP_SPOTIFY_API_CONNECTION_BLOCKED, false);
                                }
                            }
                        }
                    }

                    logger.Warn("WebException while connecting to Spotify.", ex);
                }
            } while (!connected && !signaled);

            if (!connected)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_CONNECT);
            if (logger.IsDebugEnabled)
                logger.Debug($"Connected with Spotify on \"{this.localAPIConfig.HostUrl}:{this.localAPIConfig.Port}/\".");

            // Try to get a status report from Spotify
            var status = this.localAPI.GetStatus();
            if (status == null)
                throw new ApplicationStartupException(Properties.Resources.ERROR_STARTUP_SPOTIFY_API_STATUS_NULL);
        }

        #endregion Spotify Launcher background worker

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
                User32.ShowWindow(hWnd, ShowWindowCmd.SW_SHOWMINIMIZED);
            }
        }

        public void Kill()
        {
            //Can't kindly close Spotify this way anymore since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
            //this.SendShortcut(ToastifyAction.Exit);

            try
            {
                if (this.spotifyProcess?.Handle != IntPtr.Zero)
                {
                    this.spotifyProcess?.CloseMainWindow();
                    if (this.spotifyProcess?.HasExited == false)
                        this.spotifyProcess?.Kill();
                }
                this.spotifyProcess?.Close();
                this.localAPI.Dispose();
            }
            catch { /* ignore */ }
        }

        private void ShowSpotify()
        {
            if (this.IsRunning)
            {
                var hWnd = this.GetMainWindowHandle();

                // check Spotify's current window state
                var placement = new WindowPlacement();
                User32.GetWindowPlacement(hWnd, ref placement);

                var showCommand = ShowWindowCmd.SW_SHOW;
                if (placement.showCmd == ShowWindowCmd.SW_SHOWMINIMIZED || placement.showCmd == ShowWindowCmd.SW_HIDE)
                    showCommand = ShowWindowCmd.SW_RESTORE;

                if (this.IsMinimizedToTray)
                {
                    // TODO: Restore Spotify if minimized to the tray.

                    return;

                    //IntPtr renderWindowHandle = Win32API.GetProcessWindows((uint)this.spotifyProcess.Id, "Chrome_WidgetWin_0")
                    //                                    .Select(Win32API.GetChildWindows)
                    //                                    .SingleOrDefault(children => children != null && children.Any(h => Win32API.GetClassName(h) == "Chrome_RenderWidgetHostHWND"))
                    //                                   ?.SingleOrDefault() ?? IntPtr.Zero;

                    //Win32API.ShowWindow(hWnd, showCommand);
                    //if (renderWindowHandle != IntPtr.Zero)
                    //{
                    //    IntPtr parent = Win32API.GetParent(renderWindowHandle);
                    //    if (parent != hWnd)
                    //    {
                    //        Win32API.SetParent(renderWindowHandle, hWnd);
                    //        Win32API.SendWindowMessage(renderWindowHandle, Win32API.WindowsMessagesFlags.WM_CHILDACTIVATE, IntPtr.Zero, IntPtr.Zero);
                    //        Win32API.ShowWindow(renderWindowHandle, Win32API.ShowWindowCmd.SW_SHOW);
                    //        Win32API.ShowWindow(renderWindowHandle, Win32API.ShowWindowCmd.SW_RESTORE);

                    //        IntPtr hDC = Win32API.GetDC(renderWindowHandle);
                    //        Win32API.SendWindowMessage(renderWindowHandle, Win32API.WindowsMessagesFlags.WM_ERASEBKGND, hDC, IntPtr.Zero);
                    //        Win32API.ReleaseDC(renderWindowHandle, hDC);

                    //        Win32API.UpdateWindow(renderWindowHandle);
                    //    }
                    //    else
                    //        Win32API.AddVisibleWindowStyle(renderWindowHandle);
                    //}
                }
                else
                    User32.ShowWindow(hWnd, showCommand);

                User32.SetForegroundWindow(hWnd);
                User32.SetFocus(hWnd);
            }
        }

        private IntPtr GetMainWindowHandle()
        {
            if (this.spotifyProcess == null)
                this.spotifyProcess = ToastifyAPI.Spotify.FindSpotifyProcess();

            return this.spotifyProcess == null ? IntPtr.Zero : ToastifyAPI.Spotify.GetMainWindowHandle((uint)this.spotifyProcess.Id);
        }

        public void SendAction(ToastifyAction action)
        {
            if (!this.IsRunning)
                return;

            bool sendAppCommandMessage = false;
            bool sendMediaKey = false;

            switch (action)
            {
#if DEBUG
                case ToastifyAction.ShowDebugView:
#endif
                case ToastifyAction.None:
                case ToastifyAction.CopyTrackInfo:
                case ToastifyAction.PasteTrackInfo:
                case ToastifyAction.ThumbsUp:
                case ToastifyAction.ThumbsDown:
                case ToastifyAction.ShowToast:
                case ToastifyAction.SettingsSaved:
                case ToastifyAction.Exit:
                    break;

                case ToastifyAction.ShowSpotify:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.ShowSpotify);
                    if (this.IsMinimized)
                        this.ShowSpotify();
                    else
                        this.Minimize();
                    break;

                case ToastifyAction.VolumeUp:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeUp);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        //    this.SendShortcut(action);
                        //    break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.IncrementVolume();
                            break;

                        case ToastifyVolumeControlMode.SystemGlobal:
                        default:
                            sendMediaKey = true;
                            break;
                    }
                    break;

                case ToastifyAction.VolumeDown:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeDown);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        //    this.SendShortcut(action);
                        //    break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.DecrementVolume();
                            break;

                        case ToastifyVolumeControlMode.SystemGlobal:
                        default:
                            sendMediaKey = true;
                            break;
                    }
                    break;

                case ToastifyAction.Mute:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.Mute);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            this.localAPI.ToggleMute();
                            break;

                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        case ToastifyVolumeControlMode.SystemGlobal:
                        default:
                            sendMediaKey = true;
                            break;
                    }
                    break;

                case ToastifyAction.FastForward:
                case ToastifyAction.Rewind:
                case ToastifyAction.PlayPause:
                case ToastifyAction.PreviousTrack:
                case ToastifyAction.NextTrack:
                default:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, $"{Analytics.ToastifyEvent.Action.Default}{action}");
                    sendAppCommandMessage = true;
                    break;
            }

            if (sendAppCommandMessage)
                Windows.SendAppCommandMessage(this.GetMainWindowHandle(), (IntPtr)action, true);
            if (sendMediaKey)
                Win32API.SendMediaKey(action);
        }

        private static string GetSpotifyPath()
        {
            string path = null;
            try
            {
                path = ToastifyAPI.Spotify.GetSpotifyPath();
                logger.Info($"Spotify executable found: \"{path}\"");
            }
            catch (Exception e)
            {
                logger.Error("Error while getting Spotify executable path.", e);
            }
            return path;
        }

        #region Dispose

        public static void DisposeInstance()
        {
            if (_instance != null)
            {
                _instance.Dispose();
                _instance = null;
            }
        }

        public void Dispose()
        {
            this.DisposeLocalAPI();
            this.DisposeSpotifyLauncher();
            this.DisposeSpotifyLauncherTimeoutTimer();
        }

        private void DisposeLocalAPI()
        {
            if (this.localAPI != null)
            {
                this.localAPI.ListenForEvents = false;
                this.localAPI.OnTrackChange -= this.SpotifyLocalAPI_OnTrackChange;
                this.localAPI.OnPlayStateChange -= this.SpotifyLocalAPI_OnPlayStateChange;
                this.localAPI.OnTrackTimeChange -= this.SpotifyLocalAPI_OnTrackTimeChange;
                this.localAPI.OnVolumeChange -= this.SpotifyLocalAPI_OnVolumeChange;

                this.localAPI.Dispose();
                this.localAPI = null;
            }

            this.localAPIConfig = null;
        }

        private void DisposeSpotifyLauncher()
        {
            if (this.spotifyLauncher != null)
            {
                this.spotifyLauncher.DoWork -= this.StartSpotify_WorkerTask;
                this.spotifyLauncher.RunWorkerCompleted -= this.StartSpotify_WorkerTaskCompleted;

                this.spotifyLauncher.Dispose();
                this.spotifyLauncher = null;
            }
        }

        private void DisposeSpotifyLauncherTimeoutTimer()
        {
            if (this.spotifyLauncherTimeoutTimer != null)
            {
                this.spotifyLauncherTimeoutTimer.Enabled = false;
                this.spotifyLauncherTimeoutTimer.Elapsed -= this.SpotifyLauncherTimeoutTimer_Elapsed;

                this.spotifyLauncherTimeoutTimer.Close();
                this.spotifyLauncherTimeoutTimer = null;
            }
        }

        #endregion Dispose

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
            this.SongChanged?.Invoke(this, new SpotifyTrackChangedEventArgs(e.OldTrack, this.CurrentSong));
        }

        private void SpotifyLocalAPI_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            this.IsPlaying = e.Playing;
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