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
using System.Web;
using System.Runtime.InteropServices;

namespace Toastify
{
    public partial class Toast : Window
    {
        Timer watchTimer;
        System.Windows.Forms.NotifyIcon trayIcon;

        string coverUrl = "";
        BitmapImage cover;

        private VersionChecker versionChecker;
        private bool isUpdateToast = false;

        internal List<Hotkey> HotKeys { get; set; }
        internal List<Toastify.Plugin.PluginBase> Plugins { get; set; }

        internal static Toast Current { get; private set; }

        string previousTitle = string.Empty;

        private bool dragging = false;

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

                SettingsXml.Current.Default(setHotKeys: true);
            }
        }

        public Toast()
        {
            InitializeComponent();

            // set a static reference back to ourselves, useful for callbacks
            Current = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
            menuSettings.Click += (s, ev) => { Settings.Launch(this); };

            trayIcon.ContextMenu.MenuItems.Add(menuSettings);

            System.Windows.Forms.MenuItem menuAbout = new System.Windows.Forms.MenuItem();
            menuAbout.Text = "About Toastify...";
            menuAbout.Click += (s, ev) => { new About().ShowDialog(); };

            trayIcon.ContextMenu.MenuItems.Add(menuAbout);

            trayIcon.ContextMenu.MenuItems.Add("-");

            System.Windows.Forms.MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += (s, ev) => { Application.Current.Shutdown(); }; //this.Close(); };

            trayIcon.ContextMenu.MenuItems.Add(menuExit);

            trayIcon.MouseClick += (s, ev) => { if (ev.Button == System.Windows.Forms.MouseButtons.Left) DisplayAction(SpotifyAction.ShowToast, null); };

            trayIcon.DoubleClick += (s, ev) => { Settings.Launch(this); };

            //Init watch timer
            watchTimer = new Timer(1000);
            watchTimer.Elapsed += (s, ev) =>
            {
                watchTimer.Stop();
                CheckTitle();
                watchTimer.Start();
            };

            this.Deactivated += Toast_Deactivated;

            //Remove from ALT+TAB
            WinHelper.AddToolWindowStyle(this);

            //Check if Spotify is running.
            AskUserToStartSpotify();
            LoadPlugins();

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

            if (!SettingsXml.Current.DisableToast)
                watchTimer.Enabled = true; //Only need to be enabled if we are going to show the toast.

            versionChecker = new VersionChecker();
            versionChecker.CheckVersionComplete += new EventHandler<CheckVersionCompleteEventArgs>(versionChecker_CheckVersionComplete);
            versionChecker.BeginCheckVersion();

            // TODO: right now this is pretty dumb - kick off update notifications every X hours, this might get annoying
            //       and really we should just pop a notification once per version and probably immediately after a song toast
            var updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Tick += (timerSender, timerE) => { versionChecker.BeginCheckVersion(); };
            updateTimer.Interval = new TimeSpan(6, 0, 0);
            updateTimer.Start();
        }

        void Toast_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
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

        private void CheckTitle()
        {
            string currentTitle = Spotify.GetCurrentTrack();

            if (!string.IsNullOrEmpty(currentTitle) && currentTitle != previousTitle)
            {
                string artist, title;

                // set the previous title asap so that the next timer call to this function will
                // fail fast (setting it at the end may cause multiple web requests)
                previousTitle = currentTitle;

                if (SplitTitle(currentTitle, out artist, out title))
                {
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = title; Title2.Text = artist; }, System.Windows.Threading.DispatcherPriority.Normal);

                    foreach (var p in this.Plugins)
                    {
                        try
                        {
                            p.TrackChanged(artist, title);
                        }
                        catch (Exception)
                        {
                            //For now we swallow any plugin errors.
                        }
                    }
                }

                CheckTitle(artist, title);

                this.Dispatcher.Invoke((Action)delegate { FadeIn(); }, System.Windows.Threading.DispatcherPriority.Normal);

                if (SettingsXml.Current.SaveTrackToFile)
                {
                    if (!string.IsNullOrEmpty(SettingsXml.Current.SaveTrackToFilePath))
                    {
                        try
                        {
                            string trackText = GetClipboardText(currentTitle);

                            File.WriteAllText(SettingsXml.Current.SaveTrackToFilePath, trackText);
                        }
                        catch { } // ignore errors writing out the album
                    }
                }
            }
        }

        private void CheckTitle(string artist, string title)
        {

            try
            {
                // Spotify now has a supported metadata web service that is open to all (https://developer.spotify.com/technologies/web-api/) with
                // a workaround to grab album art. See file history for the audio scrobbler method (which stopped working due to the dev key expiring)

                String URLString = "http://ws.spotify.com/search/1/track?q=artist:\"" + System.Uri.EscapeDataString(artist + "\" title:\"" + title + "\"");

                string xmlStr = String.Empty;
                using (var wc = new WebClient())
                {
                    try
                    {
                        xmlStr += wc.DownloadString(URLString);
                    }
                    catch (Exception) { }
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStr);

                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("spotify", "http://www.spotify.com/ns/music/1");

                var trackNode = xmlDoc.SelectSingleNode("//spotify:track/@href", nsmgr);

                // we're protected from infinite recursion by stripping out "(" from title (and by the stack :) )
                if (trackNode == null && title.Contains("("))
                {
                    // when Spotify has () in the brackets the search results often need this to be translated into "From". For example:
                    // Title: Cantina Band (Star Wars I)
                    // is actually displayed in search as (searching with () will result in 0 results):
                    // Title: Cantina Band - From "Star Wars I"

                    CheckTitle(artist, title.Replace("(", "from ").Replace(")", ""));
                    return;
                }

                string spotifyurl = trackNode.Value;

                String embedString = "https://embed.spotify.com/oembed/?url=" + spotifyurl + "&format=xml";

                string exmlStr = String.Empty;
                using (var wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    try
                    {
                        exmlStr = wc.DownloadString(embedString);
                    }
                    catch (Exception) { }
                }

                // Spotify has a bug that embeds various html codes and other undefined characters in their XML which is not valid when we try to parse them
                // for example: https://embed.spotify.com/oembed/?url=spotify:track:6ktNcdXhsQBqkXuacCAybC&format=xml

                var invalidEmbeds = new List<string> { "&eacute;", "&Ouml;" };

                foreach (var invalidEmbed in invalidEmbeds)
                    exmlStr = exmlStr.Replace(invalidEmbed, "");

                var exmlDoc = new XmlDocument();
                exmlDoc.LoadXml(exmlStr);

                // thumbnail_url can be modified for 60,300 or 640px with a replace. See: https://toastify.codeplex.com/workitem/13235
                coverUrl = exmlDoc.SelectSingleNode("//thumbnail_url").InnerXml.Replace("/cover/", "/60/");

            }
            catch (Exception)
            {
                coverUrl = "SpotifyToastifyLogo.png";
            }
        }

        private bool SplitTitle(string title, out string part1, out string part2)
        {
            part1 = string.Empty;
            part2 = string.Empty;

            string[] parts = title.Split('-');
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

        private void FadeIn(bool force = false, bool isUpdate = false)
        {
            if (dragging)
                return;

            SettingsXml settings = SettingsXml.Current;

            if ((settings.DisableToast || settings.OnlyShowToastOnHotkey) && !force)
                return;

            isUpdateToast = isUpdate;

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

            this.Left = settings.PositionLeft;
            this.Top = settings.PositionTop;

            ResetPositionIfOffScreen(workingArea);

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) => { FadeOut(); };
            this.BeginAnimation(Window.OpacityProperty, anim);

            this.Topmost = true;
        }

        private void ResetPositionIfOffScreen(System.Drawing.Rectangle workingArea)
        {
            var rect = new System.Drawing.Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);

            if (!System.Windows.Forms.Screen.AllScreens.Any(s => s.WorkingArea.Contains(rect)))
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
            DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
            anim.BeginTime = TimeSpan.FromMilliseconds((now ? 0 : SettingsXml.Current.FadeOutTime));
            this.BeginAnimation(Window.OpacityProperty, anim);
        }

        void versionChecker_CheckVersionComplete(object sender, CheckVersionCompleteEventArgs e)
        {
            if (!e.New)
                return;

            string title = "Update Toastify!";
            string caption = "Version " + e.Version + " available now.";

            // this is a background thread, so sleep it a bit so that it doesn't clash with the startup toast
            System.Threading.Thread.Sleep(20000);

            this.Dispatcher.Invoke((Action)delegate
            {
                Title1.Text = title;
                Title2.Text = caption;

                coverUrl = "SpotifyToastifyUpdateLogo.png";

                FadeIn(force: true, isUpdate: true);
            }, System.Windows.Threading.DispatcherPriority.Normal);
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

        private void AskUserToStartSpotify()
        {
            SettingsXml settings = SettingsXml.Current;

            // Thanks to recent changes in Spotify that removed the song Artist + Title from the titlebar
            // we are forced to launch Spotify ourselves (under WebDriver), so we no longer ask the user
            try
            {
                Spotify.StartSpotify();
            }
            catch (Exception e)
            {
                MessageBox.Show("An unknown error occurred when trying to start Spotify.\nPlease start Spotify manually.\n\nTechnical Details: " + e.Message, "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // close Spotify first
            if (SettingsXml.Current.CloseSpotifyWithToastify)
            {
                Spotify.KillSpotify();
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

            _lastHotkey = hotkey;
            _lastHotkeyPressTime = DateTime.Now;

            string currentTrack = string.Empty;

            try
            {
                string trackBeforeAction = Spotify.GetCurrentTrack();
                if (hotkey.Action == SpotifyAction.CopyTrackInfo && !string.IsNullOrEmpty(trackBeforeAction))
                {
                    CopySongToClipboard(trackBeforeAction);
                }
                else if (hotkey.Action == SpotifyAction.PasteTrackInfo && !string.IsNullOrEmpty(trackBeforeAction))
                {
                    CopySongToClipboard(trackBeforeAction);

                    SendPasteKey(hotkey);
                }
                else
                {
                    Spotify.SendAction(hotkey.Action);
                }

                Toast.Current.DisplayAction(hotkey.Action, trackBeforeAction);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine("Exception with hooked key! " + ex);
                Toast.Current.Title1.Text = "Unable to communicate with Spotify";
                Toast.Current.Title2.Text = "";
                Toast.Current.FadeIn();
            }
        }

        private static void SendPasteKey(Hotkey hotkey)
        {
            var shiftKey = new ManagedWinapi.KeyboardKey(System.Windows.Forms.Keys.ShiftKey);
            var altKey = new ManagedWinapi.KeyboardKey(System.Windows.Forms.Keys.Alt);
            var ctrlKey = new ManagedWinapi.KeyboardKey(System.Windows.Forms.Keys.ControlKey);
            var vKey = new ManagedWinapi.KeyboardKey(System.Windows.Forms.Keys.V);

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

        private static string GetClipboardText(string trackBeforeAction)
        {
            var template = SettingsXml.Current.ClipboardTemplate;

            // if the string is empty we set it to {0}
            if (string.IsNullOrWhiteSpace(template))
                template = "{0}";

            // add the song name to the end of the template if the user forgot to put in the
            // replacement marker
            if (!template.Contains("{0}"))
                template += " {0}";

            return string.Format(template, trackBeforeAction);
        }

        private static void CopySongToClipboard(string trackBeforeAction)
        {
            Clipboard.SetText(GetClipboardText(trackBeforeAction));
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
                    FadeIn(force: true);
                    break;
                case SpotifyAction.ShowSpotify:  //No need to handle
                    break;
                case SpotifyAction.ThumbsUp:
                    coverUrl = "Resources/thumbs_up.png";

                    Title1.Text = "Thumbs Up!";
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.ThumbsDown:
                    coverUrl = "Resources/thumbs_down.png";

                    Title1.Text = "Thumbs Down :(";
                    Title2.Text = currentTrack;
                    FadeIn();
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
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                dragging = true;
                DragMove();
                return;
            }

            FadeOut(now: true);

            if (isUpdateToast)
            {
                Process.Start(new ProcessStartInfo(versionChecker.UpdateUrl));
            }
            else
            {
                Spotify.SendAction(SpotifyAction.ShowSpotify);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dragging)
            {
                dragging = false;

                // save the new window position
                SettingsXml settings = SettingsXml.Current;

                settings.PositionLeft = this.Left;
                settings.PositionTop = this.Top;

                settings.Save();
            }
        }


    }
}
