using System;
using ToastifyAPI.Core.Auth;

namespace ToastifyAPI.Events
{
    public class SpotifyTokenChangedEventArgs : EventArgs
    {
        #region Public Properties

        public IToken OldToken { get; }
        public IToken NewToken { get; }

        #endregion

        public SpotifyTokenChangedEventArgs(IToken oldToken, IToken newToken)
        {
            this.OldToken = oldToken;
            this.NewToken = newToken;
        }
    }
}