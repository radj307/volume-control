using System.Runtime.InteropServices;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI
{
    public static class ClientCredentialsFlow
    {
        #region Static Members

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetClientCredentialsToken(
            [In, Out] ref HttpResponse response,
            [In, Out] ref SpotifyTokenResponse tokenResponse);

        #endregion
    }
}