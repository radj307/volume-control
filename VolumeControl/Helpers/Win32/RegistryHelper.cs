using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;

namespace VolumeControl.Helpers.Win32
{
    internal static class RegistryHelper
    {
        #region Fields
        /// <summary>
        /// The HKCU subkey used for running apps on startup
        /// </summary>
        const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        /// <summary>
        /// The HKCU subkey used by the task manager to toggle whether startup apps are enabled or not.
        /// </summary>
        const string ToggleRunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        #endregion Fields

        #region (Enum) BaseKey
        /// <summary>
        /// Enumeration of the registry base keys.
        /// </summary>
        /// <remarks>
        /// Uses the same values as <see cref="RegistryHive"/>, so it is interoperable.
        /// </remarks>
        public enum BaseKey
        {
            HKEY_CLASSES_ROOT = int.MinValue,
            HKEY_CURRENT_USER = -2147483647,
            HKEY_LOCAL_MACHINE = -2147483646,
            HKEY_USERS = -2147483645,
            HKEY_PERFORMANCE_DATA = -2147483644,
            HKEY_CURRENT_CONFIG = -2147483643,
        }
        #endregion (Enum) BaseKey

        #region Methods

        #region GetValueKind
        /// <summary>1
        /// Gets the <see cref="RegistryValueKind"/> to use for the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to get the <see cref="RegistryValueKind"/> for.</param>
        /// <param name="allowUnknown">When <see langword="true"/>, returns <see cref="RegistryValueKind.Unknown"/> instead of throwing an exception. When attempting to set the registry key, it will probably still throw an exception.</param>
        /// <returns>The <see cref="RegistryValueKind"/> for <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="allowUnknown"/> was <see langword="false"/> and the specified <paramref name="value"/> is an unsupported array type.</exception>
        public static RegistryValueKind GetValueKind(object value, bool allowUnknown = false)
        {
            if (value is int)
                return RegistryValueKind.DWord;
            else if (value is long)
                return RegistryValueKind.QWord;
            else if (value is Array)
            {
                if (value is byte[])
                    return RegistryValueKind.Binary;
                else if (value is string[])
                    return RegistryValueKind.MultiString;
                else
                {
                    if (allowUnknown)
                        return RegistryValueKind.Unknown;
                    throw new ArgumentException($"The specified value type \"{value.GetType()}\" is not supported!", nameof(value));
                }
            }
            else return RegistryValueKind.String;
        }
        #endregion GetValueKind

        #region GetBaseKey
        /// <summary>
        /// Gets the <see cref="RegistryKey"/> associated with the specified <paramref name="baseKey"/>.
        /// </summary>
        /// <param name="baseKey">The <see cref="BaseKey"/> enum value of the registry base key to get.</param>
        /// <returns>The <see cref="RegistryKey"/> for the specified <paramref name="baseKey"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">The specified <paramref name="baseKey"/> value is not valid for the <see cref="BaseKey"/> enum.</exception>
        public static RegistryKey GetBaseKey(BaseKey baseKey) => baseKey switch
        {
            BaseKey.HKEY_CLASSES_ROOT => Registry.ClassesRoot,
            BaseKey.HKEY_CURRENT_USER => Registry.CurrentUser,
            BaseKey.HKEY_LOCAL_MACHINE => Registry.LocalMachine,
            BaseKey.HKEY_USERS => Registry.Users,
            BaseKey.HKEY_PERFORMANCE_DATA => Registry.PerformanceData,
            BaseKey.HKEY_CURRENT_CONFIG => Registry.CurrentConfig,
            _ => throw new InvalidEnumArgumentException(nameof(baseKey), (int)baseKey, typeof(BaseKey)),
        };
        #endregion GetBaseKey

        #region SetValue
        /// <summary>
        /// Sets the specified value in the registry.
        /// </summary>
        /// <param name="baseKey">The registry base key of the target key.</param>
        /// <param name="key">The path to the target key, relative to the specified <paramref name="baseKey"/>.</param>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="valueKind">The registry value type of the specified <paramref name="value"/>.</param>
        /// <returns><see langword="true"/> when successful; otherwise, <see langword="false"/>.</returns>
        public static bool SetValue(BaseKey baseKey, string key, string name, object value, RegistryValueKind valueKind)
        {
            using var regKey = GetBaseKey(baseKey).OpenSubKey(key, writable: true);
            if (regKey == null) return false;
            regKey.SetValue(name, value, valueKind);
            return true;
        }
        /// <inheritdoc cref="SetValue(BaseKey, string, string, object, RegistryValueKind)"/>
        public static bool SetValue(BaseKey baseKey, string key, string name, object value)
        {
            using var regKey = GetBaseKey(baseKey).OpenSubKey(key, writable: true);
            if (regKey == null) return false;
            regKey.SetValue(name, value, GetValueKind(value));
            return true;
        }
        #endregion SetValue

        #region DeleteValue
        /// <summary>
        /// Deletes the specified value from the registry.
        /// </summary>
        /// <param name="baseKey">The registry base key of the target key.</param>
        /// <param name="key">The path to the target key, relative to the specified <paramref name="baseKey"/>.</param>
        /// <param name="name">The name of the value to delete.</param>
        /// <returns><see langword="true"/> when the value does exist anymore; otherwise, <see langword="false"/>.</returns>
        public static bool DeleteValue(BaseKey baseKey, string key, string name)
        {
            using var regKey = GetBaseKey(baseKey).OpenSubKey(key, true);
            if (regKey == null) return false;
            regKey.DeleteValue(name, false);
            return true;
        }
        #endregion DeleteValue

