using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Toastify
{
    public partial class Toast : Window
    {
        private readonly string SETTINGS_FILE = "Toastify.xml";

        Timer watchTimer;
        System.Windows.Forms.NotifyIcon trayIcon;

        internal List<Hotkey> HotKeys { get; set; }

        SettingsXml settings;
        public Toast()
        {
            InitializeComponent();

            string applicationPath = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            string fullPathSettingsFile = System.IO.Path.Combine(applicationPath, SETTINGS_FILE);

            if (!System.IO.File.Exists(fullPathSettingsFile))
            {
                settings = SettingsXml.Defaul;
                try
                {
                    settings.Save(fullPathSettingsFile);
                }
                catch (Exception)
                {
                    MessageBox.Show(@"Toastify was unable to create the settings file." + Environment.NewLine +
                                     "Make sure the application is executed from a folder with write access." + Environment.NewLine +
                                     Environment.NewLine + 
                                     "The application will now be started with default settings.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                try
                {
                    settings = SettingsXml.Open(fullPathSettingsFile);
                }
                catch(Exception)
                {
                    MessageBox.Show(@"Toastify was unable to load the settings file." + Environment.NewLine +
                                     "Delete the Toastify.xml file and restart the application to recreate the settings file." + Environment.NewLine +
                                    Environment.NewLine + 
                                    "The application will now be started with default settings.", "Toastify", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            //Init toast(color settings)
            InitToast();

            //Init tray icon
            trayIcon = new System.Windows.Forms.NotifyIcon();
            trayIcon.Icon = Toastify.Properties.Resources.spotifyicon;
            trayIcon.Text = "Toastify";
            trayIcon.Visible = true;

            //Init tray icon menu
            System.Windows.Forms.MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += (s, e) => { this.Close(); };
            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add(menuExit);
            
            //Init watch timer
            watchTimer = new Timer(500);
            watchTimer.Elapsed += (s, e) =>
            {
                CheckTitle();
            };
        }

        private void InitToast()
        {
            const double MIN_WIDTH = 200.0;
            const double MIN_HEIGHT = 65.0;

            //If we find any invalid settings in the xml we skip it and use default.
            //User notification of bad settings will be implemented with the settings dialog.

            //This method is UGLY but we'll keep it until the settings dialog is implemented.


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


            if (!string.IsNullOrEmpty(settings.ToastBorderCornerRadious))
            {
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                string[] parts = settings.ToastBorderCornerRadious.Split(',');
                if (parts.Length != 4)
                    return;

                double topleft, topright, bottomright, bottomleft;
                if (!double.TryParse(parts[0], NumberStyles.Float, culture, out topleft))
                    return;
                if (!double.TryParse(parts[1], NumberStyles.Float, culture, out topright))
                    return;
                if (!double.TryParse(parts[2], NumberStyles.Float, culture, out bottomright))
                    return;
                if (!double.TryParse(parts[3], NumberStyles.Float, culture, out bottomleft))
                    return;

                //If we made it this far we have all the values needed.
                ToastBorder.CornerRadius = new CornerRadius(topleft, topright, bottomright, bottomleft);
            }
        }

        string previousTitle = string.Empty;
        private void CheckTitle()
        {
            string currentTitle = Spotify.GetCurrentTrack();
            if (!string.IsNullOrEmpty(currentTitle) && currentTitle != previousTitle)
            {
                string[] parts = currentTitle.Split('\u2013'); //Spotify uses an en dash to separate Artist and Title
                if (parts.Length == 0)
                    return;

                if (parts.Length == 2)
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = parts[1].Trim(); Title2.Text = parts[0].Trim(); }, System.Windows.Threading.DispatcherPriority.Normal);
                else
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = parts[0].Trim(); Title2.Text = ""; }, System.Windows.Threading.DispatcherPriority.Normal);

                previousTitle = currentTitle;
                this.Dispatcher.Invoke((Action)delegate { FadeIn(); }, System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private void FadeIn()
        {
            if (settings.DisableToast)
                return;

            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 5.0;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 70.0;

            DoubleAnimation anim = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(250));
            anim.Completed += (s, e) => { FadeOut(); };
            this.BeginAnimation(Window.OpacityProperty, anim);
        }

        private void FadeOut()
        {
            DoubleAnimation anim = new DoubleAnimation(0.0, TimeSpan.FromMilliseconds(500));
            anim.BeginTime = TimeSpan.FromMilliseconds(settings.FadeOutTime);
            this.BeginAnimation(Window.OpacityProperty, anim);
        }

        KeyboardHook hook;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!settings.DisableToast)
                watchTimer.Enabled = true; //Only need to be enabled if we are going to show the toast.

            if (settings.GlobalHotKeys)
            {
                hook = new KeyboardHook();
                hook.KeyUp += new KeyboardHook.HookEventHandler(hook_KeyUp);
            }
        }

        private System.Windows.Input.Key ConvertKey(System.Windows.Forms.Keys key)
        {
            if (Enum.GetNames(typeof(System.Windows.Input.Key)).Contains(key.ToString()))
                return (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), key.ToString());
            else
                return Key.None;
        } 

        void hook_KeyUp(object sender, HookEventArgs e)
        {
            string currentTrack = string.Empty;
            var key = ConvertKey(e.Key);

            foreach (var hotkey in settings.HotKeys)
            {
                if (hotkey.Alt == e.Alt && hotkey.Ctrl == e.Control && hotkey.Shift == e.Shift && hotkey.Key == key)
                {
                    string trackBeforeAction = Spotify.GetCurrentTrack();
                    Spotify.SendAction(hotkey.Action);
                    DisplayAction(hotkey.Action, trackBeforeAction);
                }
            }
        }

        private void DisplayAction(SpotifyAction action, string trackBeforeAction)
        {
            //Anything that changes track doesn't need to be handled since
            //that will be handled in the timer event.

            if (!Spotify.IsAvailable())
            {
                Title1.Text = "Spotify not available!";
                Title2.Text = string.Empty;
                FadeIn();
                return;
            }

            string currentTrack = Spotify.GetCurrentTrack();
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
                case SpotifyAction.NextTrack:      //No need to handle
                    break;
                case SpotifyAction.PreviousTrack:  //No need to handle
                    break;
                case SpotifyAction.VolumeUp:
                    Title1.Text = "Volume ++";
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.VolumeDown:
                    Title1.Text = "Volume --";
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.Mute:
                    Title1.Text = "Mute On/Off";
                    Title2.Text = currentTrack;
                    FadeIn();
                    break;
                case SpotifyAction.ShowToast:
                    FadeIn();
                    break;
                case SpotifyAction.ShowSpotify:  //No need to handle
                    break;
            }
        }
    }
}
