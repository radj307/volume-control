using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using log4net;
using Toastify.Common;
using Toastify.DI;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Properties;
using Toastify.Services;
using ToastifyAPI.Core;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Events;
using ToastifyAPI.Model.Interfaces;
using ToastifyAPI.Native;
using Settings = Toastify.Model.Settings;

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

        #endregion

        private readonly string spotifyPath;

        private SpotifyWindow spotifyWindow;
        private Process spotifyProcess;

        #region Public Properties

        [PropertyDependency]
        public ITokenManager TokenManager { get; set; }

        [PropertyDependency]
        public ISpotifyWeb Web { get; set; }

        [PropertyDependency]
        public IToastifyBroadcaster Broadcaster { get; set; }

        public bool IsRunning
        {
            get { return this.spotifyWindow?.IsValid ?? false; }
        }

        public ISpotifyTrack CurrentTrack { get; private set; }

        public bool IsPlaying { get; private set; }

        #endregion

        #region Events

        public event EventHandler Exited;

        public event EventHandler<SpotifyStateEventArgs> WebAPIInitializationSucceeded;

        public event EventHandler<SpotifyWebAPIInitializationFailedEventArgs> WebAPIInitializationFailed;

        public event EventHandler WebAPIDisabled;

        public event EventHandler<SpotifyStateEventArgs> Connected;

        public event EventHandler<SpotifyTrackChangedEventArgs> TrackChanged;

        public event EventHandler<SpotifyPlayStateChangedEventArgs> PlayStateChanged;

        public event EventHandler<SpotifyTrackTimeChangedEventArgs> TrackTimeChanged;

        public event EventHandler<SpotifyVolumeChangedEventArgs> VolumeChanged;

        #endregion

        protected Spotify()
        {
            this.spotifyPath = GetSpotifyPath();

            Settings.CurrentSettingsChanged += this.Settings_CurrentSettingsChanged;

            // TODO: ITokenManager dependency is currently manually resolved. Not good!
            if (this.TokenManager == null)
                this.TokenManager = App.Container.Resolve<ITokenManager>();
            this.TokenManager.TokenNull += this.TokenManager_TokenNull;

            // TODO: ISpotifyWeb dependency is currently manually resolved. Not good!
            if (this.Web == null)
                this.Web = App.Container.Resolve<ISpotifyWeb>();

            // TODO: IToastifyBroadcaster dependency is currently manually resolved. Not good!
            if (this.Broadcaster == null)
                this.Broadcaster = App.Container.Resolve<IToastifyBroadcaster>();
        }

        public void StartSpotify()
        {
            this.spotifyLauncher = new BackgroundWorker { WorkerSupportsCancellation = true };
            this.spotifyLauncher.DoWork += this.StartSpotify_WorkerTask;
            this.spotifyLauncher.RunWorkerCompleted += this.StartSpotify_WorkerTaskCompleted;

            if (Settings.Current.StartupWaitTimeout < 60000)
                Settings.Current.StartupWaitTimeout = 60000;
            this.spotifyLauncherTimeoutTimer = new PausableTimer(Settings.Current.StartupWaitTimeout) { AutoReset = false };
            this.spotifyLauncherTimeoutTimer.Elapsed += this.SpotifyLauncherTimeoutTimer_Elapsed;

            this.spotifyLauncher.RunWorkerAsync();
            this.spotifyLauncherTimeoutTimer.Start();
        }

        public void Kill()
        {
            //Can't kindly close Spotify this way anymore since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
            //this.SendShortcut(ToastifyActionEnum.Exit);

            try
            {
                if (this.spotifyProcess?.Handle != IntPtr.Zero)
                {
                    this.spotifyProcess?.CloseMainWindow();
                    if (this.spotifyProcess?.HasExited == false)
                        this.spotifyProcess?.Kill();
                }

                this.spotifyProcess?.Close();
            }
            catch
            {
                /* ignore */
            }
        }

        public void SendAction(ToastifyActionEnum action)
        {
            if (!this.IsRunning)
                return;

            bool sendAppCommandMessage = false;
            bool sendMediaKey = false;

            switch (action)
            {
#if DEBUG
                case ToastifyActionEnum.ShowDebugView:
#endif
                case ToastifyActionEnum.None:
                case ToastifyActionEnum.CopyTrackInfo:
                case ToastifyActionEnum.PasteTrackInfo:
                case ToastifyActionEnum.ThumbsUp:
                case ToastifyActionEnum.ThumbsDown:
                case ToastifyActionEnum.ShowToast:
                case ToastifyActionEnum.SettingsSaved:
                case ToastifyActionEnum.Exit:
                    break;

                case ToastifyActionEnum.ShowSpotify:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.ShowSpotify);
                    if (this.IsMinimized)
                        this.ShowSpotify();
                    else
                        this.Minimize();
                    break;

                case ToastifyActionEnum.VolumeUp:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeUp);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        //    this.SendShortcut(action);
                        //    break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            VolumeHelper.IncrementVolume(Settings.Current.WindowsVolumeMixerIncrement);
                            break;

                        case ToastifyVolumeControlMode.SystemGlobal:
                            sendMediaKey = true;
                            break;

                        default:
                            sendMediaKey = true;
                            break;
                    }

                    break;

                case ToastifyActionEnum.VolumeDown:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.VolumeDown);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        //    this.SendShortcut(action);
                        //    break;

                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            VolumeHelper.DecrementVolume(Settings.Current.WindowsVolumeMixerIncrement);
                            break;

                        case ToastifyVolumeControlMode.SystemGlobal:
                        default:
                            sendMediaKey = true;
                            break;
                    }

                    break;

                case ToastifyActionEnum.Mute:
                    Analytics.TrackEvent(Analytics.ToastifyEventCategory.Action, Analytics.ToastifyEvent.Action.Mute);
                    switch ((ToastifyVolumeControlMode)Settings.Current.VolumeControlMode)
                    {
                        case ToastifyVolumeControlMode.SystemSpotifyOnly:
                            VolumeHelper.ToggleMute();
                            break;

                        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                        //case ToastifyVolumeControlMode.Spotify:
                        case ToastifyVolumeControlMode.SystemGlobal:
                        default:
                            sendMediaKey = true;
                            break;
                    }

                    break;

                case ToastifyActionEnum.FastForward:
                case ToastifyActionEnum.Rewind:
                case ToastifyActionEnum.Stop:
                case ToastifyActionEnum.PlayPause:
                case ToastifyActionEnum.PreviousTrack:
                case ToastifyActionEnum.NextTrack:
                    goto default;

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

        public void VolumeUp()
        {
            VolumeHelper.IncrementVolume(Settings.Current.WindowsVolumeMixerIncrement);
        }

        public void VolumeDown()
        {
            VolumeHelper.DecrementVolume(Settings.Current.WindowsVolumeMixerIncrement);
        }

        public void ToggleMute()
        {
            VolumeHelper.ToggleMute();
        }

        #region Static Members

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

        #endregion

        #region Spotify Launcher

        private BackgroundWorker spotifyLauncher;

        private PausableTimer spotifyLauncherTimeoutTimer;

        private readonly EventWaitHandle spotifyLauncherWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, "Toastify_SpotifyLauncherWaitHandle");

        private void SpotifyLauncherTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.spotifyLauncher.CancelAsync();
        }

        private void StartSpotify_WorkerTask(object sender, DoWorkEventArgs e)
        {
            var process = ToastifyAPI.Spotify.FindSpotifyProcess();

            this.spotifyProcess = null;
            if (!this.IsRunning && process == null)
            {
                logger.Debug("Spotify is not running; lanching it nad waiting for input idle...");
                this.spotifyProcess = this.LaunchSpotifyAndWaitForInputIdle(e);
            }
            else
                this.spotifyProcess = process;

            if (e.Cancel)
                return;
            if (this.spotifyProcess == null)
                throw new ApplicationStartupException(Resources.ERROR_STARTUP_PROCESS);

            this.spotifyProcess.EnableRaisingEvents = true;
            this.spotifyProcess.Exited += this.Spotify_Exited;

            this.spotifyWindow = new SpotifyWindow(this.spotifyProcess);
            this.spotifyWindow.InitializationFinished += this.SpotifyWindow_InitializationFinished;

            //this.ConnectWithSpotify(e);
        }

        private void StartSpotify_WorkerTaskCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                if (e.Error != null)
                {
                    if (e.Error is ApplicationStartupException applicationStartupException)
                    {
                        logger.Fatal("Error while starting Spotify.", applicationStartupException);

                        string errorMsg = Resources.ERROR_STARTUP_SPOTIFY;
                        MessageBox.Show($"{errorMsg}{Environment.NewLine}{applicationStartupException.Message}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(applicationStartupException, true);
                    }
                    else if (e.Error is WebException webException)
                    {
                        logger.Fatal("Web exception while starting Spotify.", webException);

                        string errorMsg = Resources.ERROR_STARTUP_RESTART;
                        string status = $"{webException.Status}";
                        if (webException.Status == WebExceptionStatus.ProtocolError)
                            status += $" ({(webException.Response as HttpWebResponse)?.StatusCode}, \"{(webException.Response as HttpWebResponse)?.StatusDescription}\")";
                        string techDetails = $"Technical details: {webException.Message}{Environment.NewLine}{webException.HResult}, {status}";
                        MessageBox.Show($"{errorMsg}{Environment.NewLine}{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(webException, true);
                    }
                    else
                    {
                        logger.Fatal("Unknown error while starting Spotify.", e.Error);

                        string errorMsg = Resources.ERROR_UNKNOWN;
                        string techDetails = $"Technical Details: {e.Error.Message}{Environment.NewLine}{e.Error.StackTrace}";
                        MessageBox.Show($"{errorMsg}{Environment.NewLine}{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                        Analytics.TrackException(e.Error, true);
                    }
                }
                else // e.Cancelled
                {
                    logger.Fatal($"Toastify was not able to find or connect to Spotify within the timeout interval ({Settings.Current.StartupWaitTimeout / 1000} seconds).");

                    string errorMsg = Resources.ERROR_STARTUP_SPOTIFY_TIMEOUT;
                    MessageBoxResult choice = MessageBoxResult.No;
                    App.CallInSTAThread(() =>
                    {
                        choice = CustomMessageBox.ShowYesNo(
                            $"{errorMsg}{Environment.NewLine}Do you need to set up or change your proxy details?{Environment.NewLine}{Environment.NewLine}Toastify will terminate regardless of your choice.",
                            "Toastify",
                            "Yes", // Yes
                            "No",  // No
                            MessageBoxImage.Error);
                    }, true);

                    if (choice == MessageBoxResult.Yes)
                        this.ChangeProxySettings();
                }

                // Terminate Toastify
                this.spotifyLauncherWaitHandle.WaitOne(2000);
                App.Terminate();
            }
            //else
            //    this.Spotify_Connected(this, new SpotifyStateEventArgs(this.Status));

            this.DisposeSpotifyLauncher();
            this.DisposeSpotifyLauncherTimeoutTimer();

            this.spotifyLauncherWaitHandle.Close();
        }

        /// <summary>
        ///     Starts the Spotify process and waits for it to enter an idle state.
        /// </summary>
        /// <returns> The started process. </returns>
        private Process LaunchSpotifyAndWaitForInputIdle(DoWorkEventArgs e)
        {
            try
            {
                logger.Info("Launching Spotify...");

                this.spotifyLauncherWaitHandle.Reset();
                bool signaled = false;

                if (string.IsNullOrWhiteSpace(this.spotifyPath))
                    throw new ApplicationStartupException(Resources.ERROR_STARTUP_SPOTIFY_NOT_FOUND);

                // Launch Spotify.
                this.spotifyProcess = Process.Start(this.spotifyPath, App.SpotifyParameters);

                // If it is an UWP app, then Process.Start should return null: we need to look for the process.
                while (this.spotifyProcess == null && !signaled)
                {
                    this.spotifyProcess = ToastifyAPI.Spotify.FindSpotifyProcess();
                    signaled = this.spotifyLauncherWaitHandle.WaitOne(1000);
                    if (this.spotifyLauncher.CheckCancellation(e))
                    {
                        logger.Error("Toastify timed out while looking for Spotify's process");
                        return this.spotifyProcess;
                    }
                }

                // ReSharper disable once RedundantToStringCall
                if (this.spotifyProcess != null)
                    logger.Info($"Spotify process started with ID {this.spotifyProcess.Id}{(!string.IsNullOrWhiteSpace(App.SpotifyParameters) ? $" and arguments \"{App.SpotifyParameters}\"" : string.Empty)}");

                // We need to let Spotify start-up before interacting with it.
                while (this.spotifyProcess?.WaitForInputIdle(1000) != true && !signaled)
                {
                    signaled = this.spotifyLauncherWaitHandle.WaitOne(1000);
                    if (this.spotifyLauncher.CheckCancellation(e))
                    {
                        logger.Error($"Toastify timed out while waiting for Spotify's process to enter an idle state. HasExited? {this.spotifyProcess?.HasExited}, Responding? {this.spotifyProcess?.Responding}");
                        return this.spotifyProcess;
                    }
                }

                if (Settings.Current.MinimizeSpotifyOnStartup)
                    this.Minimize(1000);

                return this.spotifyProcess;
            }
            finally
            {
                this.spotifyLauncherWaitHandle.Set();
            }
        }

        /// <summary>
        ///     Prompt the user with the specified message. If they answer Yes, then they are requested to enter their proxy details;
        ///     if they answer No, then the proxy is disabled.
        /// </summary>
        /// <param name="message"> The message </param>
        /// <returns> True if the user answered Yes; false otherwise. </returns>
        private bool AskTheUserToChangeOrDisableProxy(string message)
        {
            if (logger.IsDebugEnabled)
                logger.Debug("Asking the user if they are behind a proxy...");

            if (MessageBox.Show(message, "Toastify", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this.ChangeProxySettings();
                return true;
            }

            // Disable the proxy
            App.ProxyConfig.Set(null);
            Settings.Current.UseProxy = false;
            Settings.Current.Save();

            if (logger.IsDebugEnabled)
                logger.Debug("Use of proxy has been disabled.");

            return false;
        }

        private void ChangeProxySettings()
        {
            if (this.spotifyLauncherTimeoutTimer?.IntervalRemaining > 0.0d)
                this.spotifyLauncherTimeoutTimer.Pause();
            App.ShowConfigProxyDialog();
            if (this.spotifyLauncherTimeoutTimer?.IntervalRemaining > 0.0d)
                this.spotifyLauncherTimeoutTimer.Resume();

            if (logger.IsDebugEnabled)
            {
                var proxy = App.ProxyConfig;
                logger.Debug($"Proxy has been set to \"{(!string.IsNullOrEmpty(proxy.Username) ? $"{proxy.Username}@" : "")}{proxy.Host}:{proxy.Port}\"");
            }
        }

        #endregion Spotify Launcher background worker

        #region Spotify WebAPI

        private CancellationTokenSource webApiInitCancellationTokenSource;

        public bool IsWebApiRunning { get; private set; }

        public void EnableWebApi()
        {
            if (!this.IsWebApiRunning)
            {
                this.DisposeWebApiInitializer();
                this.webApiInitCancellationTokenSource = new CancellationTokenSource();
                this.BeginInitializeWebAPI();
            }
        }

        public void DisableWebApi()
        {
            if (this.IsWebApiRunning)
            {
                this.DisposeWebApiInitializer();
                this.TokenManager.ReleaseToken();
                this.OnWebApiDisabled();
            }
        }

        private void BeginInitializeWebAPI()
        {
            if (this.TokenManager == null || this.Web == null)
                return;

            var ct = this.webApiInitCancellationTokenSource?.Token ?? CancellationToken.None;
            if (this.TokenManager.BeginGetToken(ct, token =>
            {
                if (token != null)
                    this.OnWebAPIInitializationSucceeded();
                else
                {
                    this.OnWebAPIInitializationFailed(this.Web.Auth is NoAuth
                        ? SpotifyWebAPIInitializationFailedReason.ToastifyWebAuthAPINotFound
                        : SpotifyWebAPIInitializationFailedReason.NoToken);
                }
            }))
                logger.Debug("Begin Spotify WebAPI initialization");
        }

        private async Task<bool> UpdateTrackInfoUsingWebApi()
        {
            if (this.Web?.API == null)
                return false;

            // Pre-emptively waiting a little bit to let the remote Spotify server update its own information
            await Task.Delay(TimeSpan.FromMilliseconds(50)).ConfigureAwait(false);
            var currentlyPlayingObject = await this.Web.API.GetCurrentlyPlayingTrackAsync().ConfigureAwait(false);
            if (currentlyPlayingObject?.Track == null || !currentlyPlayingObject.Track.IsValid())
                return false;

            ISpotifyTrack newTrack = currentlyPlayingObject.Track;
            if (!SpotifyTrack.Equal(this.CurrentTrack, newTrack))
            {
                ISpotifyTrack oldTrack = this.CurrentTrack;
                this.CurrentTrack = newTrack;
                await this.OnTrackChanged(oldTrack).ConfigureAwait(false);
            }

            await this.OnPlayStateChanged(currentlyPlayingObject.IsPlaying).ConfigureAwait(false);
            return true;
        }

        #endregion

        #region SpotifyWindow wrapper methods/properties

        public bool IsMinimized
        {
            get { return this.spotifyWindow?.IsMinimized ?? false; }
        }

        public bool IsMinimizedToTray
        {
            get { return this.spotifyWindow?.IsMinimizedToTray ?? false; }
        }

        public Task Minimize(int delay = 0)
        {
            return this.spotifyWindow?.Minimize(delay);
        }

        public void ShowSpotify()
        {
            this.spotifyWindow?.Show();
        }

        private IntPtr GetMainWindowHandle()
        {
            return this.spotifyWindow?.Handle ?? IntPtr.Zero;
        }

        #endregion

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
            this.DisposeSpotifyLauncher();
            this.DisposeSpotifyLauncherTimeoutTimer();
            this.DisposeWebApiInitializer();

            this.Web?.Auth?.Dispose();
            this.TokenManager?.Dispose();
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

        private void DisposeWebApiInitializer()
        {
            try
            {
                this.webApiInitCancellationTokenSource?.Cancel();
            }
            catch
            {
                // ignore
            }

            this.webApiInitCancellationTokenSource?.Dispose();
            this.webApiInitCancellationTokenSource = null;
        }

        #endregion Dispose

        #region Event Raisers

        private async void OnWebAPIInitializationSucceeded()
        {
            logger.Debug("Spotify WebAPI initialization succeeded!");

            var currentlyPlayingObject = await this.Web.API.GetCurrentlyPlayingTrackAsync().ConfigureAwait(false);
            if (currentlyPlayingObject?.Track != null && currentlyPlayingObject.Track.IsValid())
            {
                this.CurrentTrack = currentlyPlayingObject.Track;
                this.IsPlaying = currentlyPlayingObject.IsPlaying;
            }

            this.IsWebApiRunning = true;
            this.WebAPIInitializationSucceeded?.Invoke(this, new SpotifyStateEventArgs(this.CurrentTrack, this.IsPlaying, this.CurrentTrack?.Length ?? 0.0, 1.0));
        }

        private void OnWebAPIInitializationFailed(SpotifyWebAPIInitializationFailedReason reason)
        {
            logger.Debug($"Spotify WebAPI initialization failed with reason: {reason}");
            this.IsWebApiRunning = false;
            this.WebAPIInitializationFailed?.Invoke(this, new SpotifyWebAPIInitializationFailedEventArgs(reason));
        }

        private void OnWebApiDisabled()
        {
            logger.Debug("Spotify WebAPI disabled");
            this.IsWebApiRunning = false;
            this.WebAPIDisabled?.Invoke(this, EventArgs.Empty);
        }

        private async Task OnSpotifyConnected(SpotifyStateEventArgs e)
        {
            if (logger.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Spotify Connected. Status = {{{Environment.NewLine}")
                  .Append($"   CurrentSong: {(e.CurrentTrack != null ? $"\"{e.CurrentTrack}\"" : "null")},{Environment.NewLine}")
                  .Append($"   Playing: {e.Playing},{Environment.NewLine}")
                  .Append($"   TrackTime: {e.TrackTime},{Environment.NewLine}")
                  .Append($"   Volume: {e.Volume}{Environment.NewLine}")
                  .Append("}");
                logger.Debug(sb.ToString());
            }

            this.IsPlaying = e.Playing;
            this.CurrentTrack = e.CurrentTrack;

            this.Connected?.Invoke(this, e);

            if (Settings.Current.EnableSpotifyWebApi)
            {
                this.DisposeWebApiInitializer();
                this.webApiInitCancellationTokenSource = new CancellationTokenSource();
                this.BeginInitializeWebAPI();
            }

            if (Settings.Current.EnableBroadcaster)
            {
                await this.Broadcaster.StartAsync().ConfigureAwait(false);
                await this.Broadcaster.BroadcastPlayState(e.Playing).ConfigureAwait(false);
                await this.Broadcaster.BroadcastCurrentTrack(e.CurrentTrack).ConfigureAwait(false);
            }
        }

        private async Task OnTrackChanged(ISpotifyTrack previousTrack)
        {
            this.TrackChanged?.Invoke(this, new SpotifyTrackChangedEventArgs(previousTrack, this.CurrentTrack));
            await this.Broadcaster.BroadcastCurrentTrack(this.CurrentTrack).ConfigureAwait(false);
        }

        private async Task OnPlayStateChanged(bool playing)
        {
            this.IsPlaying = playing;
            this.PlayStateChanged?.Invoke(this, new SpotifyPlayStateChangedEventArgs(playing));
            await this.Broadcaster.BroadcastPlayState(playing).ConfigureAwait(false);
        }

        private void OnTrackTimeChanged(double trackTime)
        {
            this.TrackTimeChanged?.Invoke(this, new SpotifyTrackTimeChangedEventArgs(trackTime));
        }

        private void OnVolumeChanged(double previousVolume, double newVolume)
        {
            this.VolumeChanged?.Invoke(this, new SpotifyVolumeChangedEventArgs(previousVolume, newVolume));
        }

        #endregion

        #region Event Handlers

        private async void Settings_CurrentSettingsChanged(object sender, CurrentSettingsChangedEventArgs e)
        {
            // Spotify WebAPI
            try
            {
                if (e.PreviousSettings?.EnableSpotifyWebApi != e.CurrentSettings?.EnableSpotifyWebApi)
                {
                    if (e.CurrentSettings?.EnableSpotifyWebApi == true)
                        this.EnableWebApi();
                    else
                        this.DisableWebApi();
                }
            }
            catch (Exception exception)
            {
                logger.Error($"Unhandled exception while {(e.CurrentSettings?.EnableSpotifyWebApi?.Value == true ? "enabling" : "disabling")} Spotify's WebAPI support.", exception);
            }

            // ToastifyBroadcaster
            try
            {
                if (e.PreviousSettings?.EnableBroadcaster != e.CurrentSettings?.EnableBroadcaster)
                {
                    if (e.CurrentSettings?.EnableBroadcaster == true)
                        await this.Broadcaster.StartAsync().ConfigureAwait(false);
                    else
                        await this.Broadcaster.StopAsync().ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                logger.Error($"Unhandled exception while {(e.CurrentSettings?.EnableBroadcaster?.Value == true ? "starting" : "stopping")} the broadcaster.", exception);
            }
        }

        private void Spotify_Exited(object sender, EventArgs e)
        {
            logger.Info("Spotify process terminated!");
            this.Exited?.Invoke(sender, e);
        }

        private async void SpotifyWindow_InitializationFinished(object sender, EventArgs e)
        {
            try
            {
                this.spotifyWindow.InitializationFinished -= this.SpotifyWindow_InitializationFinished;

                if (this.spotifyWindow.IsValid)
                {
                    this.spotifyWindow.TitleWatcher.TitleChanged += this.SpotifyWindowTitleWatcher_TitleChanged;

                    // Fake the Connected event
                    string currentTitle = this.spotifyWindow.Title;
                    SpotifyStateEventArgs spotifyStateEventArgs;

                    if (SpotifyWindow.PausedTitles.Contains(currentTitle, StringComparer.InvariantCulture))
                        spotifyStateEventArgs = new SpotifyStateEventArgs(null, false, 0.0, 1.0);
                    else
                    {
                        ISong newSong = Song.FromSpotifyWindowTitle(currentTitle);
                        spotifyStateEventArgs = new SpotifyStateEventArgs(newSong, true, newSong?.Length ?? 0.0, 1.0);
                    }

                    await this.OnSpotifyConnected(spotifyStateEventArgs).ConfigureAwait(false);
                }
                else
                {
                    string logError = this.spotifyProcess.HasExited ? "process has been terminated" : "null handle";
                    logger.Fatal($"Couldn't find Spotify's window: {logError}");

                    string errorMsg = Resources.ERROR_STARTUP_SPOTIFY_WINDOW_NOT_FOUND;
                    MessageBox.Show($"{errorMsg}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);

                    App.Terminate();
                }
            }
            catch (Exception exception)
            {
                logger.Error($"Unhandled exception in {nameof(this.SpotifyWindow_InitializationFinished)}.", exception);
            }
        }

        private async void SpotifyWindowTitleWatcher_TitleChanged(object sender, WindowTitleChangedEventArgs e)
        {
            // TODO: Refactor this method
            try
            {
                if (logger.IsDebugEnabled)
                    logger.Debug($"Spotify's window title changed: \"{e.NewTitle}\". Fetching song info...");

                if (!(Settings.Current.EnableSpotifyWebApi && this.IsWebApiRunning &&
                      await this.UpdateTrackInfoUsingWebApi().ConfigureAwait(false)))
                {
                    // If the WebAPIs are disabled or they weren't able to retrieve the song info, fallback to
                    // the old method based on the title of Spotify's window.

                    if (logger.IsDebugEnabled)
                        logger.Debug("Fetching song info using old method based on the title of Spotify's window...");

                    bool updateSong = true;
                    if (SpotifyWindow.PausedTitles.Contains(e.NewTitle, StringComparer.InvariantCulture))
                    {
                        await this.OnPlayStateChanged(false).ConfigureAwait(false);
                        updateSong = false;
                    }
                    else if (SpotifyWindow.PausedTitles.Contains(e.OldTitle, StringComparer.InvariantCulture))
                        await this.OnPlayStateChanged(false).ConfigureAwait(false);

                    if (updateSong)
                    {
                        ISpotifyTrack newSong = Song.FromSpotifyWindowTitle(e.NewTitle);
                        if (!SpotifyTrack.Equal(this.CurrentTrack, newSong))
                        {
                            ISpotifyTrack oldSong = this.CurrentTrack;
                            this.CurrentTrack = newSong;
                            await this.OnTrackChanged(oldSong).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error($"Unhandled exception in {nameof(this.SpotifyWindowTitleWatcher_TitleChanged)}.", exception);
            }
        }

        private void TokenManager_TokenNull(object sender, EventArgs e)
        {
            this.DisableWebApi();
        }

        #endregion
    }
}