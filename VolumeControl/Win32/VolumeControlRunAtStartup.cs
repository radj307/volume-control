using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Log;

namespace VolumeControl.Win32
{
    public class VolumeControlRunAtStartup : IDisposable, INotifyPropertyChanged
    {
        public VolumeControlRunAtStartup()
        {
            _runKeyHelper = new();
        }

        private static Properties.Settings Settings => Properties.Settings.Default;
        private static LogWriter Log => FLog.Log;

        private readonly RunKeyHelper _runKeyHelper;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        public string ExecutablePath { get; set; } = string.Empty;

        public bool RunAtStartup
        {
            get => Exists();
            set
            {
                if (value) // ADD
                {
                    _runKeyHelper.EnableRunAtStartup(Settings.RegistryStartupValueName, ExecutablePath);
                    Log.Info($"{nameof(RunAtStartup)} was enabled using path: '{ExecutablePath}'");
                }
                else // REMOVE
                {
                    _runKeyHelper.DisableRunAtStartup(Settings.RegistryStartupValueName);
                    Log.Info($"{nameof(RunAtStartup)} was disabled.");
                }

                Settings.RunAtStartup = value;
                Settings.Save();
                Settings.Reload();

                NotifyPropertyChanged();
            }
        }

        private bool Exists() => _runKeyHelper.CheckRunAtStartup(Settings.RegistryStartupValueName);

        public void Dispose()
        {
            _runKeyHelper.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
