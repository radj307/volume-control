using System.Runtime.InteropServices;
using ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI
{
    public static class AuthorizationCodeFlow
    {
        #region Static Members

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Authorize(
            [In, Optional] [MarshalAs(UnmanagedType.LPStr)] string scope,
            [In, Optional] [MarshalAs(UnmanagedType.LPStr)] string state,
            [In, Optional] [MarshalAs(UnmanagedType.Bool)] bool showDialog);

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetAuthorizationToken(
            [In, Out] ref HttpResponse response,
            [In, Out] ref SpotifyTokenResponse tokenResponse,
            [In] [MarshalAs(UnmanagedType.LPStr)] string code);

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RefreshAuthorizationToken(
            [In, Out] ref HttpResponse response,
            [In, Out] ref SpotifyTokenResponse tokenResponse,
            [In] [MarshalAs(UnmanagedType.LPStr)] string refreshToken);

        #endregion
    }
}