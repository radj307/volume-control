using System;
using System.Threading.Tasks;

namespace ToastifyAPI.Core.Auth
{
    /// <summary>
    ///     Interface for an HTTP server that should listen for the response of the /authorize endpoint of Spotify's Account service
    /// </summary>
    public interface IAuthHttpServer
    {
        #region Events

        event EventHandler<AuthEventArgs> AuthorizationFinished;

        #endregion

        Task Start();
        Task Stop();
    }
}