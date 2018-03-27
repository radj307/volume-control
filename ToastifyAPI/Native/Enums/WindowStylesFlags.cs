using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    /// <summary>
    /// Window styles (<see cref="GWL"/>).
    /// </summary>
    [Flags]
    public enum WindowStylesFlags : long
    {
        WS_BORDER           = 0x00800000L,
        WS_CAPTION          = 0x00C00000L,
        WS_CHILD            = 0x40000000L,
        WS_CHILDWINDOW      = 0x40000000L,
        WS_CLIPCHILDREN     = 0x02000000L,
        WS_CLIPSIBLINGS     = 0x04000000L,
        WS_DISABLED         = 0x08000000L,
        WS_DLGFRAME         = 0x00400000L,
        WS_GROUP            = 0x00020000L,
        WS_HSCROLL          = 0x00100000L,
        WS_ICONIC           = 0x20000000L,
        WS_MAXIMIZE         = 0x01000000L,
        WS_MAXIMIZEBOX      = 0x00010000L,
        WS_MINIMIZE         = 0x20000000L,
        WS_MINIMIZEBOX      = 0x00020000L,
        WS_OVERLAPPED       = 0x00000000L,
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUP            = 0x80000000L,
        WS_POPUPWINDOW      = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_SIZEBOX          = 0x00040000L,
        WS_SYSMENU          = 0x00080000L,
        WS_TABSTOP          = 0x00010000L,
        WS_THICKFRAME       = 0x00040000L,
        WS_TILED            = 0x00000000L,
        WS_TILEDWINDOW      = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_VISIBLE          = 0x10000000L,
        WS_VSCROLL          = 0x00200000L
    }
}