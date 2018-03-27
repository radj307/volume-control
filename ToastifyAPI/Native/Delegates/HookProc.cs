using System;

namespace ToastifyAPI.Native.Delegates
{
    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);
}