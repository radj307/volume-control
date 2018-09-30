using SpotifyAPI.Web.Models;
using Toastify.Model;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Core
{
    public class SpotifyWebAPI : ISpotifyWebAPI
    {
        #region Public Properties

        public SpotifyAPI.Web.SpotifyWebAPI SpotifyWebApi { get; set; }

        #endregion

        public ICurrentlyPlayingObject GetCurrentlyPlayingTrack()
        {
            PlaybackContext playbackContext = this.SpotifyWebApi?.GetPlayingTrack();
            return playbackContext != null ? new CurrentlyPlayingObject(playbackContext) : null;
        }

        public ISpotifyUserProfile GetUserPrivateProfile()
        {
            PrivateProfile profile = this.SpotifyWebApi?.GetPrivateProfile();
            return profile != null ? new SpotifyUserProfile(profile) : null;
        }
    }
}