using ToastifyAPI.Core.Auth;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Core
{
    public interface ISpotifyWebAPI
    {
        IToken Token { get; set; }

        ICurrentlyPlayingObject GetCurrentlyPlayingTrack();
        ISpotifyUserProfile GetUserPrivateProfile();
    }
}