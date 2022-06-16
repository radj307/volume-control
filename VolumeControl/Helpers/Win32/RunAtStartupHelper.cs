using System;
using System.Linq;
using VolumeControl.Properties;
using VolumeControl.Log;
using Microsoft.Win32;

namespace VolumeControl.Helpers.Win32
{
    internal static class RunAtStartupHelper
    {
        #region Properties
        private static Settings Settings => Settings.Default;
        private static LogWriter Log => FLog.Log;
        private static string RunKeyFullPath => $"{Settings.RegistryRunAtStartupKeyFriendlyPrefix}{Settings.RegistryRunAtStartupKeyPath}";
        /// <summary>
        /// Gets or sets the registry value named by <see cref="Settings.RegistryRunAtStartupValueName"/>, located in the <see langword="HKEY_CURRENT_USER"/> subkey specified by <see cref="Settings.RegistryRunAtStartupKeyPath"/>
        /// </summary>
        public static string? Value
        {
            get
            {
                try
                {
                    if (Registry.CurrentUser.OpenSubKey(Settings.RegistryRunAtStartupKeyPath, false) is RegistryKey runkey)
                    {
                        if (runkey.GetValueNames().Contains(Settings.RegistryRunAtStartupValueName))
                        {
                            var valueKind = runkey.GetValueKind(Settings.RegistryRunAtStartupValueName);
                            if (valueKind.Equals(RegistryValueKind.String))
                                return runkey.GetValue(Settings.RegistryStartupValueName)?.ToString();
                            else Log.Warning($"{nameof(RunAtStartupHelper)}:  Unexpected type '{valueKind:G}' for value '{RunKeyFullPath}\\{Settings.RegistryRunAtStartupValueName}'; expected type '{RegistryValueKind.String:G}'");
                        }
                        // else; value doesn't exist so return null.
                    }
                    else Log.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for reading!");
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to read registry value named '{Settings.RegistryRunAtStartupValueName}' from registry key '{RunKeyFullPath}':", ex);
                }
                return null;
            }
            set
            {
                try
                {
                    if (Registry.CurrentUser.OpenSubKey(Settings.RegistryRunAtStartupKeyPath, true) is RegistryKey runkey)
                    {
                        if (value is null)
                        {
                            if (runkey.GetValueNames().Contains(Settings.RegistryRunAtStartupValueName))
                                runkey.DeleteValue(Settings.RegistryRunAtStartupValueName);
                            // else; value doesn't exist so don't try to delete it.
                        }
                        else runkey.SetValue(Settings.RegistryRunAtStartupValueName, value);
                    }
                    else Log.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for writing!");
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to write value named '{Settings.RegistryRunAtStartupValueName}' to registry key '{RunKeyFullPath}':", ex);
                }
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Checks if the <see cref="Value"/> property is equal to <paramref name="other"/>.<br/><br/>
        /// You can use this to compare a <see langword="string"/> to the registry value named by <see cref="Settings.RegistryRunAtStartupValueName"/> located in the CurrentUser subkey specified by <see cref="Settings.RegistryRunAtStartupKeyPath"/>.
        /// </summary>
        /// <param name="other">A string to compare to the <see cref="Value"/> property.</param>
        /// <returns><see langword="true"/> when (<see cref="Value"/> == <paramref name="other"/>); otherwise <see langword="false"/></returns>
        public static bool ValueEquals(string other) => other.Equals(Value);
        #endregion Methods
    }
}
