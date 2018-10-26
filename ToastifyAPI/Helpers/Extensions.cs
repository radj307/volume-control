using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using WpfPixelFormat = System.Windows.Media.PixelFormat;
using GdiPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ToastifyAPI.Helpers
{
    public static class Extensions
    {
        #region Static Members

        public static Keys ConvertToWindowsFormsKeys(this Key key)
        {
            string sKey = key.ToString();
            Keys keys = Keys.None;
            if (Enum.GetNames(typeof(Keys)).Contains(sKey, StringComparer.InvariantCultureIgnoreCase))
                Enum.TryParse(sKey, true, out keys);

            return keys;
        }

        public static uint GetScanCode(this Key key)
        {
            return User32.MapVirtualKey(key.GetVirtualKey(), MapVirtualKeyType.MAPVK_VK_TO_VSC);
        }

        public static WpfPixelFormat? ConvertToWpfPixelFormat(this GdiPixelFormat wPixelFormat)
        {
            switch (wPixelFormat)
            {
                case GdiPixelFormat.Format1bppIndexed:
                    return PixelFormats.Indexed1;

                case GdiPixelFormat.Format4bppIndexed:
                    return PixelFormats.Indexed4;

                case GdiPixelFormat.Format8bppIndexed:
                    return PixelFormats.Indexed8;

                case GdiPixelFormat.Format16bppGrayScale:
                    return PixelFormats.Gray16;

                case GdiPixelFormat.Format16bppRgb555:
                    return PixelFormats.Bgr555;

                case GdiPixelFormat.Format16bppRgb565:
                    return PixelFormats.Bgr565;

                case GdiPixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;

                case GdiPixelFormat.Canonical:
                case GdiPixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;

                case GdiPixelFormat.Format32bppArgb:
                    return PixelFormats.Bgra32;

                case GdiPixelFormat.Format32bppPArgb:
                    return PixelFormats.Pbgra32;

                case GdiPixelFormat.Format48bppRgb:
                    return PixelFormats.Rgb48; // Bgr48?

                case GdiPixelFormat.Format64bppArgb:
                    return PixelFormats.Rgba64; // Bgra64?

                case GdiPixelFormat.Format64bppPArgb:
                    return PixelFormats.Prgba64; // Pbgra64?

                case GdiPixelFormat.Alpha:
                case GdiPixelFormat.DontCare:
                case GdiPixelFormat.Indexed:
                case GdiPixelFormat.Gdi:
                case GdiPixelFormat.PAlpha:
                case GdiPixelFormat.Extended:
                case GdiPixelFormat.Format16bppArgb1555:
                case GdiPixelFormat.Max:
                    return null;

                default:
                    throw new ArgumentOutOfRangeException(nameof(wPixelFormat), wPixelFormat, null);
            }
        }

        public static MenuItem FindMenuItem(this Menu menu, Shortcut shortcut)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            return menu.FindMenuItem(Menu.FindShortcut, new IntPtr((int)shortcut));
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