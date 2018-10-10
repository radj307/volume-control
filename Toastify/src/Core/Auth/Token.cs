using System;
using JetBrains.Annotations;
using ToastifyAPI.Core.Auth;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;
using SpotifyAPIWebToken = SpotifyAPI.Web.Models.Token;

namespace Toastify.Core.Auth
{
    [Serializable]
    public class Token : IToken, IEquatable<Token>
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

        #region Equality Members

        public bool Equals(IToken other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(this.AccessToken, other.AccessToken) &&
                   string.Equals(this.TokenType, other.TokenType) &&
                   this.ExpiresIn.Equals(other.ExpiresIn) &&
                   string.Equals(this.RefreshToken, other.RefreshToken) &&
                   this.CreateDate.Equals(other.CreateDate);
        }

        public bool Equals(Token other)
        {
            return this.Equals((IToken)other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() &&
                   this.Equals((Token)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.AccessToken != null ? this.AccessToken.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.TokenType != null ? this.TokenType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.ExpiresIn.GetHashCode();
                hashCode = (hashCode * 397) ^ (this.RefreshToken != null ? this.RefreshToken.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.CreateDate.GetHashCode();
                return hashCode;
            }
        }

        #endregion
    }
}