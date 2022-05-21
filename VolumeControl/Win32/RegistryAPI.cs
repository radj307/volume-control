using Microsoft.Win32;

namespace VolumeControl.Win32
{
    public class RegistryAPI
    {
        public struct Scope
        {
            public static readonly RegistryKey HKEY_CURRENT_USER = Registry.CurrentUser;
        }

        /// <summary>
        /// Gets a <see cref="RegistryKey"/> object from the Windows Registry.
        /// </summary>
        /// <param name="scope">The parent registry key where the target is located.</param>
        /// <param name="path">The path from the scope key to the target.</param>
        /// <param name="writable">When true, the returned key will be writable.</param>
        /// <returns>Returns the target <see cref="RegistryKey"/> if successful, or null if nothing was found.</returns>
        public static RegistryKey? GetKey(RegistryKey scope, string path, bool writable = true)
        {
            return scope.OpenSubKey(path, writable);
        }

        /// <summary>
        /// Deletes the specified <see cref="RegistryKey"/> from the Windows Registry.
        /// </summary>
        /// <param name="scope">The parent registry key where the target is located.</param>
        /// <param name="path">The path from the scope key to the target.</param>
        public static void DeleteKey(RegistryKey scope, string path)
        {
            scope.DeleteSubKey(path);
        }

        /// <summary>
        /// Gets a value from the specified key in the Windows Registry.
        /// </summary>
        /// <param name="scope">The parent registry key where the target is located.</param>
        /// <param name="path">The path from the scope key to the target.</param>
        /// <param name="valueName">The name of the target value.</param>
        /// <returns>A nullable object containing the value, or null if it wasn't found.</returns>
        public static object? GetValue(RegistryKey scope, string path, string valueName)
        {
            return GetKey(scope, path, false)?.GetValue(valueName);
        }

        /// <summary>
        /// Sets a value in the specified key in the Windows Registry.
        /// </summary>
        /// <param name="scope">The parent registry key where the target is located.</param>
        /// <param name="path">The path from the scope key to the target.</param>
        /// <param name="valueName">The name of the target value.</param>
        /// <param name="value">The value to write to the key value.</param>
        public static void SetValue(RegistryKey scope, string path, string valueName, object value)
        {
            GetKey(scope, path, true)?.SetValue(valueName, value);
        }

        /// <summary>
        /// Deletes a value in the specified key in the Windows Registry.
        /// </summary>
        /// <param name="scope">The parent registry key where the target is located.</param>
        /// <param name="path">The path from the scope key to the target.</param>
        /// <param name="valueName">The name of the target value.</param>
        public static void DeleteValue(RegistryKey scope, string path, string valueName)
        {
            GetKey(scope, path, true)?.DeleteValue(valueName);
        }
    }
}
