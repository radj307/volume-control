using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        PlayPause = 917504,
        Mute = 524288,
        VolumeDown = 589824,
        VolumeUp = 655360,
        Stop = 851968,
        PreviousTrack = 786432,
        NextTrack = 720896
    }

    class Spotify
    {
        private static IntPtr GetSpotify()
        {
            return Win32.FindWindow("SpotifyMainWindow", null);
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

            switch (a)
            {
                case SpotifyAction.CopyTrackInfo:
                case SpotifyAction.ShowToast:
                    //Nothing
                    break;
                case SpotifyAction.ShowSpotify:
                    ShowSpotify();
                    break;
                default:
                    Win32.SendMessage(GetSpotify(), Win32.Constants.WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)a));
                    break;
            }
        }
    }
}
