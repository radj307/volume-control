#if WIN_10

using Windows.ApplicationModel;

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        private static string GetSpotifyPath_platform()
        {
            Package spotifyPackage = Win32API.FindPackage("SpotifyAB.SpotifyMusic");
            return spotifyPackage != null ? $@"shell:AppsFolder\{spotifyPackage.Id.FamilyName}!Spotify" : null;
        }
    }
}

#endif