using System;

namespace ToastifyAPI.Native.RawInputAPI.Enums
{
    /// <summary>
    ///     Enumeration containing flags for raw keyboard input.
    /// </summary>
    [Flags]
    public enum RawKeyboardFlags : ushort
    {
        KeyMake = 0x00,
        KeyBreak = 0x01,
        KeyE0 = 0x02,
        KeyE1 = 0x04,
        TerminalServerSetLED = 0x08,
        TerminalServerShadow = 0x10,
        TerminalServerVkPacket = 0x20
    }
}