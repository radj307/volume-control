using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.RawInputAPI.Enums;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Value type for a raw input header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputHeader
    {
        /// <summary>
        ///     Type of device the input is coming from.
        /// </summary>
        public RawInputType Type;

        /// <summary>
        ///     Size of the packet of data.
        /// </summary>
        public int Size;

        /// <summary>
        ///     Handle to the device sending the data.
        /// </summary>
        public IntPtr Device;

        /// <summary>
        ///     wParam from the window message.
        /// </summary>
        public IntPtr wParam;
    }
}