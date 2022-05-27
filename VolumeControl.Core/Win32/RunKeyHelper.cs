using Microsoft.Win32;

namespace VolumeControl.Core.Win32
{
    /// <summary>Helper object for interacting with the Windows registry.<br/>See <see cref="RunKey"/>.</summary>
    internal class RunKeyHelper : IDisposable
    {
        public object? GetRunAtStartup(string valueName, object? defaultValue = null)
        {
            try
            {
                return RunKey.GetValue(valueName, defaultValue, RegistryValueOptions.None);
            }
            catch (Exception) { return defaultValue; }
        }
        /// <summary>
        /// Checks if the registry value named <paramref name="valueName"/> exists.
        /// </summary>
        /// <param name="valueName">The name of the target registry value within <see cref="RunKey"/>.</param>
        /// <returns><see langword="true"/> when <paramref name="valueName"/> exists; otherwise <see langword="false"/>.</returns>
        public bool CheckRunAtStartup(string valueName) => GetRunAtStartup(valueName, null) != null;
        /// <summary>
        /// Checks if the registry value named <paramref name="valueName"/> is set to <paramref name="executablePath"/>.
        /// </summary>
        /// <param name="valueName">The name of the target registry value within <see cref="RunKey"/>.</param>
        /// <param name="executablePath">The expected content of the registry value.</param>
        /// <returns><see langword="true"/> when <paramref name="valueName"/> exists &amp; is set to <paramref name="executablePath"/>; otherwise <see langword="false"/>.</returns>
        public bool CheckRunAtStartup(string valueName, string executablePath) => GetRunAtStartup(valueName, null)?.Equals(executablePath) ?? false;
        /// <summary>
        /// Sets a registry value named <paramref name="valueName"/> to <paramref name="executablePath"/>.
        /// </summary>
        /// <param name="valueName">The name of the target registry value within <see cref="RunKey"/>.</param>
        /// <param name="executablePath">The absolute filepath of the application that should run when windows starts.</param>
        public void EnableRunAtStartup(string valueName, string executablePath) => RunKey.SetValue(valueName, executablePath);
        /// <summary>
        /// Deletes a registry value named <paramref name="valueName"/>.
        /// </summary>
        /// <param name="valueName">The name of the target registry value within <see cref="RunKey"/>.</param>
        public void DisableRunAtStartup(string valueName)
        {
            if (CheckRunAtStartup(valueName))
                RunKey.DeleteValue(valueName);
        }

        /// <summary>
        /// Lazily-initialized <see cref="RegistryKey"/> object set to: <b>HKEY_CURRENT_USER => SOFTWARE\Microsoft\Windows\CurrentVersion\Run</b>
        /// </summary>
        protected RegistryKey RunKey => _runKey ??= RegistryAPI.GetKey(RegistryAPI.Scope.HKEY_CURRENT_USER, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)!;
        private RegistryKey? _runKey = null;

        /// <inheritdoc/>
        public void Dispose()
        {
            _runKey?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
