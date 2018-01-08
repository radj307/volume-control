using System;
using System.Windows;
using System.Windows.Input;
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

        public static uint GetLParam(this Key key, short repeatCount = 1, byte extended = 0, byte contextCode = 0, byte previousState = 0, byte transitionState = 0)
        {
            uint lParam = (uint)repeatCount;
            uint scanCode = key.GetScanCode();
            lParam += scanCode * 0x00010000;
            lParam += (uint)(extended * 0x01000000);
            lParam += (uint)(contextCode * 2 * 0x10000000);
            lParam += (uint)(previousState * 4 * 0x10000000);
            lParam += (uint)(transitionState * 8 * 0x10000000);
            return lParam;
        }

        public static uint GetVirtualKey(this Key key, bool preferLeftOrRight = false)
        {
            if (!preferLeftOrRight)
            {
                switch (key)
                {
                    case Key.LeftShift:
                    case Key.RightShift:
                        return 0x10;

                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        return 0x11;

                    case Key.LeftAlt:
                    case Key.RightAlt:
                        return 0x12;

                    default:
                        break;
                }
            }
            return (uint)KeyInterop.VirtualKeyFromKey(key);
        }

        public static uint GetScanCode(this Key key)
        {
            return Win32API.MapVirtualKey(key.GetVirtualKey(), Win32API.MapVirtualKeyType.MAPVK_VK_TO_VSC);
        }
    }
}