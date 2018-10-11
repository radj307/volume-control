using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Value type for a raw input data.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RawInputData
    {
        /// <summary>
        ///     Mouse raw input data.
        /// </summary>
        [FieldOffset(0)]
        public RawMouse Mouse;

        /// <summary>
        ///     Keyboard raw input data.
        /// </summary>
        [FieldOffset(0)]
        public RawKeyboard Keyboard;

        /// <summary>
        ///     HID raw input data.
        /// </summary>
        [FieldOffset(0)]
        public RawHID HID;
    }
}