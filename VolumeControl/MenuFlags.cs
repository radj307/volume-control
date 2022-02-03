using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags]
    public enum MenuFlags : long
    {
        MF_BYCOMMAND  = 0x0000L,
        MF_ENABLED    = 0x0000L,
        MF_GRAYED     = 0x0001L,
        MF_DISABLED   = 0x0002L,
        MF_BYPOSITION = 0x0400L
    }
}