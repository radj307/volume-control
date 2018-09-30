using System;

namespace ToastifyAPI.Core.Auth
{
    /// <summary>
    ///     Response of the /authorize endpoint of Spotify's Accounts service
    /// </summary>
    public class AuthEventArgs : EventArgs
    {
        #region Public Properties

        public string Code { get; }
        public string State { get; }
        public string Error { get; }

        #endregion

        public AuthEventArgs(string code, string state, string error)
        {
            this.Code = code;
            this.State = state;
            this.Error = error;
        }
    }
}