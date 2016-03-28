using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Net;
using Newtonsoft.Json;

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
            internal const int SW_SHOWMINNOACTIVE = 7;
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
        ThumbsUp = 6,   // not usable, left in for future (hopefully!)
        ThumbsDown = 7, // not usable, left in for future (hopefully!)
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

    public class Song
    {
        public string Artist { get; set; }
        public string Track { get; set; }
        public string Album { get; set; }

        public string CoverArtUrl { get; set; }

        public Song(string artist, string title, string album = null)
        {
            Artist = artist;
            Track = title;
            Album = album;
        }

        public override string ToString()
        {
            if (Artist == null)
                return Track;

            return string.Format("{0} - {1}", Artist, Track);
        }

        internal bool IsValid()
        {
            return (!string.IsNullOrEmpty(Artist) || !string.IsNullOrEmpty(Track));
        }

        public override bool Equals(object obj)
        {
            var target = obj as Song;

            if (target == null)
                return false;

            return (target.Artist == this.Artist && target.Track == this.Track);
        }

        // overriding GetHashCode is "required" when overriding Equals
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    static class Spotify
    {
        private static AutoHotkey.Interop.AutoHotkeyEngine _ahk;

        public static void StartSpotify()
        {
            if (IsRunning())
                return;

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

            // launch Spotify
            var spotifyExe = Path.Combine(spotifyPath, "spotify.exe");

            System.Diagnostics.Process.Start(spotifyExe);

            if (SettingsXml.Current.MinimizeSpotifyOnStartup)
            {
                Minimize();
            }
            else
            {

                // we need to let Spotify start up before interacting with it fully. 2 seconds is a relatively 
                // safe amount of time to wait, even if the pattern is gross. (Minimize() doesn't need it since
                // it waits for the Window to appear before minimizing)
                Thread.Sleep(2000);
            }
        }

        private static void Minimize()
        {
            var remainingSleep = 2000;

            IntPtr hWnd;

            // Since Minimize is often called during startup, the hWnd is often not created yet
            // wait a maximum of remainingSleep for it to appear and then minimize it if it did.
            while ((hWnd = Spotify.GetSpotify()) == IntPtr.Zero && remainingSleep > 0)
            {
                Thread.Sleep(100);
                remainingSleep -= 100;
            }

            if (hWnd != IntPtr.Zero)
            {
                // disgusting but sadly neccessary. Let Spotify initialize a bit before minimizing it
                // otherwise the window hides itself and doesn't respond to taskbar clicks.
                // I tried to work around this by waiting for the window size to initialize (via GetWindowRect)
                // but that didn't work, there is some internal initialization that needs to occur.
                Thread.Sleep(500);
                Win32.ShowWindow(hWnd, Win32.Constants.SW_SHOWMINIMIZED);
            }
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

        public static void KillSpotify()
        {
            KillProc("spotify");
        }

        private static IntPtr GetSpotify()
        {
            var windowClassName = "SpotifyMainWindow";

            return Win32.FindWindow(windowClassName, null);
        }

        public static bool IsRunning()
        {
            return (GetSpotify() != IntPtr.Zero);
        }

        public static Song GetCurrentSong()
        {
            if (!Spotify.IsRunning())
                return null;

            string song = "";
            string artist = "";
            string album = "";

            IntPtr hWnd = GetSpotify();
            int length = Win32.GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            Win32.GetWindowText(hWnd, sb, sb.Capacity);

            string title = sb.ToString();

            if (!string.IsNullOrWhiteSpace(title) && title != "Spotify")
            {
                // Unfortunately we don't have a great way to get the title from Spotify
                // so we need to do some gymnastics. 
                // Music played from an artist's page is usually in the format "artist - song" 
                // while music played from a playlist is often in the format "artist - song - album"
                // unfortunately this means that some songs that actually have a " - " in either the artist name
                // or in the song name will potentially display incorrectly
                var portions = title.Split(new string[] { " - " }, StringSplitOptions.None);

                song = (portions.Length > 1 ? portions[1] : null);
                artist = portions[0];
                album = (portions.Length > 2 ? string.Join(" ", portions.Skip(2).ToArray()) : null); // take everything else that's left

                return new Song(artist, song, album);
            }

            return null;
        }

        public static void SetCoverArt(Song song)
        {
            // probably an ad, don't bother looking for an image
            if (string.IsNullOrWhiteSpace(song.Track) || string.IsNullOrWhiteSpace(song.Artist))
                return;

            string imageUrl = null;
            string spotifyTrackSearchURL = null;
            var jsonResponse = string.Empty;

            try
            {
                // Spotify now have a full supported JSON-based web API that we can use to grab album art from tracks. Example URL:
                // https://api.spotify.com/v1/search?query=track%3A%22Eagle%22+artist%3Aabba&offset=0&type=track
                //
                // Documentation: https://developer.spotify.com/web-api/migration-guide/ (great overview of functionality, even though it's a comparison guide)

                // temporary workaround for https://github.com/spotify/web-api/issues/191
                // essentially '/' is considered illegal, so we replace it with ' ' which generally returns the correct result
                var artist = song.Artist.Replace('/', ' ');
                var track = song.Track.Replace('/', ' ');

                spotifyTrackSearchURL = "https://api.spotify.com/v1/search?q=track%3A%22" +
                                            Uri.EscapeDataString(track) +
                                            "%22+artist%3A%22" +
                                            Uri.EscapeDataString(artist) +
                                            "%22&type=track";

                using (var wc = new WebClient())
                {
                    jsonResponse += wc.DownloadString(spotifyTrackSearchURL);
                }

                dynamic spotifyTracks = JsonConvert.DeserializeObject(jsonResponse);

                // iterate through all of the images, finding the smallest ones. This is usually the last
                // one, but there is no guarantee in the docs.
                int smallestWidth = int.MaxValue;

                // TryGetValue doesn't seem to work, so we're doing this old school
                if (spotifyTracks != null &&
                    spotifyTracks.tracks != null &&
                    spotifyTracks.tracks.items != null &&
                    spotifyTracks.tracks.items.First != null &&
                    spotifyTracks.tracks.items.First.album != null &&
                    spotifyTracks.tracks.items.First.album.images != null)
                {
                    foreach (dynamic image in spotifyTracks.tracks.items.First.album.images)
                    {
                        if (image.width < smallestWidth)
                        {
                            imageUrl = image.url;
                            smallestWidth = image.width;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("Web Exception grabbing Spotify track art:\n" + e);

                var response = e.Response as HttpWebResponse;

                if (response != null)
                {
                    Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.NetworkError, 
                        "URL: " + spotifyTrackSearchURL + " \nError Code: " + response.StatusCode + " Dump: " + e.ToString());
                }
                else
                {
                    Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.NetworkError,
                        "URL: " + spotifyTrackSearchURL + " \n[No Response] Dump: " + e.ToString());
                }

                throw;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception grabbing Spotify track art:\n" + e);

                var jsonSubstring = (jsonResponse == null ? null :
                                        jsonResponse.Substring(0,
                                            jsonResponse.Length > 100 ? 100 : jsonResponse.Length));

                Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.ResponseError, "URL: " + spotifyTrackSearchURL + " \nType: " + e.GetType().Name + " JSON excerpt: " + jsonSubstring +" dump: " + e.ToString());

                throw;
            }

            song.CoverArtUrl = imageUrl;
        }

        public static string CurrentCoverImageUrl { get; set; }

        private static bool IsMinimized()
        {
            if (!Spotify.IsRunning())
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
            if (Spotify.IsRunning())
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
            if (!Spotify.IsRunning())
                return;

            // bah. Because control cannot fall through cases we need to special case volume
            if (SettingsXml.Current.ChangeSpotifyVolumeOnly)
            {
                if (a == SpotifyAction.VolumeUp)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeUp);

                    VolumeHelper.IncrementVolume("Spotify");
                    return;
                }
                else if (a == SpotifyAction.VolumeDown)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeDown);

                    VolumeHelper.DecrementVolume("Spotify");
                    return;
                }
                else if (a == SpotifyAction.Mute)
                {
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Mute);

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
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.ShowSpotify);


                    if (Spotify.IsMinimized())
                    {
                        ShowSpotify();
                    }
                    else
                    {
                        Minimize();
                    }

                    break;
                case SpotifyAction.FastForward:

                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.FastForward);

                    SendComplexKeys("+{Right}");
                    break;

                case SpotifyAction.Rewind:

                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Rewind);

                    SendComplexKeys("+{Left}");
                    break;

                default:

                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Default + a.ToString());

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
