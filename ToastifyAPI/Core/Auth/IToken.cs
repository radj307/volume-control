using System;

namespace ToastifyAPI.Core.Auth
{
    public interface IToken : IEquatable<IToken>
    {
        #region Public Properties

        string AccessToken { get; }
        string TokenType { get; }
        double ExpiresIn { get; }
        string RefreshToken { get; }
        DateTime CreateDate { get; }

        #endregion

        bool IsExpired();
    }
}