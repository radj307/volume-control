using AppConfig;
using System.ComponentModel;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log
{
    /// <summary>
    /// Provides a more convenient way to access the program configuration without having access to the VolumeControl.Core namespace.<br/>
    /// See the <see cref="Default"/> property.
    /// </summary>
    internal class SettingsInterface : INotifyPropertyChanged
    {
        #region Constructor
        private SettingsInterface() => Settings.PropertyChanged += this.HandleSettingsPropertyChanged;
        #endregion Constructor

        #region Fields
        private static readonly string[] _propertyNames = typeof(SettingsInterface).GetProperties().Where(p => p.CanRead && p.CanWrite).Select(p => p.Name).ToArray();
        #endregion Fields

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(object? sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
        private void HandleSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_propertyNames.Any(n => n.Equals(e.PropertyName, StringComparison.Ordinal)))
            {
                this.NotifyPropertyChanged(sender, e);
            }
        }
        #endregion Events

        #region Properties
        /// <summary>
        /// Default <see cref="SettingsInterface"/> instance.
        /// </summary>
        public static SettingsInterface Default { get; } = new();
        private static Configuration Settings => Configuration.Default;

        #region Settings
        public bool EnableLogging
        {
            get => (bool)Settings[nameof(this.EnableLogging)]!;
            set => Settings[nameof(this.EnableLogging)] = value;
        }
        public string LogPath
        {
            get => (string)Settings[nameof(this.LogPath)]!;
            set => Settings[nameof(this.LogPath)] = value;
        }
        public EventType LogFilter
        {
            get => (EventType)Settings[nameof(this.LogFilter)]!;
            set => Settings[nameof(this.LogFilter)] = value;
        }
        public bool LogClearOnInitialize
        {
            get => (bool)Settings[nameof(this.LogClearOnInitialize)]!;
            set => Settings[nameof(this.LogClearOnInitialize)] = value;
        }
        public string LogTimestampFormat
        {
            get => (string)Settings[nameof(this.LogTimestampFormat)]!;
            set => Settings[nameof(this.LogTimestampFormat)] = value;
        }
        public bool LogEnableStackTrace
        {
            get => (bool)Settings[nameof(this.LogEnableStackTrace)]!;
            set => Settings[nameof(this.LogEnableStackTrace)] = value;
        }
        public bool LogEnableStackTraceLineCount
        {
            get => (bool)Settings[nameof(this.LogEnableStackTraceLineCount)]!;
            set => Settings[nameof(this.LogEnableStackTraceLineCount)] = value;
        }
        #endregion Settings
        #endregion Properties
    }
}
