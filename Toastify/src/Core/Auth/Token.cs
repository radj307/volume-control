using System;
using JetBrains.Annotations;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;
using SpotifyAPIWebToken = SpotifyAPI.Web.Models.Token;

namespace Toastify.Core.Auth
{
    [Serializable]
    public class Token : IToken
    {
        #region Public Properties

        public string AccessToken { get; }
        public string TokenType { get; }
        public double ExpiresIn { get; }
        public string RefreshToken { get; }
        public DateTime CreateDate { get; }

        #endregion

        internal Token()
        {
        }

        public Token(SpotifyTokenResponse token)
        {
            this.AccessToken = token.accessToken;
            this.TokenType = token.tokenType;
            this.ExpiresIn = token.expiresIn;
            this.RefreshToken = token.refreshToken;
            this.CreateDate = token.CreationDate;
        }

        public Token([NotNull] SpotifyAPIWebToken token)
        {
            this.AccessToken = token.AccessToken;
            this.TokenType = token.TokenType;
            this.ExpiresIn = token.ExpiresIn;
            this.RefreshToken = token.RefreshToken;
            this.CreateDate = token.CreateDate;
        }

        public Token([NotNull] IToken token)
        {
            this.AccessToken = token.AccessToken;
            this.TokenType = token.TokenType;
            this.ExpiresIn = token.ExpiresIn;
            this.RefreshToken = token.RefreshToken;
            this.CreateDate = token.CreateDate;
        }

        public bool IsExpired()
        {
            return this.CreateDate.Add(TimeSpan.FromSeconds(this.ExpiresIn)) <= DateTime.Now;
        }

        #region Static Members

        public static implicit operator SpotifyAPIWebToken(Token token)
        {
            return new SpotifyAPIWebToken
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                ExpiresIn = token.ExpiresIn,
                RefreshToken = token.RefreshToken,
                CreateDate = token.CreateDate
            };
        }

        #endregion
    }
}