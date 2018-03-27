using System;

namespace ToastifyAPI.Native.Delegates
{
    public delegate void SendMessageDelegate(IntPtr hWnd, uint uMsg, UIntPtr dwData, IntPtr lResult);
}