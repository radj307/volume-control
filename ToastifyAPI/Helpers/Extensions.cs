using System.Windows.Input;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Helpers
{
    public static class Extensions
    {
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
            return User32.MapVirtualKey(key.GetVirtualKey(), MapVirtualKeyType.MAPVK_VK_TO_VSC);
        }
    }
}