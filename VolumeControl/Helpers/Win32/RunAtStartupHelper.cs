using Microsoft.Win32;
using System;
using System.Linq;
using VolumeControl.Log;

namespace VolumeControl.Helpers.Win32
{
    internal static class RunAtStartupHelper
    {
        #region Private Constants
        /// <summary>
        /// The friendly registry path prefix shown to users for use in `regedit.msc`
        /// </summary>
        private const string RegistryRunAtStartupKeyFriendlyPrefix = @"Computer\HKEY_CURRENT_USER\";
        /// <summary>
        /// The path to the Run registry key.<br/>
        /// This is relative to the <see cref="Registry.CurrentUser"/> key.
        /// </summary>
        private const string RegistryRunAtStartupKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        /// <summary>
        /// The name of the registry value for VolumeControl's run-on-startup feature.<br/>
        /// The actual value of the registry value is the filepath pointing to `VolumeControl.exe`.<br/>
        /// This name is used by Windows 
        /// </summary>
        private const string RegistryRunAtStartupValueName = "VolumeControl";
        #endregion Private Constants

        #region Properties
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
                            RegistryValueKind valueKind = runkey.GetValueKind(RegistryRunAtStartupValueName);
                            if (valueKind.Equals(RegistryValueKind.String))
                                return runkey.GetValue(RegistryRunAtStartupValueName)?.ToString();
                            else FLog.Warning($"{nameof(RunAtStartupHelper)}:  Unexpected type '{valueKind:G}' for value '{RunKeyFullPath}\\{RegistryRunAtStartupValueName}'; expected type '{RegistryValueKind.String:G}'");
                        }
                        // else; value doesn't exist so return null.
                    }
                    else
                    {
                        FLog.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for reading!");
                    }
                }
                catch (Exception ex)
                {
                    FLog.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to read registry value named '{RegistryRunAtStartupValueName}' from registry key '{RunKeyFullPath}':", ex);
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
                        else
                        {
                            runkey.SetValue(RegistryRunAtStartupValueName, value);
                        }
                    }
                    else
                    {
                        FLog.Error($"{nameof(RunAtStartupHelper)}:  Failed to open registry key '{RunKeyFullPath}' for writing!");
                    }
                }
                catch (Exception ex)
                {
                    FLog.Error($"{nameof(RunAtStartupHelper)}:  Exception thrown while attempting to write value named '{RegistryRunAtStartupValueName}' to registry key '{RunKeyFullPath}':", ex);
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
        public static bool ValueEquals(string other) => other.Equals(Value, StringComparison.Ordinal);
        /// <summary>
        /// Checks if <see cref="Value"/> is set to <see langword="null"/>.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="Value"/> is <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public static bool ValueEqualsNull() => Value == null;
        #endregion Methods
    }
}
