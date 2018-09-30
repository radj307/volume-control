using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Core
{
    public interface ISpotifyWebAPI
    {
        ICurrentlyPlayingObject GetCurrentlyPlayingTrack();
        ISpotifyUserProfile GetUserPrivateProfile();
    }
}