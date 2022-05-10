using System;
using System.Windows;
using System.Windows.Interop;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Helper object for C# Class Libraries being unable to get the current WPF window handle.
    /// </summary>
    /// <remarks>This can be used to receive window messages for the current window even from within non-WPF projects.</remarks>
    public static class WindowHandleGetter
    {
        /// <summary>
        /// Gets the primary WPF window's handle.
        /// </summary>
        /// <remarks>This uses <see cref="WindowInteropHelper.EnsureHandle"/> to ensure that a valid handle is always returned at any point during initialization.</remarks>
        /// <returns><see cref="IntPtr"/> set to the current WPF window's handle.</returns>
        public static IntPtr GetWindowHandle()
        {
            return new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle();
        }
        /// <summary>
        /// Gets the <see cref="HwndSource"/> associated with the given Window Handle <paramref name="hWnd"/>.
        /// </summary>
        /// <param name="hWnd">The <see cref="IntPtr"/> associated with a WPF window.</param>
        /// <returns><see cref="HwndSource"/> that can be used to receive WPF window messages.</returns>
        public static HwndSource GetHwndSource(IntPtr hWnd)
        {
            return HwndSource.FromHwnd(hWnd);
        }
    }
}
