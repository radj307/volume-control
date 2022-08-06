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
        private SettingsInterface() => Settings.PropertyChanged += HandleSettingsPropertyChanged;
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
            get => (bool)Settings[nameof(EnableLogging)]!;
            set => Settings[nameof(EnableLogging)] = value;
        }
        public string LogPath
        {
            get => (string)Settings[nameof(LogPath)]!;
            set => Settings[nameof(LogPath)] = value;
        }
        public EventType LogFilter
        {
            get => (EventType)Settings[nameof(LogFilter)]!;
            set => Settings[nameof(LogFilter)] = value;
        }
        public bool LogClearOnInitialize
        {
            get => (bool)Settings[nameof(LogClearOnInitialize)]!;
            set => Settings[nameof(LogClearOnInitialize)] = value;
        }
        public string LogTimestampFormat
        {
            get => (string)Settings[nameof(LogTimestampFormat)]!;
            set => Settings[nameof(LogTimestampFormat)] = value;
        }
        public bool LogEnableStackTrace
        {
            get => (bool)Settings[nameof(LogEnableStackTrace)]!;
            set => Settings[nameof(LogEnableStackTrace)] = value;
        }
        public bool LogEnableStackTraceLineCount
        {
            get => (bool)Settings[nameof(LogEnableStackTraceLineCount)]!;
            set => Settings[nameof(LogEnableStackTraceLineCount)] = value;
        }
        #endregion Settings
        #endregion Properties
    }
}
