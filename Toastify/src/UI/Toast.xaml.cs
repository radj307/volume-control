using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Toastify.Core;
using Toastify.Events;
using Toastify.Helpers;
using Toastify.Services;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Timer = System.Timers.Timer;

namespace Toastify.UI
{
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class Toast : Window
    {
        private const string DEFAULT_ICON = "SpotifyToastifyLogo.png";
        private const string AD_PLAYING_ICON = "SpotifyAdPlaying.png";
        private const string ALBUM_ACCESS_DENIED_ICON = "ToastifyAccessDenied.png";

        private Timer watchTimer;
        private Timer minimizeTimer;

        private NotifyIcon trayIcon;

        /// <summary>
        /// Holds the actual icon shown on the toast
        /// </summary>
        private string toastIcon = "";

        private BitmapImage cover;

        private VersionChecker versionChecker;
        private bool isUpdateToast;

        internal List<Plugin.PluginBase> Plugins { get; set; }

        internal static Toast Current { get; private set; }

        /// <summary>
        /// To the best of our knowledge this is our current playing song
        /// </summary>
        private Song currentSong;

        private bool dragging;

        public new Visibility Visibility
        {
#pragma warning disable IDE0009 // Member access should be qualified.
            get { return base.Visibility; }
            set { base.Visibility = value; }
#pragma warning restore IDE0009 // Member access should be qualified.
        }

        public void LoadSettings()
        {
            try
            {
                SettingsXml.Instance.Load();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception loading settings:\n" + ex);

                MessageBox.Show(@"Toastify was unable to load the settings file." + Environment.NewLine +
                                    "Delete the Toastify.xml file and restart the application to recreate the settings file." + Environment.NewLine +
                                Environment.NewLine +
                                "The application will now be started with default settings.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                SettingsXml.Instance.Default(setHotKeys: true);
            }
        }

        public Toast()
        {
            this.InitializeComponent();

            // set a static reference back to ourselves, useful for callbacks
            Current = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load settings from XML
            this.LoadSettings();

            string version = VersionChecker.Version;

            Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppLaunch, version);

            if (SettingsXml.Instance.PreviousOS != version)
            {
                Telemetry.TrackEvent(TelemetryCategory.General, Telemetry.TelemetryEvent.AppUpgraded, version);

                SettingsXml.Instance.PreviousOS = version;
            }

            //Init toast(color settings)
            this.InitToast();

            //Init tray icon
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

            this.trayIcon.ContextMenu.MenuItems.Add(menuSettings);

            MenuItem menuAbout = new MenuItem { Text = @"About Toastify..." };
            menuAbout.Click += (s, ev) => { new About().ShowDialog(); };

            this.trayIcon.ContextMenu.MenuItems.Add(menuAbout);

            this.trayIcon.ContextMenu.MenuItems.Add("-");

            MenuItem menuExit = new MenuItem { Text = @"Exit" };
            menuExit.Click += (s, ev) => { Application.Current.Shutdown(); }; //this.Close(); };

            this.trayIcon.ContextMenu.MenuItems.Add(menuExit);

            this.trayIcon.MouseClick += (s, ev) => { if (ev.Button == MouseButtons.Left) this.DisplayAction(SpotifyAction.ShowToast, null); };

            this.trayIcon.DoubleClick += (s, ev) => { Settings.Launch(this); };

            //Init watch timer
            this.watchTimer = new Timer(1000);
            this.watchTimer.Elapsed += (s, ev) =>
            {
                this.watchTimer.Stop();
                this.CheckTitle();
                this.watchTimer.Start();
            };

            this.Deactivated += this.Toast_Deactivated;

            //Remove from ALT+TAB
            Win32API.AddToolWindowStyle(this);

            //Check if Spotify is running.
            this.AskUserToStartSpotify();
            this.LoadPlugins();

            //Let the plugins know we're started.
            foreach (var p in this.Plugins)
            {
                try
                {
                    p.Started();
                }
                catch (Exception)
                {
                    //For now we swallow any plugin errors.
                }
            }

            if (!SettingsXml.Instance.DisableToast)
                this.watchTimer.Enabled = true; //Only need to be enabled if we are going to show the toast.

            this.versionChecker = new VersionChecker();
            this.versionChecker.CheckVersionComplete += this.VersionChecker_CheckVersionComplete;
            this.versionChecker.BeginCheckVersion();

            // TODO: right now this is pretty dumb - kick off update notifications every X hours, this might get annoying
            //       and really we should just pop a notification once per version and probably immediately after a song toast
            var updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Tick += (timerSender, timerE) => { this.versionChecker.BeginCheckVersion(); };
            updateTimer.Interval = new TimeSpan(6, 0, 0);
            updateTimer.Start();
        }

        private void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        public void InitToast()
        {
            const double minWidth = 200.0;
            const double minHeight = 65.0;

            //If we find any invalid settings in the xml we skip it and use default.
            //User notification of bad settings will be implemented with the settings dialog.

            //This method is UGLY but we'll keep it until the settings dialog is implemented.
            SettingsXml settings = SettingsXml.Instance;

            this.ToastBorder.BorderThickness = new Thickness(settings.ToastBorderThickness);

            System.Windows.Media.ColorConverter cc = new System.Windows.Media.ColorConverter();
            if (!string.IsNullOrEmpty(settings.ToastBorderColor) && cc.IsValid(settings.ToastBorderColor))
            {
                object borderBrush = cc.ConvertFrom(settings.ToastBorderColor);
                if (borderBrush != null)
                    this.ToastBorder.BorderBrush = new SolidColorBrush((System.Windows.Media.Color)borderBrush);
            }

            if (!string.IsNullOrEmpty(settings.ToastColorTop) && !string.IsNullOrEmpty(settings.ToastColorBottom) && cc.IsValid(settings.ToastColorTop) && cc.IsValid(settings.ToastColorBottom))
            {
                object top = cc.ConvertFrom(settings.ToastColorTop);
                object bottom = cc.ConvertFrom(settings.ToastColorBottom);

                if (top != null && bottom != null)
                    this.ToastBorder.Background = new LinearGradientBrush((System.Windows.Media.Color)top, (System.Windows.Media.Color)bottom, 90.0);
            }

            if (settings.ToastWidth >= minWidth)
                this.Width = settings.ToastWidth;
            if (settings.ToastHeight >= minHeight)
                this.Height = settings.ToastHeight;

            //If we made it this far we have all the values needed.
            this.ToastBorder.CornerRadius = new CornerRadius(settings.ToastBorderCornerRadiusTopLeft, settings.ToastBorderCornerRadiusTopRight, settings.ToastBorderCornerRadiusBottomRight, settings.ToastBorderCornerRadiusBottomLeft);
        }

        private void CheckTitle()
        {
            Song currentSong = Spotify.Instance.CurrentSong;

            if (currentSong != null && currentSong.IsValid() && !currentSong.Equals(this.currentSong))
            {
                // set the previous title asap so that the next timer call to this function will
                // fail fast (setting it at the end may cause multiple web requests)
                this.currentSong = currentSong;

                try
                {
                    Spotify.Instance.SetCoverArt(currentSong);
                }
                catch
                {
                    // Exceptions will be handled (for telemetry etc.) within SetCoverArt, but they will be rethrown
                    // so that we can set custom artwork here
                    currentSong.CoverArtUrl = ALBUM_ACCESS_DENIED_ICON;
                }

                // Toastify-specific custom logic around album art (if it's missing, or an ad)
                this.UpdateSongForToastify(currentSong);

                this.toastIcon = currentSong.CoverArtUrl;

                this.Dispatcher.Invoke((Action)delegate { this.Title1.Text = currentSong.Track; this.Title2.Text = currentSong.Artist; }, System.Windows.Threading.DispatcherPriority.Normal);

                foreach (var p in this.Plugins)
                {
                    try
                    {
                        p.TrackChanged(currentSong.Artist, currentSong.Track);
                    }
                    catch (Exception)
                    {
                        //For now we swallow any plugin errors.
                    }
                }

                this.Dispatcher.Invoke((Action)delegate { this.FadeIn(); }, System.Windows.Threading.DispatcherPriority.Normal);

                if (SettingsXml.Instance.SaveTrackToFile)
                {
                    if (!string.IsNullOrEmpty(SettingsXml.Instance.SaveTrackToFilePath))
                    {
                        try
                        {
                            string trackText = GetClipboardText(currentSong);

                            File.WriteAllText(SettingsXml.Instance.SaveTrackToFilePath, trackText);
                        }
                        catch
                        {
                            // ignore errors writing out the album
                        }
                    }
                }
            }
        }

        private void UpdateSongForToastify(Song currentSong)
        {
            if (string.IsNullOrWhiteSpace(currentSong.Track))
            {
                currentSong.CoverArtUrl = AD_PLAYING_ICON;
                currentSong.Track = "Spotify Ad";
            }
            else if (string.IsNullOrWhiteSpace(currentSong.CoverArtUrl))
                currentSong.CoverArtUrl = DEFAULT_ICON;
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

            if (!string.IsNullOrEmpty(this.toastIcon))
            {
                this.cover = new BitmapImage();
                this.cover.BeginInit();
                this.cover.UriSource = new Uri(this.toastIcon, UriKind.RelativeOrAbsolute);
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
                    this.Dispatcher.Invoke((Action)delegate
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

        private void VersionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (!e.New)
                return;

            const string title = "Update Toastify!";
            string caption = "Version " + e.Version + " available now.";

            // this is a background thread, so sleep it a bit so that it doesn't clash with the startup toast
            System.Threading.Thread.Sleep(20000);

            this.Dispatcher.Invoke((Action)delegate
            {
                this.Title1.Text = title;
                this.Title2.Text = caption;

                this.toastIcon = "SpotifyToastifyUpdateLogo.png";

                this.FadeIn(true, true);
            }, System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void LoadPlugins()
        {
            //Load plugins
            this.Plugins = new List<Plugin.PluginBase>();
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
                            var plugin = Activator.CreateInstanceFrom(Path.Combine(applicationPath, p.FileName), p.TypeName).Unwrap() as Plugin.PluginBase;
                            plugin?.Init(p.Settings);
                            this.Plugins.Add(plugin);
                        }
                        else
                            Debug.WriteLine("applicationPath is null");
                    }
                    catch (Exception)
                    {
                        //For now we swallow any plugin errors.
                    }
                    Console.WriteLine(@"Loaded " + p.TypeName);
                }
            }
        }

        private void AskUserToStartSpotify()
        {
            // Thanks to recent changes in Spotify that removed the song Artist + Title from the titlebar
            // we are forced to launch Spotify ourselves (under WebDriver), so we no longer ask the user
            try
            {
                Spotify.Instance.StartSpotify();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unknown error occurred when trying to start Spotify.\nPlease start Spotify manually.\n\nTechnical Details: " + e.Message, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
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

            // Let the plugins now we're closing up.
            // we do this last since it's transparent to the user
            foreach (var p in this.Plugins)
            {
                try
                {
                    p.Closing();
                    p.Dispose();
                }
                catch (Exception)
                {
                    //For now we swallow any plugin errors.
                }
            }

            this.Plugins.Clear();

            base.OnClosing(e);
        }

        private Key ConvertKey(Keys key)
        {
            if (Enum.GetNames(typeof(Key)).Contains(key.ToString()))
                return (Key)Enum.Parse(typeof(Key), key.ToString());

            return Key.None;
        }

        #region ActionHookCallback

        private static Hotkey _lastHotkey;
        private static DateTime _lastHotkeyPressTime = DateTime.Now;

        /// <summary>
        /// If the same hotkey press happens within this buffer time, it will be ignored.
        ///
        /// I came to 150 by pressing keys as quickly as possibly. The minimum time was less than 150
        /// but most values fell in the 150 to 200 range for quick presses, so 150 seemed the most reasonable
        /// </summary>
        private const int WAIT_BETWEEN_HOTKEY_PRESS = 150;

        internal static void ActionHookCallback(Hotkey hotkey)
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

            string currentTrack = string.Empty;

            try
            {
                Song songBeforeAction = Current.currentSong;

                if (hotkey.Action == SpotifyAction.CopyTrackInfo && songBeforeAction != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.CopyTrackInfo);

                    CopySongToClipboard(songBeforeAction);
                }
                else if (hotkey.Action == SpotifyAction.PasteTrackInfo && songBeforeAction != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.PasteTrackInfo);

                    CopySongToClipboard(songBeforeAction);

                    SendPasteKey(hotkey);
                }
                else
                {
                    Spotify.Instance.SendAction(hotkey.Action);
                }

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

        private static void SendPasteKey(Hotkey hotkey)
        {
            var shiftKey = new ManagedWinapi.KeyboardKey(Keys.ShiftKey);
            var altKey = new ManagedWinapi.KeyboardKey(Keys.Alt);
            var ctrlKey = new ManagedWinapi.KeyboardKey(Keys.ControlKey);
            var vKey = new ManagedWinapi.KeyboardKey(Keys.V);

            // Before injecting a paste command, first make sure that no modifiers are already
            // being pressed (which will throw off the Ctrl+v).
            // Since key state is notoriously unreliable, set a max sleep so that we don't get stuck
            var maxSleep = 250;

            // minimum sleep time
            System.Threading.Thread.Sleep(150);

            //System.Diagnostics.Debug.WriteLine("shift: " + shiftKey.State + " alt: " + altKey.State + " ctrl: " + ctrlKey.State);

            while (maxSleep > 0 && (shiftKey.State != 0 || altKey.State != 0 || ctrlKey.State != 0))
                System.Threading.Thread.Sleep(maxSleep -= 50);

            //System.Diagnostics.Debug.WriteLine("maxSleep: " + maxSleep);

            // press keys in sequence. Don't use PressAndRelease since that seems to be too fast
            // for most applications and the sequence gets lost.
            ctrlKey.Press();
            vKey.Press();
            System.Threading.Thread.Sleep(25);
            vKey.Release();
            System.Threading.Thread.Sleep(25);
            ctrlKey.Release();
        }

        private static string GetClipboardText(Song currentSong)
        {
            string trackBeforeAction = currentSong.ToString();
            var template = SettingsXml.Instance.ClipboardTemplate;

            // if the string is empty we set it to {0}
            if (string.IsNullOrWhiteSpace(template))
                template = "{0}";

            // add the song name to the end of the template if the user forgot to put in the
            // replacement marker
            if (!template.Contains("{0}"))
                template += " {0}";

            return string.Format(template, trackBeforeAction);
        }

        private static void CopySongToClipboard(Song trackBeforeAction)
        {
            Clipboard.SetText(GetClipboardText(trackBeforeAction));
        }

        #endregion ActionHookCallback

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
                this.toastIcon = DEFAULT_ICON;
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
                        this.toastIcon = DEFAULT_ICON;

                        this.Title1.Text = nothingsPlaying;
                        this.Title2.Text = string.Empty;
                    }
                    else
                    {
                        if (currentTrack.IsValid())
                        {
                            this.toastIcon = currentTrack.CoverArtUrl;

                            this.Title1.Text = currentTrack.Artist;
                            this.Title2.Text = currentTrack.Track;
                        }
                    }

                    this.FadeIn(true);
                    break;

                case SpotifyAction.ShowSpotify:  //No need to handle
                    break;

                case SpotifyAction.ThumbsUp:
                    this.toastIcon = "Resources/thumbs_up.png";

                    this.Title1.Text = "Thumbs Up!";
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;

                case SpotifyAction.ThumbsDown:
                    this.toastIcon = "Resources/thumbs_down.png";

                    this.Title1.Text = "Thumbs Down :(";
                    this.Title2.Text = currentTrack.ToString();
                    this.FadeIn();
                    break;
            }
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
    }
}