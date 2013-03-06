using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Diagnostics;

namespace Toastify
{
    public partial class Toast : Window
    {
        Timer watchTimer;
        System.Windows.Forms.NotifyIcon trayIcon;

        string coverUrl = "";
        BitmapImage cover;

        internal List<Hotkey> HotKeys { get; set; }
        internal List<Toastify.Plugin.PluginBase> Plugins { get; set; }

        internal static Toast Current { get; private set; }

        public void LoadSettings()
        {

            try
            {
                SettingsXml.Current.Load();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception loading settings:\n" + ex);

                MessageBox.Show(@"Toastify was unable to load the settings file." + Environment.NewLine +
                                    "Delete the Toastify.xml file and restart the application to recreate the settings file." + Environment.NewLine +
                                Environment.NewLine +
                                "The application will now be started with default settings.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);

                SettingsXml.Current.Default();
            }
        }

        public Toast()
        {
            InitializeComponent();

            // set a static reference back to ourselves, useful for callbacks
            Current = this;

            //Load settings from XML
            LoadSettings();

            //Init toast(color settings)
            InitToast();

            //Init tray icon
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = Toastify.Properties.Resources.spotifyicon;
            trayIcon.Text = "Toastify";
            trayIcon.Visible = true;

            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

            //Init tray icon menu
            System.Windows.Forms.MenuItem menuSettings = new System.Windows.Forms.MenuItem();
            menuSettings.Text = "Settings";
            menuSettings.Click += (s, e) => { new Settings(this).ShowDialog(); };

            trayIcon.ContextMenu.MenuItems.Add(menuSettings);
            
            System.Windows.Forms.MenuItem menuAbout = new System.Windows.Forms.MenuItem();
            menuAbout.Text = "About Toastify...";
            menuAbout.Click += (s, e) => { new About().ShowDialog(); };

            trayIcon.ContextMenu.MenuItems.Add(menuAbout);

            trayIcon.ContextMenu.MenuItems.Add("-");

            System.Windows.Forms.MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += (s, e) => { Application.Current.Shutdown(); }; //this.Close(); };

            trayIcon.ContextMenu.MenuItems.Add(menuExit);

            //Init watch timer
            watchTimer = new Timer(1000);
            watchTimer.Elapsed += (s, e) =>
            {
                CheckTitle();
            };
        }

        public void InitToast()
        {
            const double MIN_WIDTH = 200.0;
            const double MIN_HEIGHT = 65.0;

            //If we find any invalid settings in the xml we skip it and use default.
            //User notification of bad settings will be implemented with the settings dialog.

            //This method is UGLY but we'll keep it until the settings dialog is implemented.
            SettingsXml settings = SettingsXml.Current;

            ToastBorder.BorderThickness = new Thickness(settings.ToastBorderThickness);

            ColorConverter cc = new ColorConverter();
            if (!string.IsNullOrEmpty(settings.ToastBorderColor) && cc.IsValid(settings.ToastBorderColor))
                ToastBorder.BorderBrush = new SolidColorBrush((Color)cc.ConvertFrom(settings.ToastBorderColor));

            if (!string.IsNullOrEmpty(settings.ToastColorTop) && !string.IsNullOrEmpty(settings.ToastColorBottom) && cc.IsValid(settings.ToastColorTop) && cc.IsValid(settings.ToastColorBottom))
            {
                Color top = (Color)cc.ConvertFrom(settings.ToastColorTop);
                Color botton = (Color)cc.ConvertFrom(settings.ToastColorBottom);

                ToastBorder.Background = new LinearGradientBrush(top, botton, 90.0);
            }

            if (settings.ToastWidth >= MIN_WIDTH)
                this.Width = settings.ToastWidth;
            if (settings.ToastHeight >= MIN_HEIGHT)
                this.Height = settings.ToastHeight;

            //If we made it this far we have all the values needed.
            ToastBorder.CornerRadius = new CornerRadius(settings.ToastBorderCornerRadiusTopLeft, settings.ToastBorderCornerRadiusTopRight, settings.ToastBorderCornerRadiusBottomRight, settings.ToastBorderCornerRadiusBottomLeft);

        }

        string previousTitle = string.Empty;
        private void CheckTitle()
        {
            string currentTitle = Spotify.GetCurrentTrack();
            if (!string.IsNullOrEmpty(currentTitle) && currentTitle != previousTitle)
            {
                string part1, part2;

                // set the previous title asap so that the next timer call to this function will
                // fail fast (setting it at the end may cause multiple web requests)
                previousTitle = currentTitle;

                if (SplitTitle(currentTitle, out part1, out part2))
                {
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = part2; Title2.Text = part1; }, System.Windows.Threading.DispatcherPriority.Normal);

                    foreach (var p in this.Plugins)
                    {
                        try
                        {
                            p.TrackChanged(part1, part2);
                        }
                        catch (Exception)
                        {
                            //For now we swallow any plugin errors.
                        }
                    }
                }

                try
                {
                    System.Diagnostics.Debug.WriteLine("http://ws.audioscrobbler.com/2.0/?method=track.getinfo&api_key=b25b959554ed76058ac220b7b2e0a026&artist=" + part1 + "&track=" + part2);
                    XPathDocument doc = new XPathDocument("http://ws.audioscrobbler.com/2.0/?method=track.getinfo&api_key=b25b959554ed76058ac220b7b2e0a026&artist=" + part1 + "&track=" + part2);

                    XPathNavigator navigator = doc.CreateNavigator();
                    XPathNodeIterator nodeImage = navigator.Select("/lfm/track/album/image[@size='medium']");

                    if (nodeImage.MoveNext())
                    {
                        XPathNavigator node = nodeImage.Current;
                        coverUrl = node.InnerXml;
                    }
                    else
                        coverUrl = "SpotifyToastifyLogo.png";
                }
                catch (Exception)
                {
                    coverUrl = "SpotifyToastifyLogo.png";
                }

                this.Dispatcher.Invoke((Action)delegate { FadeIn(); }, System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private bool SplitTitle(string title, out string part1, out string part2)
        {
            part1 = string.Empty;
            part2 = string.Empty;

            string[] parts = title.Split('\u2013'); //Spotify uses an en dash to separate Artist and Title
            if (parts.Length < 1 || parts.Length > 2)
                return false; //Invalid title

            if (parts.Length == 1)
                part2 = parts[0].Trim();
            else if (parts.Length == 2)
            {
                part1 = parts[0].Trim();
                part2 = parts[1].Trim();
            }

            return true;
        }

        private void FadeIn(bool force = false)
        {
            SettingsXml settings = SettingsXml.Current;

            if ((settings.DisableToast || settings.OnlyShowToastOnHotkey) && !force)
                return;

            if (coverUrl != "")
            {
                cover = new BitmapImage();
                cover.BeginInit();
                cover.UriSource = new Uri(coverUrl, UriKind.RelativeOrAbsolute);
                cover.EndInit();
                LogoToast.Source = cover;
            }

            System.Drawing.Rectangle workingArea = new System.Drawing.Rectangle((int)this.Left, (int)this.Height, (int)this.ActualWidth, (int)this.ActualHeight);
            workingArea = System.Windows.Forms.Screen.GetWorkingArea(workingArea);

            this.Left = workingArea.Right - this.ActualWidth - settings.OffsetRight;
            this.Top = workingArea.Bottom - this.ActualHeight - settings.OffsetBottom;

            // force the toast to be topmost (we set this in code since for some reason the setting seems to
            // get lost after a while, this seems to refresh it)
            this.Topmost = true;

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) => { FadeOut(); };
            this.BeginAnimation(Window.OpacityProperty, anim);
        }

        private void FadeOut()
        {
            DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
            anim.BeginTime = TimeSpan.FromMilliseconds(SettingsXml.Current.FadeOutTime);
            this.BeginAnimation(Window.OpacityProperty, anim);
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Remove from ALT+TAB
            WinHelper.AddToolWindowStyle(this);

            //Check if Spotify is running.
            EnsureSpotify();
            LoadPlugins();

            if (!SettingsXml.Current.DisableToast)
                watchTimer.Enabled = true; //Only need to be enabled if we are going to show the toast.

            //Let the plugins now we're started.
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
        }

        private void LoadPlugins()
        {
            //Load plugins
            this.Plugins = new List<Toastify.Plugin.PluginBase>();
            string applicationPath = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;

            foreach (var p in SettingsXml.Current.Plugins)
            {
                try
                {
                    var plugin = Activator.CreateInstanceFrom(System.IO.Path.Combine(applicationPath, p.FileName), p.TypeName).Unwrap() as Toastify.Plugin.PluginBase;
                    plugin.Init(p.Settings);
                    this.Plugins.Add(plugin);
                }
                catch (Exception)
                {
                    //For now we swallow any plugin errors.
                }
                Console.WriteLine("Loaded " + p.TypeName);
            }
        }

        private void EnsureSpotify()
        {
            SettingsXml settings = SettingsXml.Current;

            //Make sure Spotify is running when starting Toastify.
            //If not ask the user and try to start it.

            if (!Spotify.IsAvailable())
            {
                if ((settings.AlwaysStartSpotify.HasValue && settings.AlwaysStartSpotify.Value) || (MessageBox.Show("Spotify doesn't seem to be running.\n\nDo you want Toastify to try and start it for you?", "Toastify", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
                {
                    string spotifyPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;  //string.Empty = (Default) value

                    // try in the secondary location
                    if (string.IsNullOrEmpty(spotifyPath))
                    {
                        spotifyPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;  //string.Empty = (Default) value
                    }

                    if (string.IsNullOrEmpty(spotifyPath)) 
                    {
                        MessageBox.Show("Unable to find Spotify. Make sure it is installed and/or start it manually.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(System.IO.Path.Combine(spotifyPath, "Spotify.exe"));

                            if (!settings.AlwaysStartSpotify.HasValue)
                            {
                                var ret = MessageBox.Show("Do you always want to start Spotify if it's not already running?", "Toastify", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                settings.AlwaysStartSpotify = (ret == MessageBoxResult.Yes);
                                settings.Save();
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("An unknown error occurd when trying to start Spotify.\nPlease start Spotify manually.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // close Spotify first
            if (SettingsXml.Current.CloseSpotifyWithToastify)
            {
                Process[] possibleSpotifys = Process.GetProcessesByName("Spotify");
                if (possibleSpotifys.Count() > 0)
                {
                    using (Process spotify = possibleSpotifys[0])
                    {

                        try
                        {
                            // try to close spotify gracefully
                            if (spotify.CloseMainWindow())
                            {
                                spotify.WaitForExit(1000);
                            }

                            // didn't work (Spotify often treats window close as hide main window) :( Kill them!
                            if (!spotify.HasExited)
                                Process.GetProcessesByName("Spotify")[0].Kill();
                        }
                        catch { } // ignore all process exceptions
                    }
                }
            }

            // Ensure trayicon is removed on exit. (Thx Linus)
            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayIcon = null;

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

        private System.Windows.Input.Key ConvertKey(System.Windows.Forms.Keys key)
        {
            if (Enum.GetNames(typeof(System.Windows.Input.Key)).Contains(key.ToString()))
                return (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), key.ToString());
            else
                return Key.None;
        }
        
        #region ActionHookCallback

        private static Hotkey _lastHotkey = null;
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

            _lastHotkey          = hotkey;
            _lastHotkeyPressTime = DateTime.Now;

            string currentTrack = string.Empty;
            
            try
            {
                string trackBeforeAction = Spotify.GetCurrentTrack();
                if (hotkey.Action == SpotifyAction.CopyTrackInfo && !string.IsNullOrEmpty(trackBeforeAction))
                    Clipboard.SetText(string.Format(SettingsXml.Current.ClipboardTemplate, trackBeforeAction));
                else
                    Spotify.SendAction(hotkey.Action);

                Toast.Current.DisplayAction(hotkey.Action, trackBeforeAction);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception with hooked key! " + ex);
                Toast.Current.Title1.Text = "Unable to communicate with Spotify";
                Toast.Current.Title2.Text = "";
                Toast.Current.FadeIn();
            }
        }

        #endregion

        public void DisplayAction(SpotifyAction action, string trackBeforeAction)
        {
            //Anything that changes track doesn't need to be handled since
            //that will be handled in the timer event.

            const string VOLUME_UP_TEXT = "Volume ++";
            const string VOLUME_DOWN_TEXT = "Volume --";
            const string MUTE_ON_OFF_TEXT = "Mute On/Off";
            const string NOTHINGS_PLAYING = "Nothing's playing";
            const string PAUSED_TEXT = "Paused";
            const string STOPPED_TEXT = "Stopped";
            const string SETTINGS_TEXT = "Settings saved";

            if (!Spotify.IsAvailable() && action != SpotifyAction.SettingsSaved)
            {
                coverUrl = "SpotifyToastifyLogo.png";
                Title1.Text = "Spotify not available!";
                Title2.Text = string.Empty;
                FadeIn();
                return;
            }

            string currentTrack = Spotify.GetCurrentTrack();

            string prevTitle1 = Title1.Text;
            string prevTitle2 = Title2.Text;

            switch (action)
            {
                case SpotifyAction.PlayPause:
                    if (!string.IsNullOrEmpty(trackBeforeAction))
                    {
                        //We pressed pause
                        Title1.Text = "Paused";
                        Title2.Text = trackBeforeAction;
                        FadeIn();
                    }
                    previousTitle = string.Empty;  //If we presses play this will force a toast to display in next timer event.
                    break;
                case SpotifyAction.Stop:
                    previousTitle = string.Empty;
                    Title1.Text = "Stopped";
                    Title2.Text = trackBeforeAction;
                    FadeIn();
                    break;
                case SpotifyAction.SettingsSaved:
                    Title1.Text = SETTINGS_TEXT;
                    Title2.Text = "Here is a preview of your settings!";
                    FadeIn();
                    break;
                case SpotifyAction.NextTrack:      //No need to handle
                    break;
                case SpotifyAction.PreviousTrack:  //No need to handle
                    break;
                case SpotifyAction.VolumeUp:
                    Title1.Text = VOLUME_UP_TEXT;
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.VolumeDown:
                    Title1.Text = VOLUME_DOWN_TEXT;
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.Mute:
                    Title1.Text = MUTE_ON_OFF_TEXT;
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.ShowToast:
                    if (string.IsNullOrEmpty(currentTrack) && Title1.Text != PAUSED_TEXT && Title1.Text != STOPPED_TEXT)
                    {
                        coverUrl = "SpotifyToastifyLogo.png";
                        Title1.Text = NOTHINGS_PLAYING;
                        Title2.Text = string.Empty;
                    }
                    else
                    {
                        string part1, part2;
                        if (SplitTitle(currentTrack, out part1, out part2))
                        {
                            Title1.Text = part2;
                            Title2.Text = part1;
                        }
                    }
                    FadeIn(force:true);
                    break;
                case SpotifyAction.ShowSpotify:  //No need to handle
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
            this.BeginAnimation(Window.OpacityProperty, null);
            this.Opacity = 1.0;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            FadeOut();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FadeOut();
            Spotify.SendAction(SpotifyAction.ShowSpotify);
        }
    }
}
