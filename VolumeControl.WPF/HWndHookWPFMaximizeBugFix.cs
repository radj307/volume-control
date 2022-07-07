using System;
using System.Runtime.InteropServices;
using System.Windows;
using VolumeControl.WPF.PInvoke;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Adds a message hook handler that fixes a bug with WPF windows that have their <see cref="Window.WindowStyle"/> property set to <see cref="WindowStyle.None"/>.
    /// </summary>
    public static class HWndHookWPFMaximizeBugFix
    {
        private const int WM_GETMINMAXINFO = 0x0024;

        /// <inheritdoc cref="HWndHookWPFMaximizeBugFix"/>
        /// <remarks>This is an extension method provided by <see cref="HWndHookWPFMaximizeBugFix"/>.</remarks>
        /// <param name="hook">The hook object.</param>
        public static void AddMaximizeBugFixHandler(this HWndHook hook) => hook.AddHook(HandleMaximizeBug);

        private static IntPtr HandleMaximizeBug(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case WM_GETMINMAXINFO:
                WmGetMinMaxInfo(hWnd, lParam);
                handled = true;
                break;
            }
            return IntPtr.Zero;
        }

        private static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {

            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != System.IntPtr.Zero)
            {

                var monitorInfo = new MONITORINFO();
                _ = GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
    }
}