        #region GetValue
        /// <summary>
        /// Gets the specified value from the registry.
        /// </summary>
        /// <param name="baseKey">The registry base key of the target key.</param>
        /// <param name="key">The path to the target key, relative to the specified <paramref name="baseKey"/>.</param>
        /// <param name="name">The name of the value to get.</param>
        /// <returns>The value of the target key when found; otherwise (or if the value was <see langword="null"/>), <see langword="null"/>.</returns>
        public static object? GetValue(BaseKey baseKey, string key, string name)
        {
            using var regKey = GetBaseKey(baseKey).OpenSubKey(key);
            return regKey?.GetValue(name, null);
        }
        /// <inheritdoc cref="GetValue(BaseKey, string, string)"/>
        /// <typeparam name="T">Type to cast the value to before returning it.</typeparam>
        public static T? GetValue<T>(BaseKey baseKey, string key, string name)
            => (T?)GetValue(baseKey, key, name);
        #endregion GetValue

        #region TryGetValue
        public static bool TryGetValue<T>(BaseKey baseKey, string key, string name, out T value)
        {
            try
            {
                value = (T)GetValue(baseKey, key, name)!;
                return value != null;
            }
            catch
            {
                value = default!;
                return false;
            }
        }
        #endregion TryGetValue

        #region (Private) IsToggleRunValueSetToEnabled
        private static bool IsToggleRunValueSetToEnabled(byte[]? value)
        {
            if (value == null) return false;
            if (value.Length != 24)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Expected length 24; actual length was {value.Length}");

            for (int i = 0; i < 24; ++i)
            {
                switch (i)
                {
                case 1:
                    if (value[i] != 2)
                        return false;
                    break;
                default:
                    if (value[i] != 0)
                        return false;
                    break;
                }
            }
            return true;
        }
        #endregion (Private) IsToggleRunValueSetToEnabled

        #region (Private) GetEnabledIsToggleRunValue
        private static byte[] GetEnabledIsToggleRunValue()
        {
            var b = new byte[24];
            b[1] = 2;
            return b;
        }
        #endregion (Private) GetEnabledIsToggleRunValue

        #region EnableRunAtStartup
        /// <summary>
        /// Enables run at startup for the specified application.
        /// </summary>
        /// <param name="valueName">The name of the target value in the run at startup subkey.</param>
        /// <param name="executablePath">The path to the executable to set.</param>
        /// <returns><see langword="true"/> when successful; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="FileNotFoundException">The specified <paramref name="executablePath"/> doesn't exist!</exception>
        public static bool EnableRunAtStartup(string valueName, string executablePath)
        {
            if (!File.Exists(executablePath))
                throw new FileNotFoundException(executablePath);
            // check for an existing value
            if (TryGetValue<string>(BaseKey.HKEY_CURRENT_USER, RunKeyPath, valueName, out var value)
                && value.Equals(executablePath, StringComparison.OrdinalIgnoreCase))
            { // this instance already has a run at startup key
              // check if it's disabled through the task manager
                if (!IsToggleRunValueSetToEnabled(GetValue<byte[]>(BaseKey.HKEY_CURRENT_USER, ToggleRunKeyPath, valueName)))
                { // run at startup is disabled through the task manager, enable it
                    var enableValue = new byte[24];
                    enableValue[1] = 2;
                    return SetValue(BaseKey.HKEY_CURRENT_USER, ToggleRunKeyPath, valueName, enableValue);
                }
                else return true; //< run at startup is already enabled for this instance
            }

            return SetValue(BaseKey.HKEY_CURRENT_USER, RunKeyPath, valueName, executablePath) //< set the executable path for run at startup
                && SetValue(BaseKey.HKEY_CURRENT_USER, ToggleRunKeyPath, valueName, GetEnabledIsToggleRunValue()); //< enable run at startup
        }
        #endregion EnableRunAtStartup

        #region DisableRunAtStartup
        /// <summary>
        /// Disables run at startup for the specified application.
        /// </summary>
        /// <param name="valueName">The name of the target value in the run at startup subkey.</param>
        /// <returns><see langword="true"/> when successful; otherwise, <see langword="false"/>.</returns>
        public static bool DisableRunAtStartup(string valueName)
        {
            if (DeleteValue(BaseKey.HKEY_CURRENT_USER, RunKeyPath, valueName))
            {
                // delete the entry for toggling run at startup too, but don't let it affect the return value
                DeleteValue(BaseKey.HKEY_CURRENT_USER, ToggleRunKeyPath, valueName);
                return true;
            }
            else return false;
        }
        #endregion DisableRunAtStartup

        #region IsRunAtStartupEnabled
        /// <summary>
        /// Checks if the specified application is set to run at startup.
        /// </summary>
        /// <remarks>
        /// Does not check if it is disabled through the task manager.
        /// </remarks>
        /// <param name="valueName">The name of the target value in the run at startup subkey.</param>
        /// <param name="executablePath">The path to the executable to check.</param>
        /// <returns><see langword="true"/> when enabled; <see langword="false"/> when disabled; <see langword="null"/> when enabled but the <paramref name="executablePath"/> doesn't match.</returns>
        public static bool? IsRunAtStartupEnabled(string valueName, string executablePath)
        {
            if (TryGetValue<string>(BaseKey.HKEY_CURRENT_USER, RunKeyPath, valueName, out var value))
            {
                return value.Equals(executablePath, StringComparison.OrdinalIgnoreCase)
                    ? true
                    : null;
            }
            else return false;
        }
        #endregion IsRunAtStartupEnabled

        #endregion Methods
    }
}
