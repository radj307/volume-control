using ToastifyAPI.Core;
using ToastifyAPI.Core.Auth;

namespace Toastify.Core
{
    public class SpotifyWeb : ISpotifyWeb
    {
        #region Public Properties

        public ISpotifyWebAuth Auth { get; }
        public ISpotifyWebAPI API { get; }

        #endregion

        public SpotifyWeb(ISpotifyWebAuth auth, ISpotifyWebAPI webApi)
        {
            this.Auth = auth;
            this.API = webApi;
        }
    }
}