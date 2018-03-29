using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

// ReSharper disable BuiltInTypeReferenceStyle
namespace ToastifyAPI.Native
{
    public static class Shell32
    {
        [DllImport("shell32.dll", SetLastError = false)]
        internal static extern Int32 SHGetStockIconInfo(ShStockIconId siid, ShGSI uFlags, ref ShStockIconInfo psii);
    }
}