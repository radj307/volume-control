using System.Runtime.InteropServices;

namespace VolumeControl.Core
{
    public class User32
    {
        [DllImport("user32.dll")]
        public extern static bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_HIDE = 0;
        public const int SW_SHOWNOACTIVATE = 4;
    }
}
