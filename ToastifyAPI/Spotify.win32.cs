#if !WIN_10

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        private static string GetSpotifyPath_platform()
        {
            return GetSpotifyPath_common();
        }
    }
}

#endif