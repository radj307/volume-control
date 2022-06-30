using System;
using System.Linq;
using VolumeControl.Properties;
using VolumeControl.Log;
using Microsoft.Win32;
using VolumeControl.Core;

namespace VolumeControl.Helpers.Win32
{
    internal static class RunAtStartupHelper
    {
        private const string RegistryRunAtStartupKeyFriendlyPrefix = @"Computer\HKEY_CURRENT_USER\";
        private const string RegistryRunAtStartupKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RegistryRunAtStartupValueName = "VolumeControl";

        #region Properties
        private static LogWriter Log => FLog.Log;
        private static string RunKeyFullPath => $"{RegistryRunAtStartupKeyFriendlyPrefix}{RegistryRunAtStartupKeyPath}";
        /// <summary>
        /// Gets or sets the registry value named by <see cref="RegistryRunAtStartupValueName"/>, located in the <see langword="HKEY_CURRENT_USER"/> subkey specified by <see cref="RegistryRunAtStartupKeyPath"/>
        /// </summary>
        public static string? Value
        {
            get
            {
                try
                {
                    if (Registry.CurrentUser.OpenSubKey(RegistryRunAtStartupKeyPath, false) is RegistryKey runkey)
                    {
                        if (runkey.GetValueNames().Contains(RegistryRunAtStartupValueName))
                        {
                            var valueKind = runkey.GetValueKind(RegistryRunAtStartupValueName);
                            if (valueKind.Equals(RegistryValueKind.String))
                                return runkey.GetValue(RegistryRunAtStartupValueName)?.ToString();
                            else Log.Warning($"{nameof(RunAtStartupHelper)}:  Unexpected type '{valueKind:G}' for value '{RunKeyFullPath}\\{RegistryRunAtStartupValueName}'; expected type '{RegistryValueKind.String:G}'");
                        }
                        // else; value doesn't exist so return null.
                    }
                    else Log.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for reading!");
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to read registry value named '{RegistryRunAtStartupValueName}' from registry key '{RunKeyFullPath}':", ex);
                }
                return null;
            }
            set
            {
                try
                {
                    if (Registry.CurrentUser.OpenSubKey(RegistryRunAtStartupKeyPath, true) is RegistryKey runkey)
                    {
                        if (value is null)
                        {
                            if (runkey.GetValueNames().Contains(RegistryRunAtStartupValueName))
                                runkey.DeleteValue(RegistryRunAtStartupValueName);
                            // else; value doesn't exist so don't try to delete it.
                        }
                        else runkey.SetValue(RegistryRunAtStartupValueName, value);
                    }
                    else Log.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for writing!");
                }
                catch (Exception ex)
                {
                    Log.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to write value named '{RegistryRunAtStartupValueName}' to registry key '{RunKeyFullPath}':", ex);
                }
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Checks if the <see cref="Value"/> property is equal to <paramref name="other"/>.<br/><br/>
        /// You can use this to compare a <see langword="string"/> to the registry value named by <see cref="RegistryRunAtStartupValueName"/> located in the CurrentUser subkey specified by <see cref="RegistryRunAtStartupKeyPath"/>.
        /// </summary>
        /// <param name="other">A string to compare to the <see cref="Value"/> property.</param>
        /// <returns><see langword="true"/> when (<see cref="Value"/> == <paramref name="other"/>); otherwise <see langword="false"/></returns>
        public static bool ValueEquals(string other) => other.Equals(Value);
        #endregion Methods
    }
}
