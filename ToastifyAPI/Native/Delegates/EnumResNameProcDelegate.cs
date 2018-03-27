using System;

namespace ToastifyAPI.Native.Delegates
{
    public delegate bool EnumResNameProcDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);
}