using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

// ReSharper disable BuiltInTypeReferenceStyle
namespace ToastifyAPI
{
    public static partial class Win32API
    {
        public static ImageSource GetStockIconImage(ShStockIconId iconId, bool large)
        {
            return GetStockIconImage(iconId, large, false);
        }

        public static ImageSource GetStockIconImage(ShStockIconId iconId, bool large, bool getSystemIcon)
        {
            BitmapSource imageSource;

            if (Environment.OSVersion.Version.Major >= 6 && !getSystemIcon)
            {
                ShStockIconInfo sii = new ShStockIconInfo { cbSize = (UInt32)Marshal.SizeOf(typeof(ShStockIconInfo)) };

                int errCode = Shell32.SHGetStockIconInfo(iconId, ShGSI.SHGSI_ICON | (large ? ShGSI.SHGSI_LARGEICON : ShGSI.SHGSI_SMALLICON) | ShGSI.SHGSI_SHELLICONSIZE, ref sii);
                Marshal.ThrowExceptionForHR(errCode);

                imageSource = Imaging.CreateBitmapSourceFromHIcon(sii.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                User32.DestroyIcon(sii.hIcon);
            }
            else
            {
                Icon icon;
                switch (iconId)
                {
                    case ShStockIconId.SIID_APPLICATION:
                        icon = SystemIcons.Application;
                        break;

                    case ShStockIconId.SIID_HELP:
                        icon = SystemIcons.Question;
                        break;

                    case ShStockIconId.SIID_ERROR:
                        icon = SystemIcons.Error;
                        break;

                    case ShStockIconId.SIID_INFO:
                        icon = SystemIcons.Information;
                        break;

                    case ShStockIconId.SIID_WARNING:
                        icon = SystemIcons.Warning;
                        break;

                    case ShStockIconId.SIID_SHIELD:
                        icon = SystemIcons.Shield;
                        break;

                    default:
                        throw new ArgumentException($@"No system icon available for {iconId}", nameof(iconId));
                }

                imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            return imageSource;
        }
    }
}