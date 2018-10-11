using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Contains the raw input from a device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawInput
    {
        public RawInputHeader Header;
        public RawInputData Data;
    }
}