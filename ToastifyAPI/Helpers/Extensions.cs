using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Helpers
{
    public static class Extensions
    {
        #region Static Members

        public static Keys ConvertToWindowsFormsKeys(this Key key)
        {
            if (Enum.GetNames(typeof(Keys)).Contains(key.ToString(), StringComparer.InvariantCultureIgnoreCase))
            {
                if (Enum.TryParse(key.ToString(), out Keys keys))
                    return keys;
            }

            return Keys.None;
        }

        public static uint GetScanCode(this Key key)
        {
            return User32.MapVirtualKey(key.GetVirtualKey(), MapVirtualKeyType.MAPVK_VK_TO_VSC);
        }

        public static object GetDefault(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        #endregion

        #region GetLParam

        public static uint GetLParam(this Key key)
        {
            return key.GetLParam(1, 0, 0, 0, 0);
        }

        public static uint GetLParam(this Key key, short repeatCount)
        {
            return key.GetLParam(repeatCount, 0, 0, 0, 0);
        }

        public static uint GetLParam(this Key key, short repeatCount, byte extended)
        {
            return key.GetLParam(repeatCount, extended, 0, 0, 0);
        }

        public static uint GetLParam(this Key key, short repeatCount, byte extended, byte contextCode)
        {
            return key.GetLParam(repeatCount, extended, contextCode, 0, 0);
        }

        public static uint GetLParam(this Key key, short repeatCount, byte extended, byte contextCode, byte previousState)
        {
            return key.GetLParam(repeatCount, extended, contextCode, previousState, 0);
        }

        public static uint GetLParam(this Key key, short repeatCount, byte extended, byte contextCode, byte previousState, byte transitionState)
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

        #endregion GetLParam

        #region GetVirtualKey

        public static uint GetVirtualKey(this Key key)
        {
            return key.GetVirtualKey(false);
        }

        public static uint GetVirtualKey(this Key key, bool preferLeftOrRight)
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
                        // Ignore!
                        break;
                }
            }

            return (uint)KeyInterop.VirtualKeyFromKey(key);
        }

        #endregion GetVirtualKey
    }
}