using System;
using System.Threading.Tasks;

namespace ToastifyAPI.Core.Auth
{
    public interface ISpotifyWebAuth : IDisposable
    {
        Task<IToken> GetToken();
        Task<IToken> RefreshToken(IToken token);
    }
}