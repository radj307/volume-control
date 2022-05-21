using Microsoft.Win32;
using System;

namespace VolumeControl.Win32
{
    public class RunKeyHelper : IDisposable
    {
        public bool CheckRunAtStartup(string valueName)
        {
            return RunKey.GetValue(valueName) != null;
        }

        public bool CheckRunAtStartup(string valueName, string executablePath)
        {
            return RunKey.GetValue(valueName)?.Equals(executablePath) ?? false;
        }

        public void EnableRunAtStartup(string valueName, string executablePath)
        {
            RunKey.SetValue(valueName, executablePath);
        }

        public void DisableRunAtStartup(string valueName)
        {
            RunKey.DeleteValue(valueName);
        }

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
