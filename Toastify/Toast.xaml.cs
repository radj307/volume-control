using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Timers;
using System.Text.RegularExpressions;

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
                settings.Save(fullPathSettingsFile);
            }
            else
                settings = SettingsXml.Open(fullPathSettingsFile);            

            //Init hotkeys
            this.HotKeys = new List<Hotkey>();
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.Up, Action = SpotifyAction.PlayPause });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.Right, Action = SpotifyAction.NextTrack });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.Left, Action = SpotifyAction.PreviousTrack });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.Down, Action = SpotifyAction.Stop });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.PageUp, Action = SpotifyAction.VolumeUp });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.PageDown, Action = SpotifyAction.VolumeDown });
            this.HotKeys.Add(new Hotkey { Ctrl = true, Alt = true, Key = Key.M, Action = SpotifyAction.Mute });

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
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = parts[1].Trim(); Title2.Text = parts[0].Trim(); });
                else
                    this.Dispatcher.Invoke((Action)delegate { Title1.Text = parts[0].Trim(); Title2.Text = ""; });

                previousTitle = currentTitle;
                this.Dispatcher.Invoke((Action)delegate { FadeIn(); });
            }
        }

        private void FadeIn()
        {
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
            watchTimer.Enabled = true;

            hook = new KeyboardHook();
            hook.KeyUp += new KeyboardHook.HookEventHandler(hook_KeyUp);
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
            if (!settings.GlobalHotKeys)
                return;

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
            }
        }
    }
}
