using System.Runtime.InteropServices;
using ToastifyAPI.Native.RawInputAPI.Enums;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Contains information about the state of the mouse.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RawMouse
    {
        /// <summary>
        ///     The mouse state.
        /// </summary>
        [FieldOffset(0)]
        public RawMouseFlags Flags;

        /// <summary>
        ///     Flags for the event.
        /// </summary>
        [FieldOffset(4)]
        public RawMouseButtons ButtonFlags;

        /// <summary>
        ///     If the mouse wheel is moved, this will contain the delta amount.
        /// </summary>
        [FieldOffset(6)]
        public ushort ButtonData;

        /// <summary>
        ///     Raw button data.
        /// </summary>
        [FieldOffset(8)]
        public uint RawButtons;

        /// <summary>
        ///     The motion in the X direction. This is signed relative motion
        ///     or absolute motion, depending on the value of usFlags.
        /// </summary>
        [FieldOffset(12)]
        public int LastX;

        /// <summary>
        ///     The motion in the Y direction. This is signed relative motion or absolute motion,
        ///     depending on the value of usFlags.
        /// </summary>
        [FieldOffset(16)]
        public int LastY;

        /// <summary>
        ///     The device-specific additional information for the event.
        /// </summary>
        [FieldOffset(20)]
        public uint ExtraInformation;
    }
}