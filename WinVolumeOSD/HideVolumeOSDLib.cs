using System.Runtime.InteropServices;
using WinHook;
using WinHook.Enum;

namespace WinVolumeOSD
{
    public class HideVolumeOSDLib
    {
        public HideVolumeOSDLib()
        {
            Hidden = false;
        }

        IntPtr hWndInject = IntPtr.Zero;
        public bool Hidden { get; private set; }

        public void Init()
        {
            hWndInject = FindOSDWindow(true);

            int count = 1;

            while (hWndInject == IntPtr.Zero && count < 9)
            {
                Win32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_UP, 0, 0, IntPtr.Zero);
                Win32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_DOWN, 0, 0, IntPtr.Zero);

                hWndInject = FindOSDWindow(true);

                // Quadratic backoff if the window is not found
                Thread.Sleep(1000 * (count ^ 2));
                count++;
            }

            // final try

            hWndInject = FindOSDWindow(false);

            if (hWndInject == IntPtr.Zero)
                return;
        }

        private static IntPtr FindOSDWindow(bool bSilent)
        {
            IntPtr hwndRet = IntPtr.Zero;
            IntPtr hwndHost = IntPtr.Zero;

            int pairCount = 0;

            // search for all windows with class 'NativeHWNDHost'

            while ((hwndHost = Win32.FindWindowEx(IntPtr.Zero, hwndHost, "NativeHWNDHost", "")) != IntPtr.Zero)
            {
                // if this window has a child with class 'DirectUIHWND' it might be the volume OSD

                if (Win32.FindWindowEx(hwndHost, IntPtr.Zero, "DirectUIHWND", "") != IntPtr.Zero)
                {
                    // if this is the only pair we are sure

                    if (pairCount == 0)
                        hwndRet = hwndHost;

                    pairCount++;

                    // if there are more pairs the criteria has failed...

                    if (pairCount > 1)
                    {
                        MessageBox.Show("Severe error: Multiple pairs found!", "HideVolumeOSD");
                        return IntPtr.Zero;
                    }
                }
            }

            // if no window found yet, there is no OSD window at all

            if (hwndRet == IntPtr.Zero && !bSilent)
                MessageBox.Show("Severe error: OSD window not found!", "HideVolumeOSD");

            return hwndRet;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            ShowOSD();
        }

        public void HideOSD()
        {
            if (!Win32.IsWindow(hWndInject))
                Init();

            Win32.ShowWindow(hWndInject, 6); // SW_MINIMIZE
            Hidden = true;
        }

        public void ShowOSD()
        {
            if (!Win32.IsWindow(hWndInject))
                Init();

            Win32.ShowWindow(hWndInject, 9); // SW_RESTORE
            Hidden = false;

            // show window on the screen

            Win32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_UP, 0, 0, IntPtr.Zero);
            Win32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_DOWN, 0, 0, IntPtr.Zero);
        }
    }
}
