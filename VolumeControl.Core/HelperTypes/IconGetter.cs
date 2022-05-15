using System.Drawing;
using System.Runtime.InteropServices;
using VolumeControl.Log;

namespace VolumeControl.Core.HelperTypes
{
    public static class IconGetter
    {
        /// <summary>
        /// Gets the large and small variants of the specified icon from the specified DLL.
        /// </summary>
        /// <param name="path">The location of the target icon DLL.</param>
        /// <param name="number">The icon index number to retrieve.</param>
        /// <returns>A pair of icons where Item1 is the small icon, Item2 is the large icon; or null if the icons couldn't be loaded.</returns>
        public static (Icon, Icon)? GetIcons(string path, int number)
        {
            if (ExtractIconEx(path, number, out IntPtr large, out IntPtr small, 1) == 0)
            {
                try
                {
                    return (Icon.FromHandle(small), Icon.FromHandle(large));
                }
                catch (Exception ex)
                {
                    FLog.Log.WarningException(ex);
                }
            }
            return null;
        }
        /// <inheritdoc cref="GetIcons(string, int)"/>
        /// <param name="iconPath">Specifies the location and index number of the target icon. See the output of <see cref="AudioDevice.IconPath"/> or <see cref="AudioSession.IconPath"/></param>
        public static (Icon, Icon)? GetIcons(string iconPath)
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
            int pos = iconPath.LastIndexOf(',');
            if (pos != -1 && int.TryParse(iconPath[(pos + 1)..], out int iconNumber))
            {
                return (iconPath[..pos], iconNumber);
            }
            return null;
        }

        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);
    }
    public static class GetIconsExtensions
    {
        /// <inheritdoc cref="IconGetter.GetIcons(string)"/>
        public static (Icon, Icon)? GetIcons(this AudioDevice device) => IconGetter.GetIcons(device.IconPath);
        /// <inheritdoc cref="IconGetter.GetIcons(string)"/>
        public static (Icon, Icon)? GetIcons(this AudioSession session) => IconGetter.GetIcons(session.IconPath);
    }
}
