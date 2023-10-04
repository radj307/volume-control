using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VolumeControl.WPF
{
    /// <summary>
    /// A <see langword="static"/> class that provides methods for extracting icons from files, directories, or DLLs, as well as directly loading them from icon or image files.
    /// </summary>
    public static class IconExtractor
    {
        #region ExtractFromHandle
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from the specified <paramref name="iconHandle"/>.
        /// </summary>
        /// <remarks>
        /// Internally, this just calls <see cref="Imaging.CreateBitmapSourceFromHIcon(IntPtr, Int32Rect, BitmapSizeOptions)"/>.
        /// </remarks>
        /// <param name="iconHandle">A pointer to the unmanaged icon source.</param>
        /// <param name="sourceRect">The size of the source image.</param>
        /// <param name="sizeOptions">The output size of the <see cref="ImageSource"/>.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified <paramref name="iconHandle"/> if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource ExtractFromHandle(IntPtr iconHandle, Int32Rect sourceRect, BitmapSizeOptions sizeOptions)
        {
            return Imaging.CreateBitmapSourceFromHIcon(iconHandle, sourceRect, sizeOptions);
        }
        /// <inheritdoc cref="ExtractFromHandle(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static ImageSource ExtractFromHandle(IntPtr iconHandle, Int32Rect sourceRect)
            => ExtractFromHandle(iconHandle, sourceRect, BitmapSizeOptions.FromEmptyOptions());
        /// <inheritdoc cref="ExtractFromHandle(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static ImageSource ExtractFromHandle(IntPtr iconHandle)
            => ExtractFromHandle(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> from the specified <paramref name="iconHandle"/>.
        /// </summary>
        /// <param name="iconHandle">A pointer to the unmanaged icon source.</param>
        /// <param name="sourceRect">The size of the source image.</param>
        /// <param name="sizeOptions">The output size of the <see cref="ImageSource"/>.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified <paramref name="iconHandle"/> if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromHandle(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static bool TryExtractFromHandle(IntPtr iconHandle, Int32Rect sourceRect, BitmapSizeOptions sizeOptions, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromHandle(iconHandle, sourceRect, sizeOptions);
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryExtractFromHandle(IntPtr, Int32Rect, BitmapSizeOptions, out ImageSource)"/>
        public static bool TryExtractFromHandle(IntPtr iconHandle, Int32Rect sourceRect, out ImageSource imageSource)
            => TryExtractFromHandle(iconHandle, sourceRect, BitmapSizeOptions.FromEmptyOptions(), out imageSource);
        /// <inheritdoc cref="TryExtractFromHandle(IntPtr, Int32Rect, BitmapSizeOptions, out ImageSource)"/>
        public static bool TryExtractFromHandle(IntPtr iconHandle, out ImageSource imageSource)
            => TryExtractFromHandle(iconHandle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions(), out imageSource);
        #endregion ExtractFromHandle

        #region ExtractFromIcoFile
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from an icon file (.ico).
        /// </summary>
        /// <remarks>
        /// To get an ImageSource from image filetypes other than ICO, use the <see cref="ExtractFromImageFile(Uri)"/> function instead.
        /// </remarks>
        /// <param name="icoPath">The path to the ICO file.</param>
        /// <param name="index">The index of the icon to retrieve from the ICO file. Not all ICO files contain multiple icons</param>
        /// <returns>An <see cref="ImageSource"/> for the specified icon if successful; otherwise <see langword="null"/>.</returns>
        /// <exception cref="InvalidOperationException">The specified <paramref name="index"/> is not valid for the specified file.</exception>
        public static ImageSource? ExtractFromIcoFile(string icoPath, int index = 0)
        {
            using var stream = new FileStream(icoPath, FileMode.Open, FileAccess.Read);

            var decoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            if (index < decoder.Frames.Count)
            {
                return decoder.Frames[index];
            }
            else throw new InvalidOperationException($"The specified icon index {index} doesn't exist in file '{icoPath}'");
        }
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> from an icon file (.ico).
        /// </summary>
        /// <param name="icoPath">The path to the ICO file.</param>
        /// <param name="index">The index of the icon to retrieve from the ICO file. Not all ICO files contain multiple icons</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified icon if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromIcoFile(string, int)"/>
        public static bool TryExtractFromIcoFile(string icoPath, int index, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromIcoFile(icoPath, index)!;
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryExtractFromIcoFile(string, int, out ImageSource)"/>
        public static bool TryExtractFromIcoFile(string icoPath, out ImageSource imageSource)
            => TryExtractFromIcoFile(icoPath, 0, out imageSource);
        #endregion ExtractFromIcoFile

        #region ExtractFromImageFile
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from the specified image file (.png, .jpg, etc.).
        /// </summary>
        /// <remarks>
        /// This function is not capable of loading icon files (.ico) that contain multiple icons;
        /// use the <see cref="ExtractFromIcoFile(string, int)"/> function instead.
        /// </remarks>
        /// <param name="imageUri">The <see cref="Uri"/> of an image file.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified image if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource? ExtractFromImageFile(Uri imageUri)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = imageUri;
            bmp.EndInit();
            return bmp;
        }
        /// <inheritdoc cref="ExtractFromImageFile(Uri)"/>
        /// <param name="imagePath">The imageUri of an image file.</param>
        public static ImageSource? ExtractFromImageFile(string imagePath)
            => ExtractFromImageFile(new Uri(imagePath));
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> from the specified image file (.png, .jpg, etc.).
        /// </summary>
        /// <param name="imageUri">The <see cref="Uri"/> of an image file.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified image if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromImageFile(string)"/>
        public static bool TryExtractFromImageFile(Uri imageUri, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromImageFile(imageUri)!;
                return imageSource != null;
            }
            catch (Exception)
            {
                imageSource = null!;
                return false; //< return null in release configurations
            }
        }
        /// <inheritdoc cref="TryExtractFromImageFile(Uri, out ImageSource)"/>
        public static bool TryExtractFromImageFile(string imagePath, out ImageSource imageSource)
            => TryExtractFromImageFile(new Uri(imagePath), out imageSource);
        #endregion ExtractFromImageFile

        #region ExtractFromFile
        /// <summary>
        /// Gets an <see cref="ImageSource"/> for the icon of a regular file, including executable files (.exe).
        /// </summary>
        /// <param name="filePath">The location of a file.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified file's icon if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource? ExtractFromFile(string filePath)
            => ExtractWithSHGetFileInfo(
                path: filePath,
                flags: SHGFI_ICON | SHGFI_SYSICONINDEX | SHGFI_SHELLICONSIZE | SHGFI_USEFILEATTRIBUTES,
                attributes: FILE_ATTRIBUTE_NORMAL);
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> for the icon of a regular file, including executable files (.exe).
        /// </summary>
        /// <param name="filePath">The location of a file.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified file icon if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromFile(string)"/>
        public static bool TryExtractFromFile(string filePath, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromFile(filePath)!;
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        #endregion ExtractFromFile

        #region ExtractFromDirectory
        /// <summary>
        /// Gets an <see cref="ImageSource"/> for the specified directory.
        /// </summary>
        /// <param name="directoryPath">The location of a directory.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified directory's icon if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource? ExtractFromDirectory(string directoryPath)
            => ExtractWithSHGetFileInfo(
                path: directoryPath,
                flags: SHGFI_ICON | SHGFI_SYSICONINDEX | SHGFI_SHELLICONSIZE | SHGFI_USEFILEATTRIBUTES,
                attributes: FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIRECTORY);
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> for the specified directory.
        /// </summary>
        /// <param name="directoryPath">The location of a directory.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified directory icon if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromDirectory(string)"/>
        public static bool TryExtractFromDirectory(string directoryPath, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromDirectory(directoryPath)!;
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        #endregion ExtractFromDirectory

        #region ExtractFromDll
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from an icon in the specified dynamic-link library file (.dll).
        /// </summary>
        /// <param name="dllPath">The location of a DLL.</param>
        /// <param name="index">The index of the icon to retrieve.</param>
        /// <param name="getSmallIcon">When <see langword="true"/>, retrieves the smaller size of icon; otherwise retrieves the larger size of icon.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified icon if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource? ExtractFromDll(string dllPath, int index, bool getSmallIcon)
            => ExtractWithExtractIconEx(Environment.ExpandEnvironmentVariables(dllPath.Trim('@', '"', ' ', '\r', '\n', '\t')), index, getSmallIcon);
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> from an icon in the specified dynamic-link library file (.dll).
        /// </summary>
        /// <param name="dllPath">The location of a DLL.</param>
        /// <param name="index">The index of the icon to retrieve.</param>
        /// <param name="getSmallIcon">When <see langword="true"/>, retrieves the smaller size of icon; otherwise retrieves the larger size of icon.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified icon if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromDll(string, int, bool)"/>
        public static bool TryExtractFromDll(string dllPath, int index, bool getSmallIcon, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromDll(dllPath, index, getSmallIcon)!;
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        #endregion ExtractFromDll

        #region ExtractFromPath
        /// <summary>
        /// Gets an <see cref="ImageSource"/> from the specified <paramref name="iconPath"/>.
        /// </summary>
        /// <param name="iconPath">A filesystem path to a file or directory, or the path and index of an icon in a DLL.</param>
        /// <param name="getSmallIcon">When <see langword="true"/>, retrieves the smaller size of icon; otherwise retrieves the larger size of icon.</param>
        /// <returns>An <see cref="ImageSource"/> for the specified icon if successful; otherwise <see langword="null"/>.</returns>
        public static ImageSource? ExtractFromPath(string iconPath, bool getSmallIcon = true)
        {
            if (string.IsNullOrEmpty(iconPath)) return null;

            ParseIconPath(iconPath, out string resolvedIconPath, out int iconIndex);

            if (iconIndex == -1)
            { // path points to a file that isn't a DLL
                if (iconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                { // path points to an ICO file
                    return ExtractFromIcoFile(iconPath);
                }
                else return ExtractFromFile(resolvedIconPath);
            }
            if (resolvedIconPath.Contains(".dll", StringComparison.OrdinalIgnoreCase))
            { // path points to a DLL
                return ExtractFromDll(resolvedIconPath, iconIndex, getSmallIcon);
            }
            if (File.GetAttributes(iconPath).HasFlag(FileAttributes.Directory)) //< this is faster than Directory.Exists()
            { // path points to a directory
                return ExtractFromDirectory(iconPath);
            }

            return null;
            //throw new IconNotFoundException("Couldn't find any icons with the specified path!", iconPath);
        }
        /// <summary>
        /// Attempts to get an <see cref="ImageSource"/> from the specified <paramref name="iconPath"/>.
        /// </summary>
        /// <param name="iconPath">A filesystem path to a file or directory, or the path and index of an icon in a DLL.</param>
        /// <param name="getSmallIcon">When <see langword="true"/>, retrieves the smaller size of icon; otherwise retrieves the larger size of icon.</param>
        /// <param name="imageSource">An <see cref="ImageSource"/> for the specified icon if successful.</param>
        /// <returns><see langword="true"/> when no exceptions occurred and <paramref name="imageSource"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        /// <inheritdoc cref="ExtractFromPath(string, bool)"/>
        public static bool TryExtractFromPath(string iconPath, bool getSmallIcon, out ImageSource imageSource)
        {
            try
            {
                imageSource = ExtractFromPath(iconPath, getSmallIcon)!;
                return imageSource != null;
            }
            catch
            {
                imageSource = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryExtractFromPath(string, bool, out ImageSource)"/>
        public static bool TryExtractFromPath(string path, out ImageSource imageSource)
            => TryExtractFromPath(path, true, out imageSource);
        #endregion ExtractFromPath

        #region ParseIconPath
        /// <summary>
        /// Parses the specified <paramref name="iconPath"/> by separating the filepath and the icon index.
        /// </summary>
        /// <remarks>
        /// This is used by <see cref="ExtractFromPath(string, bool)"/> to parse icon paths that point to a specific icon within a DLL that may contain multiple icons.
        /// </remarks>
        /// <param name="iconPath">The path to an icon within a file, or the path to an icon file.</param>
        /// <param name="filePath">The path to the file that contains the target icon.</param>
        /// <param name="iconIndex">The index of the target icon when the specified <paramref name="iconPath"/> contains one; otherwise -1.</param>
        public static void ParseIconPath(string iconPath, out string filePath, out int iconIndex)
        {
            // remove invalid preceding/trailing chars
            iconPath = iconPath.Trim('"', ' ');

            // find the position of the last comma (delimitor) in the path
            int separatorPos = iconPath.LastIndexOf(',');

            // check if the separator position is valid and try to parse the index
            if (separatorPos != -1 && int.TryParse(iconPath[(separatorPos + 1)..], out iconIndex))
            { // icon index is present and valid
                filePath = iconPath[..separatorPos];
            }
            else
            { // icon index is invalid/not present
                filePath = iconPath;
                iconIndex = -1;
            }
        }
        #endregion ParseIconPath

        #region Win32 Helper Methods
        #region ExtractWithSHGetFileInfo
        private static ImageSource? ExtractWithSHGetFileInfo(string path, uint flags, uint attributes, Int32Rect int32Rect, BitmapSizeOptions bitmapSizeOptions)
        {
            IntPtr iconHandle = IntPtr.Zero;
            try
            {
                if (SHGetFileInfo(path, attributes, out SHFILEINFO fileInfo, (uint)Marshal.SizeOf<SHFILEINFO>(), flags) != 0)
                {
                    iconHandle = fileInfo.hIcon; //< do this so that we can clean up in the finally block
                    return Imaging.CreateBitmapSourceFromHIcon(iconHandle, int32Rect, bitmapSizeOptions);
                }
                else return null;
            }
            finally
            {
                _ = DestroyIcon(iconHandle);
            }
        }
        private static ImageSource? ExtractWithSHGetFileInfo(string path, uint flags, uint attributes)
            => ExtractWithSHGetFileInfo(path, flags, attributes, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        #endregion ExtractWithSHGetFileInfo

        #region ExtractWithExtractIconEx
        private static (ImageSource? largeIcon, ImageSource? smallIcon) ExtractWithExtractIconEx(string path, int iconIndex, Int32Rect smallInt32Rect, BitmapSizeOptions smallBitmapSizeOptions, Int32Rect largeInt32Rect, BitmapSizeOptions largeBitmapSizeOptions)
        {
            IntPtr largeIcon = IntPtr.Zero;
            IntPtr smallIcon = IntPtr.Zero;
            try
            {
                if (ExtractIconEx(path, iconIndex, out largeIcon, out smallIcon, 1) != 0)
                {
                    return (largeIcon != IntPtr.Zero ? Imaging.CreateBitmapSourceFromHIcon(largeIcon, largeInt32Rect, largeBitmapSizeOptions) : null,
                            smallIcon != IntPtr.Zero ? Imaging.CreateBitmapSourceFromHIcon(smallIcon, smallInt32Rect, smallBitmapSizeOptions) : null);
                }
                else return (null, null);
            }
            finally
            {
                _ = DestroyIcon(largeIcon);
                _ = DestroyIcon(smallIcon);
            }
        }
        private static (ImageSource? largeIcon, ImageSource? smallIcon) ExtractWithExtractIconEx(string path, int iconIndex)
        {
            var int32Rect = Int32Rect.Empty;
            var bitmapSizeOptions = BitmapSizeOptions.FromEmptyOptions();
            return ExtractWithExtractIconEx(path, iconIndex, int32Rect, bitmapSizeOptions, int32Rect, bitmapSizeOptions);
        }
        private static ImageSource? ExtractWithExtractIconEx(string path, int iconIndex, bool getSmallIcon)
        {
            var (largeIcon, smallIcon) = ExtractWithExtractIconEx(path, iconIndex);
            return getSmallIcon
                ? smallIcon
                : largeIcon;
        }
        #endregion ExtractWithExtractIconEx
        #endregion Win32 Helper Methods

        #region Win32
        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }
        [DllImport("shell32", CharSet = CharSet.Unicode)]
        static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        #region Constants
        const uint SHGFI_ICON = 0x000000100;                // get icon
        //const uint SHGFI_DISPLAYNAME = 0x000000200;         // get display name
        //const uint SHGFI_TYPENAME = 0x000000400;            // get type name
        //const uint SHGFI_ATTRIBUTES = 0x000000800;          // get attributes
        //const uint SHGFI_ICONLOCATION = 0x000001000;        // get icon imageUri
        //const uint SHGFI_EXETYPE = 0x000002000;             // return exe type
        const uint SHGFI_SYSICONINDEX = 0x000004000;        // get system icon index
        //const uint SHGFI_LINKOVERLAY = 0x000008000;         // put a link overlay on icon
        //const uint SHGFI_SELECTED = 0x000010000;            // show icon in selected state
        //const uint SHGFI_ATTR_SPECIFIED = 0x000020000;      // get only specified attributes
        //const uint SHGFI_LARGEICON = 0x000000000;           // get largeIcon icon
        //const uint SHGFI_SMALLICON = 0x000000001;           // get getSmallIcon icon
        //const uint SHGFI_OPENICON = 0x000000002;            // get open icon
        const uint SHGFI_SHELLICONSIZE = 0x000000004;       // get shell size icon
        //const uint SHGFI_PIDL = 0x000000008;                // pszPath is a pidl
        const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;   // use passed dwFileAttribute

        //const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
        //const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
        //const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
        const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        //const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
        //const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
        const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        //const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
        //const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
        //const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        //const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
        //const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
        //const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
        //const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;
        //const uint FILE_ATTRIBUTE_VIRTUAL = 0x00010000;
        #endregion Constants

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
        [DllImport("user32.dll")]
        static extern int DestroyIcon(IntPtr hIcon);
        #endregion Win32
    }
}
