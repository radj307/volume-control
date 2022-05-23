using Microsoft.Win32;
using System;
using VolumeControl.Log;

namespace VolumeControl.Win32
{
    public class RunKeyHelper : IDisposable
    {
        private static LogWriter Log => FLog.Log;

        public object? GetRunAtStartup(string valueName, object? defaultValue = null)
        {
            try
            {
                return RunKey.GetValue(valueName, defaultValue, RegistryValueOptions.None);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return defaultValue;
        }

        public bool CheckRunAtStartup(string valueName) => GetRunAtStartup(valueName, null) != null;

        public bool CheckRunAtStartup(string valueName, string executablePath) => GetRunAtStartup(valueName, null)?.Equals(executablePath) ?? false;

        public void EnableRunAtStartup(string valueName, string executablePath) => RunKey.SetValue(valueName, executablePath);

        public void DisableRunAtStartup(string valueName) => RunKey.DeleteValue(valueName);

        private RegistryKey? _runKey = null;
        protected RegistryKey RunKey => _runKey ??= RegistryAPI.GetKey(RegistryAPI.Scope.HKEY_CURRENT_USER, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;

        /// <inheritdoc/>
        public void Dispose()
        {
            _runKey?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
