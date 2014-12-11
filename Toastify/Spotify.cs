using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Toastify
{
    internal class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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

        internal class Constants
        {
            internal const uint WM_APPCOMMAND = 0x0319;
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

    class Spotify
    {
        private static AutoHotkey.Interop.AutoHotkeyEngine _ahk;

        private static IntPtr GetSpotify()
        {
            var windowClassName = (SettingsXml.Current.UseSpotifyBeta ? "SpotifyMainWindow" : "SpotifyBetaMainWindow");

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

            IntPtr hWnd = GetSpotify();
            int length = Win32.GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            Win32.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString().Replace("Spotify", "").TrimStart(' ', '-').Trim();
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
                Win32.ShowWindow(Spotify.GetSpotify(), 1);
                Win32.SetForegroundWindow(Spotify.GetSpotify());
                Win32.SetFocus(Spotify.GetSpotify());
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
