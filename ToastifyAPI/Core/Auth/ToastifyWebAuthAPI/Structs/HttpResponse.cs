using System.Runtime.InteropServices;
using System.Text;

namespace ToastifyAPI.Core.Auth.ToastifyWebAuthAPI.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HttpResponse
    {
        public int status;

        [MarshalAs(UnmanagedType.LPStr)]
        public string error;

        [MarshalAs(UnmanagedType.LPStr)]
        public string body;

        public int maxErrorLength;
        public int maxBodyLength;

        public HttpResponse(int maxErrorLength, int maxBodyLength)
        {
            this.status = 0;
            this.error = new StringBuilder(maxErrorLength).Append((char)0, maxErrorLength).ToString();
            this.body = new StringBuilder(maxBodyLength).Append((char)0, maxBodyLength).ToString();
            this.maxErrorLength = maxErrorLength;
            this.maxBodyLength = maxBodyLength;
        }
    }
}