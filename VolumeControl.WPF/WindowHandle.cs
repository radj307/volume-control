using System;
using System.Windows;
using System.Windows.Interop;

namespace VolumeControl.WPF
{
    public class WindowHandleGetter
    {
        public IntPtr GetWindowHandle()
        {
            return new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle();
        }
        public HwndSource GetHwndSource(IntPtr hWnd)
        {
            return HwndSource.FromHwnd(hWnd);
        }
    }
}
