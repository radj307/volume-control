using System;
using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DataBlob
    {
        public int cbData;
        public IntPtr pbData;
    }
}