using System;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.RawInputAPI.Enums;
using ToastifyAPI.Native.RawInputAPI.Structs;

namespace ToastifyAPI.Native.RawInputAPI
{
    public static class RawInput
    {
        #region Static Members

        /// <summary>
        ///     Registers the mouse as raw input device.
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <exception cref="ArgumentException"><see cref="hWnd" /> is <see cref="IntPtr.Zero" />.</exception>
        /// <returns><code>true</code> if the function succeeds; <code>false</code> otherwise.</returns>
        public static bool RegisterRawMouseInput(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentException("Invalid window handle: IntPtr.Zero!", nameof(hWnd));

            RawInputDevice[] devices =
            {
                new RawInputDevice
                {
                    WindowHandle = hWnd,
                    UsagePage = HIDUsagePage.Generic,
                    Usage = HIDUsage.Mouse,
                    Flags = RawInputDeviceFlags.InputSink
                }
            };

            return User32.RegisterRawInputDevices(devices, 1, Marshal.SizeOf(typeof(RawInputDevice)));
        }

        #endregion
    }
}