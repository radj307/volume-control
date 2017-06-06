using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
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
        private bool visible;
        private bool paused;

        #endregion Private fields

        internal List<IPluginBase> Plugins { get; set; }

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

            string version = VersionChecker.CurrentVersion;

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
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
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

        /// <summary>
        /// Update current song's cover art url and toast's text.
        /// Also, save track info to file, if settings say so.
        /// </summary>
        /// <param name="song"> The song to set as current. </param>
        private void UpdateCurrentSong(Song song)
        {
            this.currentSong = song;

            this.UpdateCoverArtUrl();
            this.UpdateToastText(this.currentSong);

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
            if (string.IsNullOrWhiteSpace(this.currentSong.Track))
            {
                this.currentSong.CoverArtUrl = AD_PLAYING_ICON;
                this.currentSong.Track = "Spotify Ad";
            }
            else if (string.IsNullOrWhiteSpace(this.currentSong.CoverArtUrl))
                this.currentSong.CoverArtUrl = DEFAULT_ICON;

            this.toastIconURI = this.currentSong.CoverArtUrl;
        }

        private void UpdateCoverArt()
        {
            if (!string.IsNullOrEmpty(this.toastIconURI))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        try
                        {
                            this.cover = new BitmapImage();
                            this.cover.BeginInit();
                            this.cover.CacheOption = BitmapCacheOption.OnLoad;
                            this.cover.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                            this.cover.UriSource = new Uri(this.toastIconURI, UriKind.RelativeOrAbsolute);
                            this.cover.EndInit();
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);

                            this.cover = new BitmapImage();
                            this.cover.BeginInit();
                            this.cover.CacheOption = BitmapCacheOption.OnLoad;
                            this.cover.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                            this.cover.UriSource = new Uri(ALBUM_ACCESS_DENIED_ICON, UriKind.RelativeOrAbsolute);
                            this.cover.EndInit();
                        }
                        finally
                        {
                            this.LogoToast.Source = this.cover;
                        }
                    }));
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
                title1 = this.paused ? "Paused" : song.Artist;
                title2 = this.paused ? song.ToString() : song.Track;
            }
            else
            {
                title1 = altTitle1;
                title2 = song.ToString();
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
            this.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    this.Title1.Text = title1;
                    this.Title2.Text = title2;
                    if (fadeIn)
                        this.FadeIn(force, isUpdate);
                }));
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

            if (settings.DisableToast || (settings.OnlyShowToastOnHotkey && !force && !this.visible))
                return;

            this.isUpdateToast = isUpdate;
            this.WindowState = WindowState.Normal;
            this.Topmost = true;

            this.UpdateCoverArt();
            this.Left = settings.PositionLeft;
            this.Top = settings.PositionTop;
            this.ResetPositionIfOffScreen();

            if (!this.visible)
            {
                DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
                anim.Completed += (s, e) =>
                {
                    this.visible = true;
                    this.FadeOut();
                };
                this.BeginAnimation(OpacityProperty, anim);
            }
            else if (this.minimizeTimer != null)
            {
                // Reset the timer's Interval so that the toast does not fade out while pressing the hotkeys.
                this.BeginAnimation(OpacityProperty, null);
                this.Opacity = 1.0;
                this.minimizeTimer.Interval = SettingsXml.Instance.FadeOutTime;
                this.minimizeTimer.Start();
            }
        }

        private void FadeOut(bool now = false)
        {
            // 16 == one frame (0 is not a valid interval)
            var interval = now ? 16 : SettingsXml.Instance.FadeOutTime;

            if (this.minimizeTimer == null)
            {
                this.minimizeTimer = new Timer { AutoReset = false };
                this.minimizeTimer.Elapsed += (s, e) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
                        anim.Completed += (ss, ee) =>
                        {
                            this.visible = false;
                            this.WindowState = WindowState.Minimized;
                            Debug.WriteLine("Minimized");
                        };
                        this.BeginAnimation(OpacityProperty, anim);
                    });
                };
            }

            this.minimizeTimer.Interval = interval;

            this.minimizeTimer.Stop();
            this.minimizeTimer.Start();
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

                // TODO: Is Stop really possible on Spotify?
                case SpotifyAction.Stop:
                    this.UpdateToastText(this.currentSong, "Stopped");
                    this.currentSong = null;
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
                    if (!this.visible)
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Close Spotify first.
            if (SettingsXml.Instance.CloseSpotifyWithToastify)
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

            try
            {
                if (hotkey.Action == SpotifyAction.CopyTrackInfo && Current.currentSong != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.CopyTrackInfo);
                    Clipboard.SetText(Current.currentSong.GetClipboardText(SettingsXml.Instance.ClipboardTemplate));
                }
                else if (hotkey.Action == SpotifyAction.PasteTrackInfo && Current.currentSong != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.PasteTrackInfo);
                    Clipboard.SetText(Current.currentSong.GetClipboardText(SettingsXml.Instance.ClipboardTemplate));
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
                Process.Start(new ProcessStartInfo(this.versionChecker.UpdateUrl));
            else
                Spotify.Instance.SendAction(SpotifyAction.ShowSpotify);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.dragging)
            {
                this.dragging = false;

                // Save the new window position
                SettingsXml settings = SettingsXml.Instance;

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

        private void Spotify_SongChanged(object sender, Core.SpotifyTrackChangedEventArgs e)
        {
            if (e.NewSong == null || !e.NewSong.IsValid())
                return;

            this.UpdateCurrentSong(e.NewSong);
        }

        private void Spotify_PlayStateChanged(object sender, SpotifyPlayStateChangedEventArgs e)
        {
            // Only fade-in if the play state change was triggered by a hotkey.
            bool fadeIn = _lastHotkey.Action == SpotifyAction.PlayPause;
            this.paused = !e.Playing;
            this.UpdateToastText(this.currentSong, null, fadeIn);
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

        private void TrayIcon_MouseClick(object s, System.Windows.Forms.MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Left)
                this.DisplayAction(SpotifyAction.ShowToast);
        }

        private void TrayIcon_DoubleClick(object s, EventArgs ev)
        {
            Settings.Launch(this);
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