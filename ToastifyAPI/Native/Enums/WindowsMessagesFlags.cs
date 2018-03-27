using System;

// ReSharper disable InconsistentNaming
namespace ToastifyAPI.Native.Enums
{
    [Flags]
    public enum WindowsMessagesFlags : uint
    {
        // Window Messages & Notifications
        WM_CLOSE           = 0x0010,
        WM_QUIT            = 0x0012,
        WM_ERASEBKGND      = 0x0014,
        WM_CHILDACTIVATE   = 0x0022,
        WM_GETICON         = 0x007F,

        // Keyboard Input Messages & Notifications
        WM_KEYDOWN         = 0x0100,
        WM_KEYUP           = 0x0101,
        WM_CHAR            = 0x0102,
        WM_SYSKEYDOWN      = 0x0104,
        WM_SYSKEYUP        = 0x0105,
        WM_SYSCHAR         = 0x0106,

        WM_APPCOMMAND      = 0x0319,

        // Menu Messages & Notifications
        WM_COMMAND         = 0x0111,
        WM_UNINITMENUPOPUP = 0x0125,

        // Keyboard Accelerator Messages & Notifications
        WM_INITMENU        = 0x0116,
        WM_INITMENUPOPUP   = 0x0117,
        WM_MENUSELECT      = 0x011F,

        // Mouse Input Notifications
        WM_LBUTTONDOWN     = 0x0201,
        WM_LBUTTONUP       = 0x0202,
        WM_LBUTTONDBLCLK   = 0x0203,
        WM_RBUTTONDOWN     = 0x0204,
        WM_RBUTTONUP       = 0x0205,
        WM_RBUTTONDBLCLK   = 0x0206,
        WM_MBUTTONDOWN     = 0x0207,
        WM_MBUTTONUP       = 0x0208,
        WM_MBUTTONDBLCLK   = 0x0209,
        WM_MOUSEWHEEL      = 0x020A,
        WM_XBUTTONDOWN     = 0x020B,
        WM_XBUTTONUP       = 0x020C,
        WM_XBUTTONDBLCLK   = 0x020D,
        WM_MOUSEHWHEEL     = 0x020E,
        WM_NCXBUTTONDOWN   = 0x00AB,
        WM_NCXBUTTONUP     = 0x00AC,
        WM_NCXBUTTONDBLCLK = 0x00AD,
    }
}