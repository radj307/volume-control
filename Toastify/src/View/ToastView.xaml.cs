using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Toastify.Common;
using Toastify.Core;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Model;
using Toastify.Services;
using Toastify.ViewModel;
using ToastifyAPI.Plugins;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;

namespace Toastify.View
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class ToastView : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ToastView));

        private const string DEFAULT_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyLogo.png";
        private const string AD_PLAYING_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyAdPlaying.png";
        private const string ALBUM_ACCESS_DENIED_ICON = "pack://application:,,,/Toastify;component/Resources/ToastifyAccessDenied.png";
        private const string UPDATE_LOGO_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyUpdateLogo.png";

        private const string STATE_PAUSED = "Paused";
        private const string STATE_NOTHING_PLAYING = "Nothing's playing";

        internal static ToastView Current { get; private set; }

        #region Private fields

        private IntPtr _windowHandle;

        private readonly ToastViewModel toastViewModel;

        private Timer minimizeTimer;

        private SystemTray trayIcon;

        private Song currentSong;
        private BitmapImage cover;
        private string toastIconURI = "";

        private bool isUpdateToast;
        private bool isPreviewForSettings;

        private bool dragging;
        private bool paused;

        #endregion Private fields

        private IntPtr WindowHandle
        {
            get
            {
                if (this._windowHandle == IntPtr.Zero)
                    this._windowHandle = this.GetHandle();
                return this._windowHandle;
            }
        }

        public Point Position { get { return this.Rect.Location; } }

        public Rect Rect { get { return new Rect(this.Left, this.Top, this.Width, this.Height); } }

        public bool IsInitComplete { get; private set; }

        public Settings Settings
        {
            get
            {
                return this.toastViewModel?.Settings;
            }
            private set
            {
                if (this.toastViewModel != null)
                    this.toastViewModel.Settings = value;
            }
        }

        public bool ShownOrFading { get; private set; }

        internal List<IPluginBase> Plugins { get; set; }

        #region Events

        public event EventHandler Started;

        public event EventHandler ToastClosing;

        #endregion Events

        public ToastView()
        {
            this.InitializeComponent();

            this.toastViewModel = new ToastViewModel();
            this.toastViewModel.PropertyChanged += this.ToastViewModel_PropertyChanged;
            this.DataContext = this.toastViewModel;

            // Set a static reference back to ourselves, useful for callbacks.
            Current = this;
        }

        #region Initialization

        private void Init()
        {
            this.InitToast();
            this.InitTrayIcon();
            this.StartSpotify();
        }

        private void FinalizeInit()
        {
            // Subscribe to Spotify's events (i.e. SpotifyLocalAPI's).
            Spotify.Instance.Exited += this.Application_Shutdown;
            Spotify.Instance.SongChanged += this.Spotify_SongChanged;
            Spotify.Instance.PlayStateChanged += this.Spotify_PlayStateChanged;
            Spotify.Instance.TrackTimeChanged += this.Spotify_TrackTimeChanged;

            // Subscribe to SettingsView's events
            SettingsView.SettingsLaunched += this.SettingsView_Launched;
            SettingsView.SettingsClosed += this.SettingsView_Closed;

            this.Deactivated += this.Toast_Deactivated;

            this.LoadPlugins();
            this.Started?.Invoke(this, EventArgs.Empty);

            this.InitVersionChecker();

            this.IsInitComplete = true;
        }

        public void InitToast()
        {
            if (!this.isPreviewForSettings)
                this.SetToastVisibility(false);

            // [ POSITION & DIMENSIONS ]
            this.Left = this.Settings.PositionLeft;
            this.Top = this.Settings.PositionTop;
            this.Width = this.Settings.ToastWidth;
            this.Height = this.Settings.ToastHeight;

            // [ BORDERS ]
            this.ToastBorder.BorderThickness = new Thickness(this.Settings.ToastBorderThickness);
            this.ToastBorder.BorderBrush = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastBorderColor));
            this.ToastBorder.CornerRadius = new CornerRadius(
                this.Settings.ToastBorderCornerRadiusTopLeft,
                this.Settings.ToastBorderCornerRadiusTopRight,
                this.Settings.ToastBorderCornerRadiusBottomRight,
                this.Settings.ToastBorderCornerRadiusBottomLeft);

            // [ BACKGROUND COLOR ]
            Color top = ColorHelper.HexToColor(this.Settings.ToastColorTop);
            Color bottom = ColorHelper.HexToColor(this.Settings.ToastColorBottom);

            GradientStopCollection stops = new GradientStopCollection(2)
            {
                new GradientStop(top, this.Settings.ToastColorTopOffset),
                new GradientStop(bottom, this.Settings.ToastColorBottomOffset)
            };
            this.ToastBorder.Background = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));

            // [ TEXT COLOR ]
            this.Title1.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle1Color));
            this.Title2.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle2Color));
            this.Title1.Effect = new DropShadowEffect
            {
                ShadowDepth = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowDepth : 0.0,
                BlurRadius = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowBlur : 0.0
            };
            this.Title2.Effect = new DropShadowEffect
            {
                ShadowDepth = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowDepth : 0.0,
                BlurRadius = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowBlur : 0.0
            };

            // [ TEXT FONT SIZE ]
            this.Title1.FontSize = this.Settings.ToastTitle1FontSize;
            this.Title2.FontSize = this.Settings.ToastTitle2FontSize;

            // [ SONG PROGRESS BAR ]
            this.SongProgressBar.Visibility = this.Settings.ShowSongProgressBar ? Visibility.Visible : Visibility.Hidden;
            this.SongProgressBarContainer.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarBackgroundColor));
            this.SongProgressBarLine.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
            this.SongProgressBarLineEllipse.Fill = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
        }

        private void InitTrayIcon()
        {
            this.trayIcon = new SystemTray(@"Toastify – Waiting for Spotify...", true)
            {
                AnimationStepMilliseconds = 75,
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            object[] animationIcons = {
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_1,
                Properties.Resources.toastify_loading_spotify_2,
                Properties.Resources.toastify_loading_spotify_3,
                Properties.Resources.toastify_loading_spotify_4,
                Properties.Resources.toastify_loading_spotify_5,
                Properties.Resources.toastify_loading_spotify_6,
                Properties.Resources.toastify_loading_spotify_7,
                Properties.Resources.toastify_loading_spotify_8,
                Properties.Resources.toastify_loading_spotify_9,
                Properties.Resources.toastify_loading_spotify_10,
                Properties.Resources.toastify_loading_spotify_11,
                Properties.Resources.toastify_loading_spotify_12,
                Properties.Resources.toastify_loading_spotify_13,
                Properties.Resources.toastify_loading_spotify_14,
                Properties.Resources.toastify_loading_spotify_15,
                Properties.Resources.toastify_loading_spotify_16,
                Properties.Resources.toastify_loading_spotify_17,
                Properties.Resources.toastify_loading_spotify_18,
                Properties.Resources.toastify_loading_spotify_19,
                Properties.Resources.toastify_loading_spotify_20,
                Properties.Resources.toastify_loading_spotify_21,
                Properties.Resources.toastify_loading_spotify_22,
                Properties.Resources.toastify_loading_spotify_23,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0,
                Properties.Resources.toastify_loading_spotify_0
            };
            this.trayIcon.SetIconRange(animationIcons);
            this.trayIcon.StartAnimation();

            // Init tray icon menu
            MenuItem menuSettings = new MenuItem { Text = @"Settings", Enabled = false };
            menuSettings.Click += (s, ev) => { SettingsView.Launch(this); };

            MenuItem menuAbout = new MenuItem { Text = @"About Toastify" };
            menuAbout.Click += (s, ev) => { new AboutView().ShowDialog(); };

            MenuItem menuExit = new MenuItem { Text = @"Exit" };
            menuExit.Click += this.Application_Shutdown;

            this.trayIcon.ContextMenu.MenuItems.Add(menuSettings);
            this.trayIcon.ContextMenu.MenuItems.Add(menuAbout);
            this.trayIcon.ContextMenu.MenuItems.Add("-");
            this.trayIcon.ContextMenu.MenuItems.Add(menuExit);
        }

        private void FinalizeTrayIconInitialization()
        {
            this.trayIcon.StopAnimation();
            this.trayIcon.Animate = false;
            this.trayIcon.Text = @"Toastify";
            this.trayIcon.Icon = Properties.Resources.ToastifyIcon;
            this.trayIcon.ContextMenu.MenuItems[0].Enabled = true;
            this.trayIcon.DoubleClick += this.TrayIcon_DoubleClick;
        }

        private void StartSpotify()
        {
            Spotify.Instance.Connected -= this.Spotify_Connected;
            Spotify.Instance.Connected += this.Spotify_Connected;
            Spotify.Instance.StartSpotify();
        }

        private void LoadPlugins()
        {
            this.Plugins = new List<IPluginBase>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string applicationPath = new FileInfo(assembly.Location).DirectoryName;

            foreach (var p in this.Settings.Plugins)
            {
                try
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
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
                catch (Exception e)
                {
                    // TODO: Handle plugins' errors.
                    logger.Error("Error while loading plugins.", e);
                    Analytics.TrackException(e);
                }
                Console.WriteLine(@"Loaded " + p.TypeName);
            }
        }

        /// <summary>
        /// Initialize VersionChecker. Updates are only checked once.
        /// </summary>
        private void InitVersionChecker()
        {
            VersionChecker.Instance.CheckVersionComplete -= this.VersionChecker_CheckVersionComplete;
            VersionChecker.Instance.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;
            VersionChecker.Instance.BeginCheckVersion();
        }

        #endregion Initialization

        #region Toast update

        /// <summary>
        /// Update current song's cover art url and toast's text.
        /// Also, save track info to file, if settings say so.
        /// </summary>
        /// <param name="song"> The song to set as current. </param>
        private void ChangeCurrentSong(Song song)
        {
            this.currentSong = song;

            if (this.currentSong.IsOtherTrackType())
                this.FetchOtherTrackInfo();

            this.UpdateCoverArt();
            this.UpdateToastText(this.currentSong);
            this.UpdateSongProgressBar(0.0);

            // Save track info to file.
            if (this.Settings.SaveTrackToFile)
            {
                if (!string.IsNullOrEmpty(this.Settings.SaveTrackToFilePath))
                {
                    try
                    {
                        string trackText = this.currentSong.GetClipboardText(this.Settings.ClipboardTemplate);
                        File.WriteAllText(this.Settings.SaveTrackToFilePath, trackText);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error while saving track to file.", e);
                    }
                }
            }
        }

        private void UpdateCoverArt(bool forceUpdate = false)
        {
            // TODO: Cache cover arts

            // Update cover art URL.
            string previousURI = this.toastIconURI;
            if (this.currentSong == null)
                this.toastIconURI = DEFAULT_ICON;
            else
            {
                if (this.currentSong.IsAd())
                    this.currentSong.CoverArtUrl = AD_PLAYING_ICON;
                else if (string.IsNullOrWhiteSpace(this.currentSong?.CoverArtUrl))
                    this.currentSong.CoverArtUrl = DEFAULT_ICON;

                this.toastIconURI = this.currentSong.CoverArtUrl;
            }

            // Update the cover art only if the url has changed.
            if (!string.IsNullOrEmpty(this.toastIconURI) && (forceUpdate || this.toastIconURI != previousURI))
                this.UpdateCoverArt(this.toastIconURI);
        }

        private void UpdateCoverArt(string coverArtUri)
        {
            this.toastIconURI = coverArtUri;
            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    this.cover = new BitmapImage();
                    this.cover.BeginInit();
                    this.cover.CacheOption = BitmapCacheOption.OnLoad;
                    this.cover.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                    try
                    {
                        this.cover.UriSource = new Uri(coverArtUri, UriKind.RelativeOrAbsolute);
                    }
                    catch (UriFormatException e)
                    {
                        logger.Error($"UriFormatException with URI=[{coverArtUri}]", e);
                        this.cover.UriSource = new Uri(ALBUM_ACCESS_DENIED_ICON, UriKind.RelativeOrAbsolute);
                    }
                    finally
                    {
                        this.cover.EndInit();
                        this.AlbumArt.Source = this.cover;
                    }
                }));
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
        private void UpdateToastText(Song song, string altTitle1 = null, bool fadeIn = true)
        {
            if (altTitle1 != null)
                this.UpdateToastText(altTitle1, song?.ToString() ?? string.Empty, fadeIn);
            else if (this.paused)
                this.UpdateToastText(STATE_PAUSED, song?.ToString() ?? string.Empty, fadeIn);
            else
            {
                this.toastViewModel.TrackName = song?.Track ?? string.Empty;
                this.toastViewModel.ArtistName = song?.Artist ?? string.Empty;
                if (fadeIn)
                    this.ShowOrHideToast(keepUp: true);
            }
        }

        /// <summary>
        /// Update the toast's text using custom strings.
        /// </summary>
        /// <param name="title1"> First title. </param>
        /// <param name="title2">  Second title. </param>
        /// <param name="fadeIn"> Whether or not to start the toast fade-in animation. </param>
        /// <param name="force"> Whether or no0t to force the toast to show up. </param>
        private void UpdateToastText(string title1, string title2 = "", bool fadeIn = true, bool force = false)
        {
            this.toastViewModel.Title1 = title1;
            this.toastViewModel.Title2 = title2;
            if (fadeIn)
                this.ShowOrHideToast(force, keepUp: true);
        }

        private void FetchOtherTrackInfo()
        {
            // TODO: Podcast? Try to fetch track info using Spotify Web API? (Podcast specific APIs are not yet available)
        }

        #endregion Toast update

        /// <summary>
        /// Display a temporary content (title and image) on the toast.
        /// </summary>
        /// <param name="title1"> The string on the first line. </param>
        /// <param name="title2"> The text on the second line. </param>
        /// <param name="artUri"> The URI of the image to display. </param>
        /// <param name="displayTime"> The display time in milliseconds. </param>
        public void DisplayFlashContent(string title1, string title2, string artUri, double displayTime)
        {
            this.UpdateToastText(title1, title2, false);
            this.UpdateCoverArt(artUri);

            Timer timer = new Timer(displayTime) { AutoReset = false };
            timer.Elapsed += (sender, args) => this.ChangeCurrentSong(Spotify.Instance.CurrentSong);
            timer.Start();
        }

        private void SetToastVisibility(bool shallBeVisible)
        {
            this.ShownOrFading = shallBeVisible;
            this.Topmost = shallBeVisible;
            this.Visibility = shallBeVisible ? Visibility.Visible : Visibility.Collapsed;
            Win32API.ShowWindow(this.WindowHandle, shallBeVisible ? Win32API.ShowWindowCmd.SW_RESTORE : Win32API.ShowWindowCmd.SW_SHOWMINNOACTIVE);

            if (shallBeVisible)
            {
                this.Left = this.Settings.PositionLeft;
                this.Top = this.Settings.PositionTop;
                this.ResetPositionIfOffScreen();
            }
        }

        private void ShowToastPreview(bool show)
        {
            if (show)
                this.minimizeTimer?.Stop();

            this.isPreviewForSettings = !show;
            this.ShowOrHideToast(true, previewForSettings: true);
            this.isPreviewForSettings = show;

            if (!show)
                this.InitToast();
        }

        /// <summary>
        /// Toggles the show state of the toast.
        /// </summary>
        /// <param name="force"> Whether to force the toast to fade-in or fade-out. </param>
        /// <param name="keepUp"> Whether to reset the fade-out timer or not. </param>
        /// <param name="previewForSettings"></param>
        private void ShowOrHideToast(bool force = false, bool keepUp = false, bool previewForSettings = false)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.ShownOrFading && keepUp && this.minimizeTimer != null && !previewForSettings && !this.isPreviewForSettings)
                {
                    // Reset the timer's Interval so that the toast does not fade out while pressing the hotkeys.
                    this.BeginAnimation(OpacityProperty, null);
                    this.Opacity = 1.0;
                    this.minimizeTimer.Interval = this.Settings.FadeOutTime;
                    this.minimizeTimer.Start();
                }
                else if (this.ShownOrFading && !this.isPreviewForSettings)
                    this.FadeOut(force);
                else
                    this.FadeIn(force, previewForSettings);
            }, DispatcherPriority.Render);
        }

        private void FadeIn(bool force = false, bool permanent = false)
        {
            bool doNotFadeIn = this.ShownOrFading ||
                               this.dragging ||
                               this.isPreviewForSettings ||
                               (this.Settings.DisableToast || (this.Settings.OnlyShowToastOnHotkey && !force)) ||
                               (this.Settings.DisableToastWithFullscreenVideogames && Win32API.IsForegroundAppAFullscreenVideogame());
            if (doNotFadeIn)
                return;

            // Stop FadeOut
            this.minimizeTimer?.Stop();

            this.SetToastVisibility(true);

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) =>
            {
                if (!permanent)
                    this.FadeOut();
            };
            this.BeginAnimation(OpacityProperty, anim);
        }

        private void FadeOut(bool now = false)
        {
            if (!this.ShownOrFading || this.isPreviewForSettings)
                return;

            // 16 == one frame (0 is not a valid interval)
            var interval = now ? 16 : Math.Max(this.Settings.FadeOutTime, 1000);

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
                                this.SetToastVisibility(false);
                                this.isUpdateToast = false;
                            };
                            this.BeginAnimation(OpacityProperty, anim);
                        }, DispatcherPriority.Render);
                    }
                    catch (TaskCanceledException ex)
                    {
                        logger.Warn("FadeOut animation canceled.", ex);
                        this.SetToastVisibility(false);
                        this.isUpdateToast = false;
                    }
                };
            }

            this.minimizeTimer.Interval = interval;

            this.minimizeTimer.Stop();
            this.minimizeTimer.Start();
        }

        private void ResetPositionIfOffScreen()
        {
            // Bring the Toast inside the working area if it is off-screen
            Vector offsetVector = ScreenHelper.BringRectInsideWorkingArea(this.Rect);
            this.Left += offsetVector.X;
            this.Top += offsetVector.Y;
        }

        #region DisplayAction

