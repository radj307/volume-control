using System;

namespace ToastifyAPI.Native.RawInputAPI.Enums
{
    /// <summary>
    ///     Enumeration containing the button data for raw mouse input.
    /// </summary>
    [Flags]
    public enum RawMouseButtons : ushort
    {
        /// <summary>
        ///     No button.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Left/Button1 down.
        /// </summary>
        LeftDown = 0x0001,

        /// <summary>
        ///     Left/Button1 up.
        /// </summary>
        LeftUp = 0x0002,

        /// <summary>
        ///     Right/Button2 down.
        /// </summary>
        RightDown = 0x0004,

        /// <summary>
        ///     Right/Button2 up.
        /// </summary>
        RightUp = 0x0008,

        /// <summary>
        ///     Middle/Button3 down.
        /// </summary>
        MiddleDown = 0x0010,

        /// <summary>
        ///     Middle/Button3 up.
        /// </summary>
        MiddleUp = 0x0020,

        /// <summary>
        ///     XButton1/Button4 down.
        /// </summary>
        XButton1Down = 0x0040,

        /// <summary>
        ///     XButton1/Button4 up.
        /// </summary>
        XButton1Up = 0x0080,

        /// <summary>
        ///     XButton2/Button5 down.
        /// </summary>
        XButton2Down = 0x0100,

        /// <summary>
        ///     XButton2/Button5 up.
        /// </summary>
        XButton2Up = 0x0200,

        /// <summary>
        ///     Mouse wheel moved.
        /// </summary>
        MouseWheel = 0x0400
    }
}