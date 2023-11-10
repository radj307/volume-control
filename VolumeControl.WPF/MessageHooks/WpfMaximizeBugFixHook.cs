using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using VolumeControl.WPF.PInvoke;

namespace VolumeControl.WPF.MessageHooks
{
    /// <summary>
    /// Provides a window message hook that fixes a bug when maximizing WPF windows that use <see cref="WindowStyle.None"/>.
    /// </summary>
    public static class WpfMaximizeBugFixHook
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="HwndSourceHook"/> delegate that handles maximize messages.
        /// </summary>
        public static HwndSourceHook Hook { get; } = WndProcHook;
        #endregion Properties

        #region Methods

        #region (Private) WndProcHook
        private static IntPtr WndProcHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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
        #endregion (Private) WndProcHook

        #endregion Methods

        #region P/Invoke
        private const int WM_GETMINMAXINFO = 0x0024;

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
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
        #endregion P/Invoke
    }
}