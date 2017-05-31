using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Toastify.Core;
using Toastify.Helpers;
using Toastify.Services;

namespace Toastify
{
    internal static class Spotify
    {
        private static AutoHotkey.Interop.AutoHotkeyEngine _ahk;

        public static string CurrentCoverImageUrl { get; set; }

        public static void StartSpotify()
        {
            if (IsRunning())
                return;

            string spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;

            // try in the secondary location
            if (string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;

            if (string.IsNullOrEmpty(spotifyPath))
                throw new ArgumentException("Could not find spotify path in registry");

            // launch Spotify
            var spotifyExe = Path.Combine(spotifyPath, "spotify.exe");

            Process.Start(spotifyExe);

            if (SettingsXml.Instance.MinimizeSpotifyOnStartup)
                Minimize();
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
            while ((hWnd = GetSpotify()) == IntPtr.Zero && remainingSleep > 0)
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
                Win32API.ShowWindow(hWnd, Win32API.Constants.SW_SHOWMINIMIZED);
            }
        }

        private static void KillProc(string name)
        {
            // let's play nice and try to gracefully clear out all Sync processes
            var procs = Process.GetProcessesByName(name);

            foreach (var proc in procs)
            {
                // lParam == Band Process Id, passed in below
                Win32API.EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                {
                    Win32API.GetWindowThreadProcessId(hWnd, out uint processId);

                    // Essentially: Find every hWnd associated with this process and ask it to go away
                    if (processId == (uint)lParam)
                    {
                        Win32API.SendMessage(hWnd, Win32API.Constants.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        Win32API.SendMessage(hWnd, Win32API.Constants.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                    }

                    return true;
                },
                (IntPtr)proc.Id);
            }

            // let everything calm down
            Thread.Sleep(1000);

            procs = Process.GetProcessesByName(name);

            // ok, no more mister nice guy. Sadly.
            foreach (var proc in procs)
            {
                try
                {
                    proc.Kill();
                }
                catch
                {
                    // ignore exceptions (usually due to trying to kill non-existant child processes
                }
            }
        }

        public static void KillSpotify()
        {
            KillProc("spotify");
        }

        private static IntPtr GetSpotify()
        {
            return Win32API.FindWindow("SpotifyMainWindow", null);
        }

        public static bool IsRunning()
        {
            return GetSpotify() != IntPtr.Zero;
        }

        public static Song GetCurrentSong()
        {
            if (!IsRunning())
                return null;

            IntPtr hWnd = GetSpotify();
            int length = Win32API.GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            Win32API.GetWindowText(hWnd, sb, sb.Capacity);

            string title = sb.ToString();

            if (!string.IsNullOrWhiteSpace(title) && title != "Spotify")
            {
                // Unfortunately we don't have a great way to get the title from Spotify
                // so we need to do some gymnastics.
                // Music played from an artist's page is usually in the format "artist - song"
                // while music played from a playlist is often in the format "artist - song - album"
                // unfortunately this means that some songs that actually have a " - " in either the artist name
                // or in the song name will potentially display incorrectly
                var portions = title.Split(new[] { " - " }, StringSplitOptions.None);

                string song = portions.Length > 1 ? portions[1] : null;
                string artist = portions[0];
                string album = portions.Length > 2 ? string.Join(" ", portions.Skip(2).ToArray()) : null;

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
                // Spotify now has a full supported JSON-based web API that we can use to grab album art from tracks. Example URL:
                // https://api.spotify.com/v1/search?query=track%3A%22Eagle%22+artist%3Aabba&offset=0&type=track
                //
                // Documentation: https://developer.spotify.com/web-api/migration-guide/ (great overview of functionality, even though it's a comparison guide)

                // temporary workaround for https://github.com/spotify/web-api/issues/191
                // essentially '/' is considered illegal, so we replace it with ' ' which generally returns the correct result
                var artist = song.Artist.Replace('/', ' ');
                var track = song.Track.Replace('/', ' ');

                spotifyTrackSearchURL = $"https://api.spotify.com/v1/search?q=track%3A%22{Uri.EscapeDataString(track)}%22+artist%3A%22{Uri.EscapeDataString(artist)}%22&type=track";

                using (var wc = new WebClient())
                {
                    jsonResponse += wc.DownloadString(spotifyTrackSearchURL);
                }

                dynamic spotifyTracks = JsonConvert.DeserializeObject(jsonResponse);

                // iterate through all of the images, finding the smallest ones. This is usually the last
                // one, but there is no guarantee in the docs.
                int smallestWidth = int.MaxValue;

                // TryGetValue doesn't seem to work, so we're doing this old school
                if (spotifyTracks?.tracks?.items?.First?.album?.images != null)
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
                Debug.WriteLine($"Web Exception grabbing Spotify track art:\n{e}");

                if (e.Response is HttpWebResponse response)
                {
                    Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.NetworkError,
                        $"URL: {spotifyTrackSearchURL} \nError Code: {response.StatusCode} Dump: {e}");
                }
                else
                {
                    Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.NetworkError,
                        $"URL: {spotifyTrackSearchURL} \n[No Response] Dump: {e}");
                }

                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception grabbing Spotify track art:\n" + e);

                var jsonSubstring = jsonResponse.Substring(0, jsonResponse.Length > 100 ? 100 : jsonResponse.Length);

                Telemetry.TrackEvent(TelemetryCategory.SpotifyWebService, Telemetry.TelemetryEvent.SpotifyWebService.ResponseError,
                    $"URL: {spotifyTrackSearchURL} \nType: {e.GetType().Name} JSON excerpt: {jsonSubstring} dump: {e}");

                throw;
            }

