using JetBrains.Annotations;
using ToastifyAPI.Core.Auth;
using SpotifyAPIWebToken = SpotifyAPI.Web.Models.Token;

namespace Toastify.Core.Auth
{
    public class Token : SpotifyAPIWebToken, IToken
    {
        internal Token()
        {
        }

        public Token([NotNull] IToken token)
        {
            this.AccessToken = token.AccessToken;
            this.TokenType = token.TokenType;
            this.ExpiresIn = token.ExpiresIn;
            this.RefreshToken = token.RefreshToken;
            this.CreateDate = token.CreateDate;
        }
    }
}