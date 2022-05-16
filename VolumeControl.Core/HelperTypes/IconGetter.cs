using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using VolumeControl.Log;
using System.Windows.Interop;

namespace VolumeControl.Core.HelperTypes
{
    public static class IconGetter
    {
        public static ImageSource? GetIcon(string path, bool smallIcon, bool isDirectory)
        {
            // SHGFI_USEFILEATTRIBUTES takes the file name and attributes into account if it doesn't exist
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            if (smallIcon)
                flags |= SHGFI_SMALLICON;

            uint attributes = FILE_ATTRIBUTE_NORMAL;
            if (isDirectory)
                attributes |= FILE_ATTRIBUTE_DIRECTORY;

            if (0 != SHGetFileInfo(path,
                        attributes,
                        out SHFILEINFO shfi,
                        (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                        flags))
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                            shfi.hIcon,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
            }
            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        [DllImport("shell32")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        private const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        private const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        private const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        private const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        private const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        private const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        private const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        private const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        private const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        private const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
        private const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;

        private const uint SHGFI_ICON = 0x000000100;     // get icon
        private const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name
        private const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        private const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes
        private const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location
        private const uint SHGFI_EXETYPE = 0x000002000;     // return exe type
        private const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index
        private const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon
        private const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state
        private const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes
        private const uint SHGFI_LARGEICON = 0x000000000;     // get large icon
        private const uint SHGFI_SMALLICON = 0x000000001;     // get small icon
        private const uint SHGFI_OPENICON = 0x000000002;     // get open icon
        private const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon
        private const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute

        /// <summary>
        /// Gets the large and small variants of the specified icon from the specified DLL.
        /// </summary>
        /// <param name="path">The location of the target icon DLL.</param>
        /// <param name="number">The icon index number to retrieve.</param>
        /// <returns>A pair of icons where Item1 is the small icon, Item2 is the large icon; or null if the icons couldn't be loaded.</returns>
        public static (ImageSource?, ImageSource?)? GetIcons(string path, int number)
        {
            path = Environment.ExpandEnvironmentVariables(path.Trim('@'));
            if (!File.Exists(path))
                return null;
            try
            {
                if (number == -1)
                {
                    var fAttr = File.GetAttributes(path);
                    bool isDir = fAttr.HasFlag(FileAttributes.Directory);

                    return (GetIcon(path, true, isDir), GetIcon(path, false, isDir));
                }
                else if (path.Contains(".dll", StringComparison.OrdinalIgnoreCase) && ExtractIconEx(path, number, out IntPtr large, out IntPtr small, 1) != -1)
                {
                    return (Imaging.CreateBitmapSourceFromHIcon(small, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()), Imaging.CreateBitmapSourceFromHIcon(large, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                }
            }
            catch (Exception ex)
            {
                FLog.Log.WarningException(ex);
            }
            return null;
        }
        /// <inheritdoc cref="GetIcons(string, int)"/>
        /// <param name="iconPath">Specifies the location and index number of the target icon. See the output of <see cref="AudioDevice.IconPath"/> or <see cref="AudioSession.IconPath"/></param>
        public static (ImageSource?, ImageSource?)? GetIcons(string iconPath)
        {
            if (ParseIconPath(iconPath) is (string, int) result)
                return GetIcons(result.Item1, result.Item2);
            return null;
        }
        /// <summary>
        /// Parses the given icon path from <see cref="AudioDevice.IconPath"/> or <see cref="AudioSession.IconPath"/> into a path and index number.
        /// </summary>
        /// <param name="iconPath">Specifies the location and index number of the target icon. See the output of <see cref="AudioDevice.IconPath"/> or <see cref="AudioSession.IconPath"/></param>
        /// <returns>A pair where the Item1 is the filepath and Item2 is the index number, or null if <paramref name="iconPath"/> was invalid.</returns>
        public static (string, int)? ParseIconPath(string iconPath)
        {
            iconPath = iconPath.Trim('"', ' ', '\r', '\n', '\t');
            int pos = iconPath.LastIndexOf(',');
            if (pos != -1 && int.TryParse(iconPath[(pos + 1)..], out int iconNumber))
            {
                return (iconPath[..pos], iconNumber);
            }
            else return (iconPath, -1);
        }

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
    }
    public static class GetIconsExtensions
    {
        /// <inheritdoc cref="IconGetter.GetIcons(string)"/>
        public static (ImageSource?, ImageSource?)? GetIcons(this AudioDevice device) => device.IconPath.Length > 0 ? IconGetter.GetIcons(device.IconPath) : null;
        /// <inheritdoc cref="IconGetter.GetIcons(string)"/>
        public static (ImageSource?, ImageSource?)? GetIcons(this AudioSession session) => session.IconPath.Length > 0 ? IconGetter.GetIcons(session.IconPath) : null;
    }
}
