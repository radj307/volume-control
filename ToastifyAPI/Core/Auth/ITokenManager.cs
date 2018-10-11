using System;
using System.Threading;
using ToastifyAPI.Events;

namespace ToastifyAPI.Core.Auth
{
    public interface ITokenManager : IDisposable
    {
        #region Public Properties

        IToken Token { get; }
        ManualResetEvent RefreshingTokenEvent { get; }

        #endregion

        #region Events

        event EventHandler<SpotifyTokenChangedEventArgs> TokenChanged;

        #endregion

        bool BeginGetToken(CancellationToken cancellationToken);
        bool BeginGetToken(CancellationToken cancellationToken, Action<IToken> callback);

        void ReleaseToken();
    }
}