#if DEBUG
        public bool LogShowToastAction { get; set; }
#endif

        public void DisplayAction(ToastifyAction action)
        {
            if (!Spotify.Instance.IsRunning && action != ToastifyAction.SettingsSaved)
            {
                this.toastIconURI = DEFAULT_ICON;
                this.UpdateToastText("Spotify not available!");
                return;
            }

            switch (action)
            {
                case ToastifyAction.SettingsSaved:
                    this.DisplayFlashContent("Settings Saved", string.Empty, DEFAULT_ICON, 1500.0);
                    break;

                case ToastifyAction.VolumeUp:
                case ToastifyAction.VolumeDown:
                    this.UpdateToastText(this.currentSong, $"Volume {(action == ToastifyAction.VolumeUp ? "++" : "--")}");
                    break;

                case ToastifyAction.Mute:
                    this.UpdateToastText(this.currentSong, "Mute On/Off");
                    break;

                case ToastifyAction.ShowToast:
#if DEBUG
                    if (this.LogShowToastAction)
                    {
                        Win32API.WindowPlacement wPlacement = new Win32API.WindowPlacement();
                        Win32API.GetWindowPlacement(this.WindowHandle, ref wPlacement);

                        string hWnd = $"hWnd[{this.WindowHandle}]";
                        string timer = $"timer[{(this.minimizeTimer == null ? "null" : $"{(this.minimizeTimer.Enabled ? '1' : '0')}, {this.minimizeTimer.Interval}")}]";
                        string song = $"track[{(this.currentSong == null ? "null" : $"{(this.currentSong.IsValid() ? '1' : '0')}")}]";
                        string state = $"state[{(this.isUpdateToast ? '1' : '0')}{(this.isPreviewForSettings ? '1' : '0')}{(this.dragging ? '1' : '0')}{(this.paused ? '1' : '0')}]";
                        string visibility = $"visibility[{(this.ShownOrFading ? '1' : '0')}{(this.IsVisible ? '1' : '0')}{this.Visibility.ToString().Substring(0, 1)}{(this.Topmost ? '1' : '0')}, {{{wPlacement}}}]";
                        string dispatcher = $"dispatcher[{(this.Dispatcher == null ? "null" : $"{(this.Dispatcher.HasShutdownStarted ? '1' : '0')}")}]";
                        string settings = $"settings[{(this.Settings.DisableToast ? '1' : '0')}{(this.Settings.OnlyShowToastOnHotkey ? '1' : '0')}, {this.Settings.FadeOutTime}]";
                        logger.Info($"{hWnd}, {timer}, {song}, {state}, {visibility}, {dispatcher}, {settings}\n  Stack Trace:\n{Environment.StackTrace}");
                    }
#endif
                    this.ShowOrHideToast(true);
                    break;

                case ToastifyAction.ThumbsUp:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_up.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Up!");
                    break;

                case ToastifyAction.ThumbsDown:
                    this.toastIconURI = "pack://application:,,,/Toastify;component/Resources/thumbs_down.png";
                    this.UpdateToastText(this.currentSong, "Thumbs Down!");
                    break;

                default:
                    // Ignore any other action
                    break;
            }
        }

        #endregion DisplayAction

        private void UpdateSongProgressBar(double trackTime)
        {
            double timePercentage = trackTime / this.currentSong?.Length ?? trackTime;
            this.toastViewModel.SongProgressBarWidth = this.SongProgressBarContainer.ActualWidth * timePercentage;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Close Spotify first.
            if (this.Settings.CloseSpotifyWithToastify && Spotify.Instance.IsRunning)
                Spotify.Instance.Kill();

            // Dispose the timer.
            this.minimizeTimer?.Stop();
            this.minimizeTimer?.Dispose();

            // Ensure trayicon is removed on exit.
            if (this.trayIcon != null)
            {
                this.trayIcon.Visible = false;
                this.trayIcon.Dispose();
                this.trayIcon = null;
            }

            this.ToastClosing?.Invoke(this, EventArgs.Empty);
            this.Plugins?.Clear();

            base.OnClosing(e);
        }

        #region Event handlers [xaml]

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();

            // Remove from ALT+TAB.
            Win32API.AddToolWindowStyle(this.WindowHandle);
        }

        /// <summary>
        /// Mouse is over the window, halt any fade out animations and keep the toast active.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BeginAnimation(OpacityProperty, null);
            this.Opacity = 1.0;
            if (this.minimizeTimer != null)
            {
                this.minimizeTimer.Stop();
                this.minimizeTimer = null;
            }
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
                Spotify.Instance.SendAction(ToastifyAction.ShowSpotify);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;

                // Save the new window position
                this.Settings.PositionLeft = this.Left;
                this.Settings.PositionTop = this.Top;
                if (this.Settings == Settings.Current)
                    this.Settings.Save();
            }
        }

        #endregion Event handlers [xaml]

        #region Event handlers [Spotify]

        /// <summary>
        /// This event is received only once, at the start of the application.
        /// Here, we finalize the initialization of Toastify and set up the initial state of the toast based on the current state of Spotify.
        /// </summary>
        /// <param name="sender"> <see cref="Spotify"/>. </param>
        /// <param name="e"></param>
        private void Spotify_Connected(object sender, SpotifyStateEventArgs e)
        {
            // Finalize initialization
            this.FinalizeTrayIconInitialization();
            this.FinalizeInit();

            // Update current song
            if (e.CurrentSong == null || !e.CurrentSong.IsValid())
            {
                this.toastIconURI = DEFAULT_ICON;
                this.UpdateToastText(STATE_NOTHING_PLAYING, string.Empty, false);
            }
            else
            {
                this.paused = !e.Playing;
                this.ChangeCurrentSong(e.CurrentSong);
                this.UpdateSongProgressBar(e.TrackTime);
            }
        }

        private void Spotify_SongChanged(object sender, SpotifyTrackChangedEventArgs e)
        {
            if (e.NewSong == null || !e.NewSong.IsValid())
                return;

            this.ChangeCurrentSong(e.NewSong);
        }

        private void Spotify_PlayStateChanged(object sender, SpotifyPlayStateChangedEventArgs e)
        {
            // Check if the toast is actually displaying something
            if (this.currentSong == null)
                this.ChangeCurrentSong(Spotify.Instance.CurrentSong);

            this.paused = !e.Playing;

            // Only fade-in if the play state change was triggered by a hotkey.
            bool fadeIn = Hotkey.LastHotkey?.Action == ToastifyAction.PlayPause;
            this.UpdateToastText(this.currentSong, fadeIn: fadeIn);
        }

        private void Spotify_TrackTimeChanged(object sender, SpotifyTrackTimeChangedEventArgs e)
        {
            this.UpdateSongProgressBar(e.TrackTime);
        }

        #endregion Event handlers [Spotify]

        #region Event handlers

        private void SettingsView_Launched(object sender, SettingsViewLaunchedEventArgs e)
        {
            this.Settings = e.Settings;
            this.ShowToastPreview(true);
        }

        private void SettingsView_Closed(object sender, EventArgs e)
        {
            this.Settings = Settings.Current;
            this.ShowToastPreview(false);
        }

        private void Application_Shutdown(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() => Application.Current.Shutdown()));
        }

        private void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
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
            this.isUpdateToast = true;
            this.UpdateToastText("Update Toastify!", $"Version {e.Version} available now.", force: true);

            VersionChecker.Instance.CheckVersionComplete -= this.VersionChecker_CheckVersionComplete;
        }

        private void ToastViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.Settings.ShowSongProgressBar):
                    this.SongProgressBar.Visibility = this.Settings.ShowSongProgressBar ? Visibility.Visible : Visibility.Hidden;
                    break;

                case nameof(this.Settings.ToastTitlesOrder):
                    this.toastViewModel.SetTrackName(this.toastViewModel.TrackName);
                    this.toastViewModel.SetArtistName(this.toastViewModel.ArtistName);
                    break;

                case nameof(this.Settings.ToastColorTop):
                case nameof(this.Settings.ToastColorBottom):
                case nameof(this.Settings.ToastColorTopOffset):
                case nameof(this.Settings.ToastColorBottomOffset):
                    Color top = ColorHelper.HexToColor(this.Settings.ToastColorTop);
                    Color bottom = ColorHelper.HexToColor(this.Settings.ToastColorBottom);

                    GradientStopCollection stops = new GradientStopCollection(2)
                    {
                        new GradientStop(top, this.Settings.ToastColorTopOffset),
                        new GradientStop(bottom, this.Settings.ToastColorBottomOffset)
                    };
                    this.ToastBorder.Background = new LinearGradientBrush(stops, new Point(0, 0), new Point(0, 1));
                    break;

                case nameof(this.Settings.ToastBorderColor):
                    this.ToastBorder.BorderBrush = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastBorderColor));
                    break;

                case nameof(this.Settings.ToastBorderThickness):
                    this.ToastBorder.BorderThickness = new Thickness(this.Settings.ToastBorderThickness);
                    break;

                case nameof(this.Settings.ToastBorderCornerRadiusTopLeft):
                case nameof(this.Settings.ToastBorderCornerRadiusTopRight):
                case nameof(this.Settings.ToastBorderCornerRadiusBottomLeft):
                case nameof(this.Settings.ToastBorderCornerRadiusBottomRight):
                    this.ToastBorder.CornerRadius = new CornerRadius(
                        this.Settings.ToastBorderCornerRadiusTopLeft,
                        this.Settings.ToastBorderCornerRadiusTopRight,
                        this.Settings.ToastBorderCornerRadiusBottomRight,
                        this.Settings.ToastBorderCornerRadiusBottomLeft);
                    break;

                case nameof(this.Settings.ToastWidth):
                case nameof(this.Settings.ToastHeight):
                case nameof(this.Settings.PositionLeft):
                case nameof(this.Settings.PositionTop):
                    this.Width = this.Settings.ToastWidth >= this.MinWidth ? this.Settings.ToastWidth : this.MinWidth;
                    this.Height = this.Settings.ToastHeight >= this.MinHeight ? this.Settings.ToastHeight : this.MinHeight;
                    this.Left = this.Settings.PositionLeft;
                    this.Top = this.Settings.PositionTop;
                    this.ResetPositionIfOffScreen();
                    break;

                #region Title1

                case nameof(this.Settings.ToastTitle1Color):
                    this.Title1.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle1Color));
                    break;

                case nameof(this.Settings.ToastTitle1FontSize):
                    this.Title1.FontSize = this.Settings.ToastTitle1FontSize;
                    break;

                case nameof(this.Settings.ToastTitle1DropShadow):
                    this.Title1.Effect = new DropShadowEffect
                    {
                        ShadowDepth = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowDepth : 0.0,
                        BlurRadius = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowBlur : 0.0
                    };
                    break;

                case nameof(this.Settings.ToastTitle1ShadowDepth):
                    if (this.Title1.Effect is DropShadowEffect t1_effect_sd)
                        t1_effect_sd.ShadowDepth = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowDepth : 0.0;
                    break;

                case nameof(this.Settings.ToastTitle1ShadowBlur):
                    if (this.Title1.Effect is DropShadowEffect t1_effect_sb)
                        t1_effect_sb.BlurRadius = this.Settings.ToastTitle1DropShadow ? this.Settings.ToastTitle1ShadowBlur : 0.0;
                    break;

                #endregion Title1

                #region Title2

                case nameof(this.Settings.ToastTitle2Color):
                    this.Title2.Foreground = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.ToastTitle2Color));
                    break;

                case nameof(this.Settings.ToastTitle2FontSize):
                    this.Title2.FontSize = this.Settings.ToastTitle2FontSize;
                    break;

                case nameof(this.Settings.ToastTitle2DropShadow):
                    this.Title2.Effect = new DropShadowEffect
                    {
                        ShadowDepth = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowDepth : 0.0,
                        BlurRadius = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowBlur : 0.0
                    };
                    break;

                case nameof(this.Settings.ToastTitle2ShadowDepth):
                    if (this.Title2.Effect is DropShadowEffect t2_effect_sd)
                        t2_effect_sd.ShadowDepth = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowDepth : 0.0;
                    break;

                case nameof(this.Settings.ToastTitle2ShadowBlur):
                    if (this.Title2.Effect is DropShadowEffect t2_effect_sb)
                        t2_effect_sb.BlurRadius = this.Settings.ToastTitle2DropShadow ? this.Settings.ToastTitle2ShadowBlur : 0.0;
                    break;

                #endregion Title2

                case nameof(this.Settings.SongProgressBarBackgroundColor):
                    this.SongProgressBarContainer.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarBackgroundColor));
                    break;

                case nameof(this.Settings.SongProgressBarForegroundColor):
                    this.SongProgressBarLine.Background = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
                    this.SongProgressBarLineEllipse.Fill = new SolidColorBrush(ColorHelper.HexToColor(this.Settings.SongProgressBarForegroundColor));
                    break;

                default:
                    // Ignore any other property
                    break;
            }
        }

        #endregion Event handlers
    }
}