using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags] 
    public enum ShGSI : uint 
    { 
        SHGSI_ICONLOCATION  = 0, 
        SHGSI_ICON          = 0x000000100, 
        SHGSI_SYSICONINDEX  = 0x000004000, 
        SHGSI_LINKOVERLAY   = 0x000008000, 
        SHGSI_SELECTED      = 0x000010000, 
        SHGSI_LARGEICON     = 0x000000000, 
        SHGSI_SMALLICON     = 0x000000001, 
        SHGSI_SHELLICONSIZE = 0x000000004 
    }
}