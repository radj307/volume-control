using System;
using System.Windows;
using System.Windows.Interop;

namespace Toastify.Helpers
{
    internal static class WindowExtensions
    {
        public static IntPtr GetHandle(this Window window)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(window);
            return wndHelper.Handle;
        }
    }
}