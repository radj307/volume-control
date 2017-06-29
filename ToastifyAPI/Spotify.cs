using Microsoft.Win32;
using System;
using System.IO;

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        public static string GetSpotifyPath()
        {
            string spotifyPath = GetSpotifyPath_common() ?? GetSpotifyPath_platform();

            if (string.IsNullOrEmpty(spotifyPath))
                throw new ArgumentException("Could not find spotify path.");

            return spotifyPath;
        }

        private static string GetSpotifyPath_common()
        {
            string spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;

            // Try in the secondary location.
            if (string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;

            if (!string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Path.Combine(spotifyPath, "Spotify.exe");

            return spotifyPath;
        }
    }
}