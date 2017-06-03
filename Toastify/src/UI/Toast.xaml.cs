using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using Toastify.Plugin;
using Toastify.Services;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;

namespace Toastify.UI
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class Toast : Window
    {
        private const string DEFAULT_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyLogo.png";
        private const string AD_PLAYING_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyAdPlaying.png";
        private const string ALBUM_ACCESS_DENIED_ICON = "pack://application:,,,/Toastify;component/Resources/ToastifyAccessDenied.png";
        private const string UPDATE_LOGO_ICON = "pack://application:,,,/Toastify;component/Resources/SpotifyToastifyUpdateLogo.png";

        internal static Toast Current { get; private set; }

        #region Private fields

        private Timer minimizeTimer;

        private NotifyIcon trayIcon;

        private Song currentSong;
        private BitmapImage cover;
        private string toastIconURI = "";

        private VersionChecker versionChecker;
        private bool isUpdateToast;

        private bool dragging;

        #endregion Private fields

        internal List<IPluginBase> Plugins { get; set; }

        public new Visibility Visibility
        {
#pragma warning disable IDE0009 // Member access should be qualified.
            get { return base.Visibility; }
            set { base.Visibility = value; }
#pragma warning restore IDE0009 // Member access should be qualified.
        }

        #region Events

        public event EventHandler Started;

        public event EventHandler ToastClosing;

        #endregion Events

        public Toast()
        {
            this.InitializeComponent();

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
                SettingsXml.Instance.Load();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception loading settings:\n" + ex);

                MessageBox.Show(
                    $@"Toastify was unable to load the settings file.{Environment.NewLine
                        }Delete the Toastify.xml file and restart the application to recreate the settings file.{Environment.NewLine
                        }{Environment.NewLine
                        }The application will now be started with default settings.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                SettingsXml.Instance.Default(true);
            }

            string version = VersionChecker.Version;

            Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppLaunch, version);

            if (SettingsXml.Instance.PreviousOS != version)
            {
                Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppUpgraded, version);
                SettingsXml.Instance.PreviousOS = version;
            }
        }

        public void InitToast()
        {
            //If we find any invalid settings in the xml we skip it and use default.
            //User notification of bad settings will be implemented with the settings dialog.

            // TODO: Refactor InitToast method.
            //This method is UGLY but we'll keep it until the settings dialog is implemented.
            SettingsXml settings = SettingsXml.Instance;

            double minWidth = this.MinWidth > 0.0 ? this.MinWidth : 200.0;
            double minHeight = this.MinHeight > 0.0 ? this.MinHeight : 65.0;

            this.Width = settings.ToastWidth >= minWidth ? settings.ToastWidth : minWidth;
            this.Height = settings.ToastHeight >= minHeight ? settings.ToastHeight : minHeight;
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
            menuSettings.Click += (s, ev) => { Settings.Launch(this); };

            MenuItem menuAbout = new MenuItem { Text = @"About Toastify..." };
            menuAbout.Click += (s, ev) => { new About().ShowDialog(); };

            MenuItem menuExit = new MenuItem { Text = @"Exit" };
            menuExit.Click += (s, ev) => { Application.Current.Shutdown(); }; //this.Close(); };

            this.trayIcon.ContextMenu.MenuItems.Add(menuSettings);
            this.trayIcon.ContextMenu.MenuItems.Add(menuAbout);
            this.trayIcon.ContextMenu.MenuItems.Add("-");
            this.trayIcon.ContextMenu.MenuItems.Add(menuExit);

            this.trayIcon.MouseClick += (s, ev) => { if (ev.Button == MouseButtons.Left) this.DisplayAction(SpotifyAction.ShowToast, null); };
            this.trayIcon.DoubleClick += (s, ev) => { Settings.Launch(this); };
        }

        private void StartSpotifyOrAskUser()
        {
            try
            {
                Spotify.Instance.StartSpotify();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unknown error occurred when trying to start Spotify.\nPlease start Spotify manually.\n\nTechnical Details: " + e.Message, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadPlugins()
        {
            this.Plugins = new List<IPluginBase>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly.Location != null)
            {
                string applicationPath = new FileInfo(assembly.Location).DirectoryName;

                foreach (var p in SettingsXml.Instance.Plugins)
                {
                    try
                    {
                        if (applicationPath != null)
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
                        else
                            Debug.WriteLine("'applicationPath' is null");
                    }
                    catch (Exception)
                    {
                        // TODO: Handle plugins' errors.
                    }
                    Console.WriteLine(@"Loaded " + p.TypeName);
                }
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

        private void UpdateCurrentSong(Song song)
        {
            this.currentSong = song;

            this.UpdateCoverArtUrl();

            this.Dispatcher.Invoke(() => { this.Title1.Text = this.currentSong.Track; this.Title2.Text = this.currentSong.Artist; }, DispatcherPriority.Normal);
            this.Dispatcher.Invoke(() => { this.FadeIn(); }, DispatcherPriority.Normal);

            // Save track info to file.
            if (SettingsXml.Instance.SaveTrackToFile)
            {
                if (!string.IsNullOrEmpty(SettingsXml.Instance.SaveTrackToFilePath))
                {
                    try
                    {
                        string trackText = this.currentSong.GetClipboardText(SettingsXml.Instance.ClipboardTemplate);
                        File.WriteAllText(SettingsXml.Instance.SaveTrackToFilePath, trackText);
                    }
                    catch
                    {
                        // Ignore errors writing out the album.
                    }
                }
            }
        }

        private void UpdateCoverArtUrl()
        {
            try
            {
                Spotify.Instance.SetCoverArt(this.currentSong);
            }
            catch
            {
                // Exceptions will be handled (for telemetry etc.) within SetCoverArt, but they will be rethrown
                // so that we can set custom artwork here
                this.currentSong.CoverArtUrl = ALBUM_ACCESS_DENIED_ICON;
            }

            if (string.IsNullOrWhiteSpace(this.currentSong.Track))
            {
                this.currentSong.CoverArtUrl = AD_PLAYING_ICON;
                this.currentSong.Track = "Spotify Ad";
            }
            else if (string.IsNullOrWhiteSpace(this.currentSong.CoverArtUrl))
                this.currentSong.CoverArtUrl = DEFAULT_ICON;

            this.toastIconURI = this.currentSong.CoverArtUrl;
        }

        private void FadeIn(bool force = false, bool isUpdate = false)
        {
            this.minimizeTimer?.Stop();

            if (this.dragging)
                return;

            SettingsXml settings = SettingsXml.Instance;

            // this is a convenient place to reset the idle timer (if so asked)
            // as this will be triggered when a song is played. The primary problem is if there is a
            // particularly long song then this will not work. That said, this is the safest (in terms of
            // not causing a user's computer from never sleeping).
            if (settings.PreventSleepWhilePlaying)
            {
#if DEBUG
                var rv =
#endif
                    Win32API.SetThreadExecutionState(Win32API.ExecutionState.EsSystemRequired);
#if DEBUG
                Debug.WriteLine("** SetThreadExecutionState returned: " + rv);
#endif
            }

            if ((settings.DisableToast || settings.OnlyShowToastOnHotkey) && !force)
                return;

            this.isUpdateToast = isUpdate;

            if (!string.IsNullOrEmpty(this.toastIconURI))
            {
                this.cover = new BitmapImage();
                this.cover.BeginInit();
                this.cover.UriSource = new Uri(this.toastIconURI, UriKind.RelativeOrAbsolute);
                this.cover.EndInit();
                this.LogoToast.Source = this.cover;
            }

            this.WindowState = WindowState.Normal;

            Rectangle workingArea = new Rectangle((int)this.Left, (int)this.Height, (int)this.ActualWidth, (int)this.ActualHeight);
            workingArea = Screen.GetWorkingArea(workingArea);

            this.Left = settings.PositionLeft;
            this.Top = settings.PositionTop;

            this.ResetPositionIfOffScreen(workingArea);

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) => { this.FadeOut(); };
            this.BeginAnimation(OpacityProperty, anim);

            this.Topmost = true;
        }

        private void FadeOut(bool now = false)
        {
            // 16 == one frame (0 is not a valid interval)
            var interval = now ? 16 : SettingsXml.Instance.FadeOutTime;

            DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500))
            {
                BeginTime = TimeSpan.FromMilliseconds(interval)
            };
            this.BeginAnimation(OpacityProperty, anim);

            if (this.minimizeTimer == null)
            {
                this.minimizeTimer = new Timer { AutoReset = false };

                this.minimizeTimer.Elapsed += (s, ev) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.WindowState = WindowState.Minimized;
                        Debug.WriteLine("Minimized");
                    });
                };
            }

            // extra buffer to avoid graphics corruption at the tail end of the fade
            this.minimizeTimer.Interval = interval * 2;

            this.minimizeTimer.Stop();
            this.minimizeTimer.Start();
        }

        private void ResetPositionIfOffScreen(Rectangle workingArea)
        {
            var rect = new Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);

            if (!Screen.AllScreens.Any(s => s.WorkingArea.Contains(rect)))
            {
                // get the defaults, but don't save them (this allows the user to reconnect their screen and get their
                // desired settings back)
                var position = ScreenHelper.GetDefaultToastPosition(this.Width, this.Height);

                this.Left = position.X;
                this.Top = position.Y;
            }
        }

        public void DisplayAction(SpotifyAction action, Song trackBeforeAction)
        {
            //Anything that changes track doesn't need to be handled since
            //that will be handled in the timer event.

            const string volumeUpText = "Volume ++";
            const string volumeDownText = "Volume --";
            const string muteOnOffText = "Mute On/Off";
            const string nothingsPlaying = "Nothing's playing";
            const string pausedText = "Paused";
            const string stoppedText = "Stopped";
            const string settingsText = "Settings saved";

            if (!Spotify.Instance.IsRunning && action != SpotifyAction.SettingsSaved)
            {
                this.toastIconURI = DEFAULT_ICON;
                this.Title1.Text = "Spotify not available!";
                this.Title2.Text = string.Empty;
                this.FadeIn();
                return;
            }

            Song currentTrack = trackBeforeAction;

            string prevTitle1 = this.Title1.Text;
            string prevTitle2 = this.Title2.Text;

            switch (action)
            {
                case SpotifyAction.PlayPause:
                    if (trackBeforeAction != null)
                    {
                        //We pressed pause
                        this.Title1.Text = "Paused";
                        this.Title2.Text = trackBeforeAction.ToString();
                        this.FadeIn();
                    }
                    this.currentSong = null;  //If we presses play this will force a toast to display in next timer event.
                    break;

                case SpotifyAction.Stop:
                    this.currentSong = null;
                    this.Title1.Text = "Stopped";
                    this.Title2.Text = trackBeforeAction.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.SettingsSaved:
                    this.Title1.Text = settingsText;
                    this.Title2.Text = "Here is a preview of your settings!";
                    this.FadeIn();
                    break;

                case SpotifyAction.NextTrack:      //No need to handle
                    break;

                case SpotifyAction.PreviousTrack:  //No need to handle
                    break;

                case SpotifyAction.VolumeUp:
                    this.Title1.Text = volumeUpText;
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.VolumeDown:
                    this.Title1.Text = volumeDownText;
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.Mute:
                    this.Title1.Text = muteOnOffText;
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.ShowToast:
                    if (currentTrack == null || !currentTrack.IsValid())
                    {
                        this.toastIconURI = DEFAULT_ICON;

                        this.Title1.Text = nothingsPlaying;
                        this.Title2.Text = string.Empty;
                    }
                    else
                    {
                        if (currentTrack.IsValid())
                        {
                            this.toastIconURI = currentTrack.CoverArtUrl;

                            this.Title1.Text = currentTrack.Artist;
                            this.Title2.Text = currentTrack.Track;
                        }
                    }

                    this.FadeIn(true);
                    break;

                case SpotifyAction.ShowSpotify:  //No need to handle
                    break;

                case SpotifyAction.ThumbsUp:
                    this.toastIconURI = "Resources/thumbs_up.png";

                    this.Title1.Text = "Thumbs Up!";
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.ThumbsDown:
                    this.toastIconURI = "Resources/thumbs_down.png";

                    this.Title1.Text = "Thumbs Down :(";
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // close Spotify first
            if (SettingsXml.Instance.CloseSpotifyWithToastify)
                Spotify.Instance.Kill();

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

            try
            {
                Song songBeforeAction = Current.currentSong;

                if (hotkey.Action == SpotifyAction.CopyTrackInfo && songBeforeAction != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.CopyTrackInfo);
                    Clipboard.SetText(songBeforeAction.GetClipboardText(SettingsXml.Instance.ClipboardTemplate));
                }
                else if (hotkey.Action == SpotifyAction.PasteTrackInfo && songBeforeAction != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.PasteTrackInfo);
                    Clipboard.SetText(songBeforeAction.GetClipboardText(SettingsXml.Instance.ClipboardTemplate));
                    Win32API.SendPasteKey();
                }
                else
                    Spotify.Instance.SendAction(hotkey.Action);

                Current.DisplayAction(hotkey.Action, songBeforeAction);
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                Debug.WriteLine("Exception with hooked key! " + ex);
                Current.Title1.Text = "Unable to communicate with Spotify";
                Current.Title2.Text = "";
                Current.FadeIn();
            }
        }

        #endregion HotkeyActionCallback

        #region Event handlers [xaml]

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();

            // Remove from ALT+TAB.
            Win32API.AddToolWindowStyle(this);
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
            {
                Process.Start(new ProcessStartInfo(this.versionChecker.UpdateUrl));
            }
            else
            {
                Spotify.Instance.SendAction(SpotifyAction.ShowSpotify);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;

                // save the new window position
                SettingsXml settings = SettingsXml.Instance;

                settings.PositionLeft = this.Left;
                settings.PositionTop = this.Top;

                settings.Save();
            }
        }

        #endregion Event handlers [xaml]

        #region Event handlers [Spotify]

        private void Spotify_SongChanged(object sender, Core.SpotifyTrackChangedEventArgs e)
        {
            if (e.NewSong == null || !e.NewSong.IsValid())
                return;

            this.UpdateCurrentSong(e.NewSong);
        }

        private void Spotify_PlayStateChanged(object sender, SpotifyPlayStateChangedEventArgs e)
        {
            // TODO: PlayStateChanged
        }

        private void Spotify_TrackTimeChanged(object sender, SpotifyTrackTimeChangedEventArgs e)
        {
            // TODO: TrackTimeChanged
        }

        #endregion Event handlers [Spotify]

        #region Event handlers

        private void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (!e.New)
                return;

            const string title = "Update Toastify!";
            string caption = "Version " + e.Version + " available now.";

            // this is a background thread, so sleep it a bit so that it doesn't clash with the startup toast
            Thread.Sleep(20000);

            this.Dispatcher.Invoke(() =>
            {
                this.Title1.Text = title;
                this.Title2.Text = caption;

                this.toastIconURI = UPDATE_LOGO_ICON;

                this.FadeIn(true, true);
            }, DispatcherPriority.Normal);
        }

        #endregion Event handlers
    }
}