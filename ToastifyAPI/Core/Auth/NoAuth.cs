using System.Threading.Tasks;

namespace ToastifyAPI.Core.Auth
{
    public class NoAuth : ISpotifyWebAuth
    {
        public Task<IToken> GetToken()
        {
            return Task.FromResult<IToken>(null);
        }

        public Task<IToken> RefreshToken(IToken token)
        {
            return Task.FromResult<IToken>(null);
        }
    }
}