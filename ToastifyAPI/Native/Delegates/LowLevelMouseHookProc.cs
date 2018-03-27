using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

namespace ToastifyAPI.Native.Delegates
{
    public delegate IntPtr LowLevelMouseHookProc(int code, WindowsMessagesFlags wParam, [In] LowLevelMouseHookStruct lParam);
}