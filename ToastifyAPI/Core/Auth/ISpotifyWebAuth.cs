using System.Threading.Tasks;

namespace ToastifyAPI.Core.Auth
{
    public interface ISpotifyWebAuth
    {
        Task<IToken> GetToken();
    }
}