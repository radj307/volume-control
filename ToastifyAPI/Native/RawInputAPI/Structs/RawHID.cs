using System;
using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Contains information about the state of the HID.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawHID
    {
        public int Size;
        public int Count;
        public IntPtr Data;
    }
}