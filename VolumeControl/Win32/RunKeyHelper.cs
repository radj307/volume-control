using Microsoft.Win32;
using System;

namespace VolumeControl.Win32
{
    public class RunKeyHelper : IDisposable
    {
        public bool CheckRunAtStartup(string valueName) => RunKey.GetValue(valueName) != null;
        public void EnableRunAtStartup(string valueName, string executablePath) => RunKey.SetValue(valueName, executablePath);
        public void DisableRunAtStartup(string valueName) => RunKey.DeleteValue(valueName);


        private RegistryKey? _runKey = null;
        protected RegistryKey RunKey
        {
            get => _runKey ??= RegistryAPI.GetKey(RegistryAPI.Scope.HKEY_CURRENT_USER, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;
        }

        public void Dispose()
        {
            _runKey?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
