﻿using System;
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
        /// Gets the primary WPF window's handle.<br/>This uses <see cref="WindowInteropHelper.EnsureHandle"/> to ensure that a valid handle is always returned at any point during initialization.
        /// </summary>
        /// <returns><see cref="IntPtr"/> set to the current WPF window's handle.</returns>
        public static IntPtr GetWindowHandle(Window wnd) => new WindowInteropHelper(wnd).EnsureHandle();
        /// <inheritdoc cref="GetWindowHandle(Window)"/>
        /// <remarks><b>Without parameters, this function uses the main window's handle using the <see cref="Application.MainWindow"/> property from <see cref="Application.Current"/>.</b></remarks>
        public static IntPtr GetWindowHandle() => GetWindowHandle(Application.Current.MainWindow);
        /// <summary>
        /// Gets the <see cref="HwndSource"/> associated with the given Window Handle <paramref name="hWnd"/>.
        /// </summary>
        /// <param name="hWnd">The <see cref="IntPtr"/> associated with a WPF window.</param>
        /// <returns><see cref="HwndSource"/> that can be used to receive WPF window messages.</returns>
        public static HwndSource GetHwndSource(IntPtr hWnd) => HwndSource.FromHwnd(hWnd);
    }
}
