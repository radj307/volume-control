using ToastifyAPI.Core;

namespace Toastify.Core
{
    public class SpotifyWebAPI : ISpotifyWebAPI
    {
        #region Public Properties

        public SpotifyAPI.Web.SpotifyWebAPI SpotifyWebApi { get; set; }

        #endregion

        public object GetCurrentTrack()
        {
            var playbackContext = this.SpotifyWebApi?.GetPlayingTrack();
            return playbackContext;
        }

        public object GetUserPrivateProfile()
        {
            var profile = this.SpotifyWebApi?.GetPrivateProfile();
            return profile;
        }
    }
}