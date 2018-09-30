using System.Runtime.InteropServices;
using System.Text;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI
{
    public static class Utils
    {
        #region Static Members

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetRedirectUri")]
        private static extern void _GetRedirectUri([Out] [MarshalAs(UnmanagedType.LPStr)] StringBuilder redirectUri, [In] int maxLength);

        [DllImport("ToastifyWebAuthAPI.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Try")]
        private static extern void _Try();

        public static string GetRedirectUri()
        {
            StringBuilder sb = new StringBuilder(128);
            _GetRedirectUri(sb, 128);
            return sb.ToString();
        }

        public static bool Try()
        {
            try
            {
                _Try();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}