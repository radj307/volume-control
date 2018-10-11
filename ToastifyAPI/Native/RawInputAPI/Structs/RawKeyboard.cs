using System.Runtime.InteropServices;
using System.Windows.Forms;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.RawInputAPI.Enums;

namespace ToastifyAPI.Native.RawInputAPI.Structs
{
    /// <summary>
    ///     Value type for raw input from a keyboard.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawKeyboard
    {
        /// <summary>
        ///     Scan code for key depression.
        /// </summary>
        public short MakeCode;

        /// <summary>
        ///     Scan code information.
        /// </summary>
        public RawKeyboardFlags Flags;

        /// <summary>
        ///     Reserved.
        /// </summary>
        public short Reserved;

        /// <summary>
        ///     Virtual key code.
        /// </summary>
        public Keys VirtualKey;

        /// <summary>
        ///     Corresponding window message.
        /// </summary>
        public WindowsMessagesFlags Message;

        /// <summary>
        ///     Extra information.
        /// </summary>
        public int ExtraInformation;
    }
}