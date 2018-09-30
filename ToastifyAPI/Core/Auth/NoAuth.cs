using System.Threading.Tasks;

namespace ToastifyAPI.Core.Auth
{
    public class NoAuth : ISpotifyWebAuth
    {
        public Task<IToken> GetToken()
        {
            return Task.FromResult<IToken>(null);
        }
    }
}