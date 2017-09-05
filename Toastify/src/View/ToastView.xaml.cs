using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Toastify.Core;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;
using ToastifyAPI.Plugins;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;

namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class ToastView : Window
    {
        // TODO: Actually implement the MVVM pattern: create ViewModels for every View.

        private const string DEFAULT_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyLogo.png";
        private const string AD_PLAYING_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyAdPlaying.png";
        private const string ALBUM_ACCESS_DENIED_ICON = "pack://application:,,,/Toastify;component/Resources/ToastifyAccessDenied.png";
        private const string UPDATE_LOGO_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyUpdateLogo.png";

        internal static ToastView Current { get; private set; }

        #region Private fields

        private readonly ToastViewModel toastViewModel;

        private Timer minimizeTimer;

        private NotifyIcon trayIcon;

        private Song currentSong;
        private BitmapImage cover;
        private string toastIconURI = "";

        private VersionChecker versionChecker;
        private bool isUpdateToast;

        private bool dragging;
        private bool paused;

        #endregion Private fields

        public bool IsToastVisible { get; private set; }

        internal List<IPluginBase> Plugins { get; set; }

        #region Events

        public event EventHandler Started;

        public event EventHandler ToastClosing;

        #endregion Events

        public ToastView()
        {
            this.InitializeComponent();

            this.toastViewModel = new ToastViewModel();
            this.DataContext = this.toastViewModel;

            // Set a static reference back to ourselves, useful for callbacks.
            Current = this;
        }

        #region Initialization

        private void Init()
        {
            this.LoadSettings();
            this.InitToast();
            this.InitTrayIcon();
            this.StartSpotifyOrAskUser();

            // Subscribe to Spotify's events (i.e. SpotifyLocalAPI's).
            Spotify.Instance.Exited += this.Application_Shutdown;
            Spotify.Instance.SongChanged += this.Spotify_SongChanged;
            Spotify.Instance.PlayStateChanged += this.Spotify_PlayStateChanged;
            Spotify.Instance.TrackTimeChanged += this.Spotify_TrackTimeChanged;

            this.Deactivated += this.Toast_Deactivated;

            this.LoadPlugins();
            this.Started?.Invoke(this, EventArgs.Empty);

            this.InitVersionChecker();
        }

        private void LoadSettings()
        {
            try
            {
                Settings.Instance.Load();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception loading settings:\n" + ex);

                string msg = string.Format(Properties.Resources.ERROR_SETTINGS_UNABLE_TO_LOAD, Settings.Instance.SettingsFilePath);
                MessageBox.Show(msg, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                Settings.Instance.Default(true);
            }

            string version = VersionChecker.CurrentVersion;

            Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppLaunch, version);

            if (Settings.Instance.PreviousVersion != version)
            {
                Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppUpgraded, version);
                Settings.Instance.PreviousVersion = version;
            }
        }

        public void InitToast()
        {
            this.ShowToast(false);

            //If we find any invalid settings in the xml we skip it and use default.
            //User notification of bad settings will be implemented with the settings dialog.

            // TODO: Refactor InitToast method.
            //This method is UGLY but we'll keep it until the settings dialog is implemented.
            Settings settings = Settings.Instance;

            this.Width = settings.ToastWidth >= this.MinWidth ? settings.ToastWidth : this.MinWidth;
            this.Height = settings.ToastHeight >= this.MinHeight ? settings.ToastHeight : this.MinHeight;
            this.ToastBorder.BorderThickness = new Thickness(settings.ToastBorderThickness);

            ColorConverter cc = new ColorConverter();

            // Borders
            if (!string.IsNullOrEmpty(settings.ToastBorderColor) && cc.IsValid(settings.ToastBorderColor))
            {
                object borderBrush = cc.ConvertFrom(settings.ToastBorderColor);
                if (borderBrush != null)
                    this.ToastBorder.BorderBrush = new SolidColorBrush((Color)borderBrush);
            }

            // Top & Bottom
            if (!string.IsNullOrEmpty(settings.ToastColorTop) && cc.IsValid(settings.ToastColorTop) &&
                !string.IsNullOrEmpty(settings.ToastColorBottom) && cc.IsValid(settings.ToastColorBottom))
            {
                object top = cc.ConvertFrom(settings.ToastColorTop);
                object bottom = cc.ConvertFrom(settings.ToastColorBottom);

                if (top != null && bottom != null)
                    this.ToastBorder.Background = new LinearGradientBrush((Color)top, (Color)bottom, 90.0);
            }

            this.ToastBorder.CornerRadius = new CornerRadius(settings.ToastBorderCornerRadiusTopLeft, settings.ToastBorderCornerRadiusTopRight, settings.ToastBorderCornerRadiusBottomRight, settings.ToastBorderCornerRadiusBottomLeft);
        }

        private void InitTrayIcon()
        {
            this.trayIcon = new NotifyIcon
            {
                Icon = Properties.Resources.spotifyicon,
                Text = @"Toastify",
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            //Init tray icon menu
            MenuItem menuSettings = new MenuItem { Text = @"Settings" };
            menuSettings.Click += (s, ev) => { SettingsView.Launch(this); };

            MenuItem menuAbout = new MenuItem { Text = @"About Toastify..." };
            menuAbout.Click += (s, ev) => { new AboutView().ShowDialog(); };

            MenuItem menuExit = new MenuItem { Text = @"Exit" };
            menuExit.Click += this.Application_Shutdown;

            this.trayIcon.ContextMenu.MenuItems.Add(menuSettings);
            this.trayIcon.ContextMenu.MenuItems.Add(menuAbout);
            this.trayIcon.ContextMenu.MenuItems.Add("-");
            this.trayIcon.ContextMenu.MenuItems.Add(menuExit);

            this.trayIcon.MouseClick += this.TrayIcon_MouseClick;
            this.trayIcon.DoubleClick += this.TrayIcon_DoubleClick;
        }

        private void StartSpotifyOrAskUser()
        {
            try
            {
                Spotify.Instance.Connected += this.Spotify_Connected;
                Spotify.Instance.StartSpotify();
            }
            catch (ApplicationStartupException e)
            {
                Debug.WriteLine(e.StackTrace);

                string errorMsg = Properties.Resources.ERROR_STARTUP;
                string techDetails = $"Technical details\n{e.Message}";
                MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (WebException e)
            {
                Debug.WriteLine(e.StackTrace);

                string errorMsg = Properties.Resources.ERROR_STARTUP_RESTART;
                string status = $"{e.Status}";
                if (e.Status == WebExceptionStatus.ProtocolError)
                    status += $" ({(e.Response as HttpWebResponse)?.StatusCode}, \"{(e.Response as HttpWebResponse)?.StatusDescription}\")";
                string techDetails = $"Technical details: {e.Message}\n{e.HResult}, {status}";
                MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);

                string errorMsg = Properties.Resources.ERROR_UNKNOWN;
                string techDetails = $"Technical Details: {e.Message}\n{e.StackTrace}";
                MessageBox.Show($"{errorMsg}\n\n{techDetails}", "Toastify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPlugins()
        {
            this.Plugins = new List<IPluginBase>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string applicationPath = new FileInfo(assembly.Location).DirectoryName;

            foreach (var p in Settings.Instance.Plugins)
            {
                try
                {
                    string pluginFilePath = Path.Combine(applicationPath, p.FileName);
                    if (Activator.CreateInstanceFrom(pluginFilePath, p.TypeName).Unwrap() is IPluginBase plugin)
                    {
                        plugin.Init(p.Settings);
                        this.Plugins.Add(plugin);

                        this.Started += plugin.Started;
                        this.ToastClosing += plugin.Closing;
                        Spotify.Instance.SongChanged += (sender, e) => plugin.TrackChanged(sender, e);
                    }
                    else
                        Debug.WriteLine("'plugin' is not of type IPluginBase (?)");
                }
                catch (Exception)
                {
                    // TODO: Handle plugins' errors.
                }
                Console.WriteLine(@"Loaded " + p.TypeName);
            }
        }

        private void InitVersionChecker()
        {
            this.versionChecker = new VersionChecker();
            this.versionChecker.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;
            this.versionChecker.BeginCheckVersion();

            // TODO: right now this is pretty dumb - kick off update notifications every X hours, this might get annoying
            //       and really we should just pop a notification once per version and probably immediately after a song toast
            var updateTimer = new DispatcherTimer();
            updateTimer.Tick += (timerSender, timerE) => { this.versionChecker.BeginCheckVersion(); };
            updateTimer.Interval = new TimeSpan(6, 0, 0);
            updateTimer.Start();
        }

        #endregion Initialization

        /// <summary>
        /// Update current song's cover art url and toast's text.
        /// Also, save track info to file, if settings say so.
        /// </summary>
        /// <param name="song"> The song to set as current. </param>
        private void UpdateCurrentSong(Song song)
        {
            this.currentSong = song;

            this.UpdateCoverArt();
            this.UpdateToastText(this.currentSong);
            this.UpdateSongProgressBar(0.0);

            // Save track info to file.
            if (Settings.Instance.SaveTrackToFile)
            {
                if (!string.IsNullOrEmpty(Settings.Instance.SaveTrackToFilePath))
                {
                    try
                    {
                        string trackText = this.currentSong.GetClipboardText(Settings.Instance.ClipboardTemplate);
                        File.WriteAllText(Settings.Instance.SaveTrackToFilePath, trackText);
                    }
                    catch
                    {
                        // Ignore errors writing out the album.
                    }
                }
            }
        }

        private void UpdateCoverArt(bool forceUpdate = false)
        {
            // Get new CoverArtUrl.
            string previousURI = this.toastIconURI;
            this.UpdateCoverArtUrl();

            // Update the cover art only if the url has changed.
            if (!string.IsNullOrEmpty(this.toastIconURI) && (forceUpdate || this.toastIconURI != previousURI))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        this.cover = new BitmapImage();
                        this.cover.BeginInit();
                        this.cover.CacheOption = BitmapCacheOption.OnLoad;
                        this.cover.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                        try
                        {
                            this.cover.UriSource = new Uri(this.toastIconURI, UriKind.RelativeOrAbsolute);
                        }
                        catch (UriFormatException e)
                        {
                            Debug.WriteLine(e.Message);
                            this.cover.UriSource = new Uri(ALBUM_ACCESS_DENIED_ICON, UriKind.RelativeOrAbsolute);
                        }
                        finally
                        {
                            this.cover.EndInit();
                            this.AlbumArt.Source = this.cover;
                        }
                    }));
            }
        }

        private void UpdateCoverArtUrl()
        {
            if (this.currentSong == null)
                this.toastIconURI = DEFAULT_ICON;
            else
            {
                if (string.IsNullOrWhiteSpace(this.currentSong.Track))
                {
                    this.currentSong.CoverArtUrl = AD_PLAYING_ICON;
                    this.currentSong.Track = "Spotify Ad";
                }
                else if (string.IsNullOrWhiteSpace(this.currentSong?.CoverArtUrl))
                    this.currentSong.CoverArtUrl = DEFAULT_ICON;

                this.toastIconURI = this.currentSong.CoverArtUrl;
            }
        }

        /// <summary>
        /// Update the toast's text using the songs' information.
        /// </summary>
        /// <param name="song"> The song. </param>
        /// <param name="altTitle1">
        ///   An alternative text to use as the first title. If not null, this causes the
        ///   song's information to be displayed on one line as the second title.
        /// </param>
        /// <param name="fadeIn"> Whether or not to start the toast fade-in animation. </param>
        /// <param name="force"> Whether or no0t to force the toast to show up. </param>
        private void UpdateToastText(Song song, string altTitle1 = null, bool fadeIn = true, bool force = false)
        {
            string title1, title2;
            if (altTitle1 == null)
            {
                title1 = (this.paused ? "Paused" : song?.Track) ?? string.Empty;
                title2 = (this.paused ? song?.ToString() : $"by {song?.Artist}") ?? string.Empty;
            }
            else
            {
                title1 = altTitle1;
                title2 = song?.ToString() ?? string.Empty;
            }
            this.UpdateToastText(title1, title2, fadeIn, force);
        }

        /// <summary>
        /// Update the toast's text using custom strings.
        /// </summary>
        /// <param name="title1"> First title. </param>
        /// <param name="title2">  Second title. </param>
        /// <param name="fadeIn"> Whether or not to start the toast fade-in animation. </param>
        /// <param name="force"> Whether or no0t to force the toast to show up. </param>
        /// <param name="isUpdate"> Whethere or not this update is caused by <see cref="VersionChecker"/>. </param>
        private void UpdateToastText(string title1, string title2 = "", bool fadeIn = true, bool force = false, bool isUpdate = false)
        {
            this.toastViewModel.Title1 = title1;
            this.toastViewModel.Title2 = title2;
            if (fadeIn)
                this.FadeIn(force, isUpdate);
        }

        private void ShowToast(bool shallBeVisible)
        {
            this.Topmost = shallBeVisible;
            this.Visibility = shallBeVisible ? Visibility.Visible : Visibility.Collapsed;
            Win32API.ShowWindow(this.GetHandle(), shallBeVisible ? Win32API.ShowWindowCmd.SW_RESTORE : Win32API.ShowWindowCmd.SW_SHOWMINNOACTIVE);

            if (shallBeVisible)
            {
                this.Left = Settings.Instance.PositionLeft;
                this.Top = Settings.Instance.PositionTop;
                this.ResetPositionIfOffScreen();
            }
        }

        private void FadeIn(bool force = false, bool isUpdate = false)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    this.minimizeTimer?.Stop();

                    bool doNotFadeIn = this.dragging ||
                                       (Settings.Instance.DisableToast || (Settings.Instance.OnlyShowToastOnHotkey && !force && !this.IsToastVisible)) ||
                                       (Settings.Instance.DisableToastWithFullscreenVideogames && Win32API.IsForegroundAppAFullscreenVideogame());
                    if (doNotFadeIn)
                        return;

                    // this is a convenient place to reset the idle timer (if so asked)
                    // as this will be triggered when a song is played. The primary problem is if there is a
                    // particularly long song then this will not work. That said, this is the safest (in terms of
                    // not causing a user's computer from never sleeping).
                    if (Settings.Instance.PreventSleepWhilePlaying)
                    {
#if DEBUG
                        var rv =
#endif
                Win32API.SetThreadExecutionState(Win32API.ExecutionStateFlags.ES_SYSTEM_REQUIRED);
#if DEBUG
                        Debug.WriteLine("** SetThreadExecutionState returned: " + rv);
#endif
                    }

                    this.isUpdateToast = isUpdate;

                    if (!this.IsToastVisible)
                    {
                        this.ShowToast(true);

                        DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
                        anim.Completed += (s, e) =>
                        {
                            this.IsToastVisible = true;
                            this.FadeOut();
                        };
                        this.BeginAnimation(OpacityProperty, anim);
                    }
                    else if (this.minimizeTimer != null)
                    {
                        // Reset the timer's Interval so that the toast does not fade out while pressing the hotkeys.
                        this.BeginAnimation(OpacityProperty, null);
                        this.Opacity = 1.0;
                        this.minimizeTimer.Interval = Settings.Instance.FadeOutTime;
                        this.minimizeTimer.Start();
                    }
                }));
        }

        private void FadeOut(bool now = false)
        {
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    // 16 == one frame (0 is not a valid interval)
                    var interval = now ? 16 : Settings.Instance.FadeOutTime;

                    if (this.minimizeTimer == null)
                    {
                        this.minimizeTimer = new Timer { AutoReset = false };
                        this.minimizeTimer.Elapsed += (s, e) =>
                        {
                            try
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
                                    anim.Completed += (ss, ee) =>
                                    {
                                        this.ShowToast(false);
                                        this.IsToastVisible = false;
                                    };
                                    this.BeginAnimation(OpacityProperty, anim);
                                });
                            }
                            catch (TaskCanceledException) { }
                        };
                    }

                    this.minimizeTimer.Interval = interval;

                    this.minimizeTimer.Stop();
                    this.minimizeTimer.Start();
                }));
        }

        private void ResetPositionIfOffScreen()
        {
            var rect = new Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);

            if (!Screen.AllScreens.Any(s => s.WorkingArea.Contains(rect)))
            {
                // Get the defaults, but don't save them (this allows the user to reconnect their screen and get their desired settings back)
                var position = ScreenHelper.GetDefaultToastPosition(this.Width, this.Height);

                this.Left = position.X;
                this.Top = position.Y;
            }
        }

        public void DisplayAction(SpotifyAction action)
        {
            if (!Spotify.Instance.IsRunning && action != SpotifyAction.SettingsSaved)
            {
                this.toastIconURI = DEFAULT_ICON;
                this.UpdateToastText("Spotify not available!");
                return;
            }

            switch (action)
            {
                case SpotifyAction.PlayPause:
                case SpotifyAction.NextTrack:
                case SpotifyAction.PreviousTrack:
                case SpotifyAction.ShowSpotify:
                    break;

                case SpotifyAction.SettingsSaved:
                    this.toastIconURI = DEFAULT_ICON;
                    this.UpdateToastText("Settings saved", "Here is a preview of your settings!");
                    break;

                case SpotifyAction.VolumeUp:
                    this.UpdateToastText(this.currentSong, "Volume ++");
                    break;

                case SpotifyAction.VolumeDown:
                    this.UpdateToastText(this.currentSong, "Volume --");
                    break;

                case SpotifyAction.Mute:
                    this.UpdateToastText(this.currentSong, "Mute On/Off");
                    break;

                case SpotifyAction.ShowToast:
                    if (this.currentSong == null || !this.currentSong.IsValid())
                    {
                        this.toastIconURI = DEFAULT_ICON;
                        this.UpdateToastText("Nothing's playing", string.Empty, false);
                    }
                    else if (this.currentSong.IsValid())
                    {
                        this.toastIconURI = this.currentSong.CoverArtUrl;
                        this.UpdateToastText(this.currentSong, null, false);
                    }
                    if (!this.IsVisible)
                        this.FadeIn(true);
                    else
                        this.FadeOut(true);
                    break;

                case SpotifyAction.ThumbsUp:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_up.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Up!");
                    break;

                case SpotifyAction.ThumbsDown:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_down.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Down!");
                    break;
            }
        }

        private void UpdateSongProgressBar(double trackTime)
        {
            double timePercentage = trackTime / this.currentSong?.Length ?? trackTime;
            this.toastViewModel.SongProgressBarWidth = this.SongProgressBarContainer.ActualWidth * timePercentage;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Close Spotify first.
            if (Settings.Instance.CloseSpotifyWithToastify)
                Spotify.Instance.Kill();

            // Dispose the timer.
            this.minimizeTimer?.Stop();
            this.minimizeTimer?.Dispose();

            // Ensure trayicon is removed on exit. (Thx Linus)
            this.trayIcon.Visible = false;
            this.trayIcon.Dispose();
            this.trayIcon = null;

            this.ToastClosing?.Invoke(this, EventArgs.Empty);
            this.Plugins.Clear();

            base.OnClosing(e);
        }

        #region HotkeyActionCallback

        private static Hotkey _lastHotkey;
        private static DateTime _lastHotkeyPressTime = DateTime.Now;

        /// <summary>
        /// If the same hotkey press happens within this buffer time, it will be ignored.
        ///
        /// I came to 150 by pressing keys as quickly as possibly. The minimum time was less than 150
        /// but most values fell in the 150 to 200 range for quick presses, so 150 seemed the most reasonable
        /// </summary>
        private const int WAIT_BETWEEN_HOTKEY_PRESS = 150;

        internal static void HotkeyActionCallback(Hotkey hotkey)
        {
            // Bug 9421: ignore this keypress if it is the same as the previous one and it's been less than
            //           WAIT_BETWEEN_HOTKEY_PRESS since the last press. Note that we do not update
            //           _lastHotkeyPressTime in this case to avoid being trapped in a never ending cycle of
            //           ignoring keypresses if the user (for some reason) decides to press really quickly,
            //           really often on the hotkey
            if (hotkey == _lastHotkey && DateTime.Now.Subtract(_lastHotkeyPressTime).TotalMilliseconds < WAIT_BETWEEN_HOTKEY_PRESS)
                return;

            _lastHotkey = hotkey;
            _lastHotkeyPressTime = DateTime.Now;

            Debug.WriteLine($"HotkeyActionCallback: {hotkey.Action}");

            try
            {
                if (hotkey.Action == SpotifyAction.CopyTrackInfo && Current.currentSong != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.CopyTrackInfo);
                    Clipboard.SetText(Current.currentSong.GetClipboardText(Settings.Instance.ClipboardTemplate));
                }
                else if (hotkey.Action == SpotifyAction.PasteTrackInfo && Current.currentSong != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.PasteTrackInfo);
                    Clipboard.SetText(Current.currentSong.GetClipboardText(Settings.Instance.ClipboardTemplate));
                    Win32API.SendPasteKey();
                }
                else
                    Spotify.Instance.SendAction(hotkey.Action);

                Current.DisplayAction(hotkey.Action);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                Debug.WriteLine("Exception with hooked key! " + ex);
                Current.UpdateToastText("Unable to communicate with Spotify");
            }
        }

        #endregion HotkeyActionCallback

        #region Event handlers [xaml]

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();

            // Remove from ALT+TAB.
            Win32API.AddToolWindowStyle(this.GetHandle());
        }

        /// <summary>
        /// Mouse is over the window, halt any fade out animations and keep
        /// the toast active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BeginAnimation(OpacityProperty, null);
            this.Opacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.FadeOut();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                this.dragging = true;
                this.DragMove();
                return;
            }

            this.FadeOut(true);

            if (this.isUpdateToast)
                Process.Start(new ProcessStartInfo(VersionChecker.UpdateUrl));
            else
                Spotify.Instance.SendAction(SpotifyAction.ShowSpotify);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;

                // Save the new window position
                Settings settings = Settings.Instance;

                settings.PositionLeft = this.Left;
                settings.PositionTop = this.Top;

                settings.Save();
            }
        }

        #endregion Event handlers [xaml]

        #region Event handlers [Spotify]

        /// <summary>
        /// This event is received only once, at the start of the application.
        /// Here, we set up the initial state of the toast based on the current state of Spotify.
        /// </summary>
        /// <param name="sender"> <see cref="Spotify"/>. </param>
        /// <param name="e"></param>
        private void Spotify_Connected(object sender, SpotifyStateEventArgs e)
        {
            if (e.CurrentSong == null || !e.CurrentSong.IsValid())
                return;

            this.paused = !e.Playing;
            this.UpdateCurrentSong(e.CurrentSong);
        }

        private void Spotify_SongChanged(object sender, SpotifyTrackChangedEventArgs e)
        {
            if (e.NewSong == null || !e.NewSong.IsValid())
                return;

            this.paused = !e.Playing;
            this.UpdateCurrentSong(e.NewSong);
        }

        private void Spotify_PlayStateChanged(object sender, SpotifyPlayStateChangedEventArgs e)
        {
            this.paused = !e.Playing;

            // If toast's and Spotify's current songs do not match, then fix toast's current song.
            if (!Song.Equal(this.currentSong, e.CurrentSong))
                this.UpdateCurrentSong(e.CurrentSong);
            else
            {
                // Only fade-in if the play state change was triggered by a hotkey.
                bool fadeIn = _lastHotkey?.Action == SpotifyAction.PlayPause;
                this.UpdateToastText(this.currentSong, null, fadeIn);
            }
        }

        private void Spotify_TrackTimeChanged(object sender, SpotifyTrackTimeChangedEventArgs e)
        {
            this.UpdateSongProgressBar(e.TrackTime);
        }

        #endregion Event handlers [Spotify]

        #region Event handlers

        private void Application_Shutdown(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => Application.Current.Shutdown()));
            // this.Close();
        }

        private void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void TrayIcon_MouseClick(object s, System.Windows.Forms.MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Left)
                this.DisplayAction(SpotifyAction.ShowToast);
        }

        private void TrayIcon_DoubleClick(object s, EventArgs ev)
        {
            SettingsView.Launch(this);
        }

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (!e.New)
                return;

            // This is a background thread, so sleep it a bit so that it doesn't clash with the startup toast.
            Thread.Sleep(20000);

            this.toastIconURI = UPDATE_LOGO_ICON;
            this.UpdateToastText("Update Toastify!", $"Version {e.Version} available now.", true, true, true);
        }

        #endregion Event handlers
    }
}
