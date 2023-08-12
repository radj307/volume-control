using CoreAudio;
using System.Text.RegularExpressions;

namespace Audio.Helpers
{
    /// <summary>
    /// Extension methods for the <see cref="MMDevice"/> class.
    /// </summary>
    public static class MMDeviceExtensions
    {
        /// <summary>
        /// Gets the name of this <see cref="MMDevice"/> instance as shown in Windows.
        /// </summary>
        /// <param name="mmDevice">The <see cref="MMDevice"/> instance to retrieve the name of.</param>
        /// <returns>The actual name of <paramref name="mmDevice"/> as a <see cref="string"/>.</returns>
        public static string GetDeviceName(this MMDevice mmDevice)
            => Regex.Replace(mmDevice.DeviceFriendlyName, $"\\(\\s*?{mmDevice.DeviceInterfaceFriendlyName}\\s*?\\)", "", RegexOptions.Compiled).Trim();
    }
}