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
using OpenQA.Selenium;

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

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [Flags()]
        internal enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

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
            internal const int SW_SHOWNOACTIVATE = 4;
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
        ThumbsUp = 6,
        ThumbsDown = 7,
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
                Minimize();
            }

            // we need to let Spotify start up before interacting with it fully. 2 seconds is a relatively 
            // safe amount of time to wait, even if the pattern is gross.
            Thread.Sleep(2000);
        }

        private static void Minimize()
        {
            var hWnd = Spotify.GetSpotify();

            Win32.ShowWindow(hWnd, Win32.Constants.SW_SHOWMINIMIZED);
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

                return artist + " - " + song;
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

            string[] parts = title.Split('-');
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

        private static bool IsMinimized()
        {
            if (!Spotify.IsAvailable())
                return false;

            var hWnd = Spotify.GetSpotify();

            // check Spotify's current window state
            var placement = new Win32.WINDOWPLACEMENT();
            Win32.GetWindowPlacement(hWnd, ref placement);

            return (placement.showCmd == Win32.Constants.SW_SHOWMINIMIZED);
        }

        private static void ShowSpotifyWithNoActivate()
        {
            var hWnd = Spotify.GetSpotify();

            // check Spotify's current window state
            var placement = new Win32.WINDOWPLACEMENT();
            Win32.GetWindowPlacement(hWnd, ref placement);

            var flags = Win32.SetWindowPosFlags.DoNotActivate | Win32.SetWindowPosFlags.DoNotChangeOwnerZOrder | Win32.SetWindowPosFlags.ShowWindow;

            Win32.SetWindowPos(hWnd, (IntPtr)0, placement.rcNormalPosition.Left, placement.rcNormalPosition.Top, 0, 0, flags);
        }

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

        private static void ThumbsUp()
        {
            ClickThumb("spoticon-thumbs-up-32");
        }

        private static void ThumbsDown()
        {
            ClickThumb("spoticon-thumbs-down-32");
        }

        private static void ClickThumb(string thumbClass)
        {
            var needToMinimizeSpotify = false;

            if (IsMinimized())
            {
                ShowSpotifyWithNoActivate();
                needToMinimizeSpotify = true;
            }

            // check if we're already in the radio page
            try
            {
                IWebElement iframeRadio;

                var navigatedToRadio = false;

                // this convuluted piece of code allows you to thumb up songs even if you haven't
                // ever launched the radio window which can happen if you start Toastify -> the last thing
                // you were doing in Spotify was playing radio -> press play. It will also catch the state 
                // where app-radio is not the active frame.
                try
                {
                    iframeRadio = _spotifyDriver.FindElementById("app-radio");

                    if (!iframeRadio.GetAttribute("class").Contains("active"))
                        navigatedToRadio = true;
                }
                catch (NoSuchElementException)
                {
                    navigatedToRadio = true;
                }

                if (navigatedToRadio)
                {
                    _spotifyDriver.FindElementById("menu-item-radio").Click();

                    // initiate the navigation
                    Thread.Sleep(500);

                    iframeRadio = _spotifyDriver.FindElementById("app-radio");
                }

                _spotifyDriver.SwitchTo().Frame("app-radio");

                // wait max 2 seconds for the element to appear
                var waitRemaining = 2000;
                IWebElement thumb = null;

                while (thumb == null)
                {
                    Thread.Sleep(200);
                    waitRemaining -= 200;

                    try
                    {
                        thumb = _spotifyDriver.FindElementByClassName(thumbClass);
                    }
                    catch (NoSuchElementException) { }

                    if (waitRemaining < 0)
                        break;
                }

                if (thumb != null && thumb.Enabled && !thumb.GetAttribute("class").Contains("disabled"))
                    thumb.Click();

                // reset frame state
                _spotifyDriver.SwitchTo().ParentFrame();

                // if we navigated, then restore state by going back one in the backstack
                if (navigatedToRadio)
                    _spotifyDriver.FindElementByClassName("spoticon-chevron-left-16").Click();
            }
            catch (NoSuchElementException)
            {
                // radio isn't playing... move on
                
                // reset frame state in the case of an exception
                _spotifyDriver.SwitchTo().ParentFrame();
            }

            if (needToMinimizeSpotify)
                Minimize();
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

                    if (Spotify.IsMinimized())
                    {
                        ShowSpotify();
                    }
                    else
                    {
                        Minimize();
                    }

                    break;
                case SpotifyAction.ThumbsUp:
                    ThumbsUp();
                    break;
                case SpotifyAction.ThumbsDown:
                    ThumbsDown();
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
