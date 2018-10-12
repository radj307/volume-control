using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ICurrentlyPlayingObject
    {
        int ProgressMs { get; }
        bool IsPlaying { get; }
        ISpotifyTrack Track { get; }
        SpotifyTrackType Type { get; }
    }
}