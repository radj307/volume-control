using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpotifyTokenResponse
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string accessToken;

        [MarshalAs(UnmanagedType.LPStr)]
        public string tokenType;

        [MarshalAs(UnmanagedType.LPStr)]
        public string scope;

        public int expiresIn;

        [MarshalAs(UnmanagedType.LPStr)]
        public string refreshToken;

        public int maxTokenLength;
        public int maxTokenTypeLength;
        public int maxScopeLength;

        #region Public Properties

        public DateTime CreationDate { get; set; }

        #endregion

        public SpotifyTokenResponse(int maxTokenLength, int maxTokenTypeLength, int maxScopeLength)
        {
            this.accessToken = new StringBuilder(maxTokenLength).Append((char)0, maxTokenLength).ToString();
            this.tokenType = new StringBuilder(maxTokenTypeLength).Append((char)0, maxTokenTypeLength).ToString();
            this.scope = new StringBuilder(maxScopeLength).Append((char)0, maxScopeLength).ToString();
            this.expiresIn = -1;
            this.refreshToken = new StringBuilder(maxTokenLength).Append((char)0, maxTokenLength).ToString();
            this.maxTokenLength = maxTokenLength;
            this.maxTokenTypeLength = maxTokenTypeLength;
            this.maxScopeLength = maxScopeLength;

            this.CreationDate = DateTime.Now;
        }
    }
}