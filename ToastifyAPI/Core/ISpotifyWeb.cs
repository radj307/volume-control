using ToastifyAPI.Core.Auth;

namespace ToastifyAPI.Core
{
    public interface ISpotifyWeb
    {
        #region Public Properties

        ISpotifyWebAuth Auth { get; }

        ISpotifyWebAPI WebAPI { get; }

        #endregion
    }
}