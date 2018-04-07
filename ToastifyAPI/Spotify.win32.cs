#if !WIN_10
using JetBrains.Annotations;

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        [CanBeNull]
        private static string GetSpotifyPath_platform()
        {
            // noop, this is taken care of in the GetSpotifyPath_common()
            return null;
        }
    }
}

#endif