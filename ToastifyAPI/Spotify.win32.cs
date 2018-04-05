using JetBrains.Annotations;

#if !WIN_10

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        [CanBeNull]
        private static string GetSpotifyPath_platform()
        {
            return GetSpotifyPath_common();
        }
    }
}

#endif