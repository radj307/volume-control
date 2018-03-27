using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags]
    public enum ExecutionStateFlags : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS        = 0x80000000,
        ES_DISPLAY_REQUIRED  = 0x00000002,
        ES_SYSTEM_REQUIRED   = 0x00000001,
        ES_USER_PRESENT      = 0x00000004 // Legacy flag, should not be used.
    }
}