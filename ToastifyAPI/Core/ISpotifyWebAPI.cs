using System.Threading.Tasks;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Core
{
    public interface ISpotifyWebAPI
    {
        #region Public Properties

        IToken Token { get; set; }

        #endregion

        ICurrentlyPlayingObject GetCurrentlyPlayingTrack();
        ISpotifyUserProfile GetUserPrivateProfile();

        Task<ICurrentlyPlayingObject> GetCurrentlyPlayingTrackAsync();
        Task<ISpotifyUserProfile> GetUserPrivateProfileAsync();
    }
}