            song.CoverArtUrl = imageUrl;
        }

        private static bool IsMinimized()
        {
            if (!IsRunning())
                return false;

            var hWnd = GetSpotify();

            // check Spotify's current window state
            var placement = new Win32API.WindowPlacement();
            Win32API.GetWindowPlacement(hWnd, ref placement);

            return placement.showCmd == Win32API.Constants.SW_SHOWMINIMIZED;
        }

        private static void ShowSpotifyWithNoActivate()
        {
            var hWnd = GetSpotify();

            // check Spotify's current window state
            var placement = new Win32API.WindowPlacement();
            Win32API.GetWindowPlacement(hWnd, ref placement);

            const Win32API.SetWindowPosFlags flags = Win32API.SetWindowPosFlags.DoNotActivate | Win32API.SetWindowPosFlags.DoNotChangeOwnerZOrder | Win32API.SetWindowPosFlags.ShowWindow;

            Win32API.SetWindowPos(hWnd, (IntPtr)0, placement.rcNormalPosition.Left, placement.rcNormalPosition.Top, 0, 0, flags);
        }

        private static void ShowSpotify()
        {
            if (IsRunning())
            {
                var hWnd = GetSpotify();

                // check Spotify's current window state
                var placement = new Win32API.WindowPlacement();
                Win32API.GetWindowPlacement(hWnd, ref placement);

                int showCommand = Win32API.Constants.SW_SHOW;

                // if Spotify is minimzed we need to send a restore so that the window
                // will come back exactly like it was before being minimized (i.e. maximized
                // or otherwise) otherwise if we call SW_RESTORE on a currently maximized window
                // then instead of staying maximized it will return to normal size.
                if (placement.showCmd == Win32API.Constants.SW_SHOWMINIMIZED)
                    showCommand = Win32API.Constants.SW_RESTORE;

                Win32API.ShowWindow(hWnd, showCommand);

                Win32API.SetForegroundWindow(hWnd);
                Win32API.SetFocus(hWnd);
            }
        }

        public static void SendAction(SpotifyAction a)
        {
            if (!IsRunning())
                return;

            // bah. Because control cannot fall through cases we need to special case volume
            if (SettingsXml.Instance.ChangeSpotifyVolumeOnly)
            {
                switch (a)
                {
                    case SpotifyAction.VolumeUp:
                        Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeUp);

                        VolumeHelper.IncrementVolume("Spotify");
                        return;
                    case SpotifyAction.VolumeDown:
                        Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.VolumeDown);

                        VolumeHelper.DecrementVolume("Spotify");
                        return;
                    case SpotifyAction.Mute:
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
                    if (IsMinimized())
                        ShowSpotify();
                    else
                        Minimize();
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
                    Telemetry.TrackEvent(TelemetryCategory.Action, Telemetry.TelemetryEvent.Action.Default + a);
                    Win32API.SendMessage(GetSpotify(), Win32API.Constants.WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)a));
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
                _ahk = new AutoHotkey.Interop.AutoHotkeyEngine();

            _ahk.ExecRaw("SetTitleMatchMode 2");

            _ahk.ExecRaw("DetectHiddenWindows, On");
            _ahk.ExecRaw("ControlSend, ahk_parent, " + keys + ", ahk_class SpotifyMainWindow");

            _ahk.ExecRaw("DetectHiddenWindows, Off");
        }
    }
}