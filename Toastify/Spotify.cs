using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace Toastify
{
    internal class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal class Constants
        {
            internal const uint WM_APPCOMMAND = 0x0319;

            internal const int SW_SHOWMINIMIZED = 2;
            internal const int SW_SHOW = 5;
            internal const int SW_RESTORE = 9;

            internal const int WM_CLOSE = 0x10;
            internal const int WM_QUIT = 0x12;
        }
    }

    public enum SpotifyAction : long
    {
        None = 0,
        ShowToast = 1,
        ShowSpotify = 2,
        CopyTrackInfo = 3,
        SettingsSaved = 4,
        PasteTrackInfo = 5,
        PlayPause = 917504,
        Mute = 524288,
        VolumeDown = 589824,
        VolumeUp = 655360,
        Stop = 851968,
        PreviousTrack = 786432,
        NextTrack = 720896,
        FastForward = 49 << 16,
        Rewind = 50 << 16,
    }

    class Song
    {
        public string Artist { get; set; }
        public string Title { get; set; }

        public override string ToString()
        {
            if (Artist == null)
                return Title;
            return string.Format("{0} - {1}", Artist, Title);
        }
    }

    static class Spotify
    {
        private static AutoHotkey.Interop.AutoHotkeyEngine _ahk;
        private static ChromeDriver _spotifyDriver;

        public static void StartSpotify()
        {
            string spotifyPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;  //string.Empty = (Default) value

            // try in the secondary location
            if (string.IsNullOrEmpty(spotifyPath))
            {
                spotifyPath = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;  //string.Empty = (Default) value
            }

            if (string.IsNullOrEmpty(spotifyPath))
            {
                throw new ArgumentException("Could not find spotify path in registry");
            }

            if (_spotifyDriver != null)
            {
                _spotifyDriver.Close();

                // wait for Spotify to close
                Thread.Sleep(1000);
            }

            KillSpotify();
            KillChromeDriver();

            // connect to the window
            var options = new ChromeOptions();
            options.BinaryLocation = Path.Combine(spotifyPath, "spotify.exe");
            
            // create the ChromeDriver service manually so that we can hide the debug window
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            _spotifyDriver = new ChromeDriver(chromeDriverService, options);

            if (SettingsXml.Current.MinimizeSpotifyOnStartup)
            {
                var hWnd = Spotify.GetSpotify();

                Win32.ShowWindow(hWnd, Win32.Constants.SW_SHOWMINIMIZED);
            }

            // we need to let Spotify start up before interacting with it fully. 2 seconds is a relatively 
            // safe amount of time to wait, even if the pattern is gross.
            Thread.Sleep(2000);
        }

        private static void KillProc(string name)
        {
            // let's play nice and try to gracefully clear out all Sync processes
            var procs = System.Diagnostics.Process.GetProcessesByName(name);

            foreach (var proc in procs)
            {
                // lParam == Band Process Id, passed in below
                Win32.EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
                {
                    uint processId = 0;
                    Win32.GetWindowThreadProcessId(hWnd, out processId);

                    // Essentially: Find every hWnd associated with this process and ask it to go away
                    if (processId == (uint)lParam)
                    {
                        Win32.SendMessage(hWnd, Win32.Constants.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        Win32.SendMessage(hWnd, Win32.Constants.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                    }

                    return true;
                },
                (IntPtr)proc.Id);
            }

            // let everything calm down
            Thread.Sleep(1000);

            procs = System.Diagnostics.Process.GetProcessesByName(name);

            // ok, no more mister nice guy. Sadly.
            foreach (var proc in procs)
            {
                try
                {
                    proc.Kill();
                }
                catch { } // ignore exceptions (usually due to trying to kill non-existant child processes
            }
        }

        private static void KillChromeDriver()
        {
            KillProc("chromedriver");
        }

        public static void KillSpotify()
        {
            KillProc("spotify");
        }

        private static IntPtr GetSpotify()
        {
            var windowClassName = "SpotifyMainWindow";

            return Win32.FindWindow(windowClassName, null);
        }

        public static bool IsAvailable()
        {
            return (GetSpotify() != IntPtr.Zero);
        }

        public static string GetCurrentTrack()
        {
            if (!Spotify.IsAvailable())
                return string.Empty;

            string song = "";
            string artist = "";

            try
            {
                var links = _spotifyDriver.FindElementsByTagName("a");

                foreach (var link in links)
                {
                    if (string.IsNullOrEmpty(link.Text))
                        continue;

                    // TODO: could use CSS selectors?
                    var databind = link.GetAttribute("data-bind");

                    if (databind == null) continue;

                    System.Diagnostics.Debug.WriteLine(databind);

                    if (databind.Contains("href: trackURI"))
                    {
                        song = link.Text;
                    }
                    else if (databind.Contains("trackURI") && link.GetAttribute("href").Contains("artist"))
                    {
                        artist = link.Text;
                    }
                }

                return artist + " \u2013 " + song;
            }
            catch
            {
                // exceptions will occur if the Spotify content changes while it's being enumerated
                // for example, if the song occurs while we're looking for the song title
                return "";
            }
        }

        public static Song GetCurrentSong()
        {
            string title = GetCurrentTrack();

            string[] parts = title.Split('\u2013'); //Spotify uses an en dash to separate Artist and Title
            if (parts.Length < 1 || parts.Length > 2)
                return null;

            if (parts.Length == 1)
                return new Song { Title = parts[0].Trim() };
            else
            {
                return new Song
                {
                    Artist = parts[0].Trim(),
                    Title = parts[1].Trim()
                };
            }
        }

        public static string CurrentCoverImageUrl { get; set; }

        private static void ShowSpotify()
        {
            if (Spotify.IsAvailable())
            {
                var hWnd = Spotify.GetSpotify();

                // check Spotify's current window state
                var placement = new Win32.WINDOWPLACEMENT();
                Win32.GetWindowPlacement(hWnd, ref placement);

                int showCommand = Win32.Constants.SW_SHOW;

                // if Spotify is minimzed we need to send a restore so that the window
                // will come back exactly like it was before being minimized (i.e. maximized
                // or otherwise) otherwise if we call SW_RESTORE on a currently maximized window
                // then instead of staying maximized it will return to normal size.
                if (placement.showCmd == Win32.Constants.SW_SHOWMINIMIZED)
                {
                    showCommand = Win32.Constants.SW_RESTORE;
                }

                Win32.ShowWindow(hWnd, showCommand);
                
                Win32.SetForegroundWindow(hWnd);
                Win32.SetFocus(hWnd);
            }
        }

        public static void SendAction(SpotifyAction a)
        {
            if (!Spotify.IsAvailable())
                return;

            // bah. Because control cannot fall through cases we need to special case volume
            if (SettingsXml.Current.ChangeSpotifyVolumeOnly)
            {
                if (a == SpotifyAction.VolumeUp)
                {
                    VolumeHelper.IncrementVolume("Spotify");
                    return;
                }
                else if (a == SpotifyAction.VolumeDown)
                {
                    VolumeHelper.DecrementVolume("Spotify");
                    return;
                }
                else if (a == SpotifyAction.Mute)
                {
                    VolumeHelper.ToggleApplicationMute("Spotify");
                    return;
                }
            }

            switch (a)
            {
                case SpotifyAction.CopyTrackInfo:
                case SpotifyAction.ShowToast:
                    //Nothing
                    break;
                case SpotifyAction.ShowSpotify:
                    ShowSpotify();
                    break;
                case SpotifyAction.FastForward:

                    SendComplexKeys("+{Right}");
                    break;

                case SpotifyAction.Rewind:

                    SendComplexKeys("+{Left}");
                    break;

                default:
                    Win32.SendMessage(GetSpotify(), Win32.Constants.WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)a));
                    break;
            }
        }

        /// <summary>
        /// Some commands require sending keys directly to Spotify (for example, Fast Forward and Rewind which
        /// are not handled by Spotify). We can't inject keys directly with WM_KEYDOWN/UP since we need a keyboard
        /// hook to actually change the state of various modifier keys (for example, Shift + Right for Fast Forward).
        /// 
        /// AutoHotKey has that hook and can modify the state for us, so let's take advantge of it.
        /// </summary>
        /// <param name="keys"></param>
        private static void SendComplexKeys(string keys)
        {
            // Is this nicer? 
            // _ahk = _ahk ?? new AutoHotkey.Interop.AutoHotkeyEngine();

            // only initialize AHK when needed as it can be expensive (dll copy etc) if not actually needed
            if (_ahk == null)
            {
                _ahk = new AutoHotkey.Interop.AutoHotkeyEngine();
            }

            _ahk.ExecRaw("SetTitleMatchMode 2");

            _ahk.ExecRaw("DetectHiddenWindows, On");
            _ahk.ExecRaw("ControlSend, ahk_parent, " + keys + ", ahk_class SpotifyMainWindow");

            _ahk.ExecRaw("DetectHiddenWindows, Off");
        }
    }
}
