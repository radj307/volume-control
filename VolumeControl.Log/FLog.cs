using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log
{
    /// <summary>
    /// Static logger class.
    /// </summary>
    /// <remarks>
    /// The <see cref="Initialize"/> method must be called before accessing any properties, and after the settings have initialized.
    /// </remarks>
    public static class FLog
    {
        #region Fields
        private static bool _initialized = false;
        private static AsyncLogWriter _logWriter = null!;
        #endregion Fields

        #region Properties
        private static SettingsInterface SettingsInterface => SettingsInterface.Default;
        /// <summary>
        /// Gets the <see cref="AsyncLogWriter"/> instance.
        /// </summary>
        /// <remarks>
        /// To preserve thread-safety, the <see cref="FileEndpoint"/> cannot be accessed through the log writer. It can be configured via the relevant setting in the Config.
        /// </remarks>
        public static AsyncLogWriter Log
        {
            get
            {
                if (!_initialized)
                    throw MakeLogNotInitializedException();
                return _logWriter;
            }
        }
        /// <inheritdoc cref="AsyncLogWriter.IsAsyncEnabled"/>
        public static bool IsAsyncEnabled
        {
            get
            {
                if (!_initialized)
                    throw MakeLogNotInitializedException();
                return _logWriter.IsAsyncEnabled;
            }
            set
            {
                if (!_initialized)
                    throw MakeLogNotInitializedException();
                _logWriter.IsAsyncEnabled = value;
            }
        }
        #endregion Properties

        #region Methods

        #region (Private)
        /// <summary>
        /// Creates a new string that contains the log init message that appears on the first line of the log.
        /// </summary>
        /// <param name="verb">Displayed in the header as "Log (verb)"</param>
        private static string MakeInitMessage(string verb)
            => $"{AsyncLogWriter.DateTimeFormatString}{AsyncLogWriter.Indent(AsyncLogWriter.TimestampLength, AsyncLogWriter.DateTimeFormatString.Length)}{new string(' ', AsyncLogWriter.EventTypeLength)}=== Log {verb} @ {DateTime.UtcNow:U} ===  {{ {nameof(SettingsInterface.LogFilter)}: {(int)Log.EventTypeFilter} ({Log.EventTypeFilter:G}) }}{Environment.NewLine}";
        private static void WriteRaw(string text)
        {
            lock (Log.Endpoint)
            {
                Log.Endpoint.WriteRaw(text);
            }
        }
        private static NotInitializedException MakeLogNotInitializedException(Exception? innerException = null)
            => new(nameof(FLog), "The log hasn't been initialized yet!", innerException);
        #endregion (Private)

        #region Initialize
        /// <summary>
        /// Initializes the log. The settings object must be created before calling this method. This method can only be called once.
        /// </summary>
        /// <exception cref="InvalidOperationException">The method has already been called before.</exception>
        /// <exception cref="NotInitializedException">The default config object hasn't been initialized yet.</exception>
        public static void Initialize()
        {
            if (_initialized) // already initialized
                throw new InvalidOperationException($"{nameof(FLog)} is already initialized! {nameof(Initialize)}() can only be called once.");
            else if (SettingsInterface == null) // settings aren't initialized yet
                throw new NotInitializedException(nameof(SettingsInterface), $"The default {nameof(AppConfig.Configuration)} instance must be initialized prior to calling {nameof(Initialize)}()!");

            // attach property changed handler to the settings object so we can react to changes
            SettingsInterface.PropertyChanged += SettingsInterface_PropertyChanged;

            _logWriter = new(new FileEndpoint(SettingsInterface.LogPath, SettingsInterface.EnableLogging), SettingsInterface.LogFilter);

            _initialized = true; //< Log is valid here

            if (SettingsInterface.LogClearOnInitialize)
            {
                Log.ResetEndpoint(MakeInitMessage("Initialized"));
            }
            else WriteRaw(MakeInitMessage("Initialized"));
        }
        #endregion Initialize

        #region AsyncLogWriter Methods
        /// <inheritdoc cref="AsyncLogWriter.FilterEventType(EventType)"/>
        public static bool FilterEventType(EventType eventType) => Log.FilterEventType(eventType);
        /// <inheritdoc cref="AsyncLogWriter.LogMessage(Log.LogMessage)"/>
        public static void LogMessage(LogMessage logMessage) => Log.LogMessage(logMessage);
        /// <inheritdoc cref="AsyncLogWriter.Trace"/>
        public static void Trace(params object?[] lines) => Log.LogMessage(new(EventType.TRACE, lines));
        /// <inheritdoc cref="AsyncLogWriter.Debug(object?[])"/>
        public static void Debug(params object?[] lines) => Log.LogMessage(new(EventType.DEBUG, lines));
        /// <inheritdoc cref="AsyncLogWriter.Info"/>
        public static void Info(params object?[] lines) => Log.LogMessage(new(EventType.INFO, lines));
        /// <inheritdoc cref="AsyncLogWriter.Warning"/>
        public static void Warning(params object?[] lines) => Log.LogMessage(new(EventType.WARN, lines));
        /// <inheritdoc cref="AsyncLogWriter.Error(object?[])"/>
        public static void Error(params object?[] lines) => Log.LogMessage(new(EventType.ERROR, lines));
        /// <inheritdoc cref="AsyncLogWriter.Fatal(object?[])"/>
        public static void Fatal(params object?[] lines) => Log.LogMessage(new(EventType.FATAL, lines));
        /// <inheritdoc cref="AsyncLogWriter.Critical(object?[])"/>
        public static void Critical(params object?[] lines) => Log.LogMessage(new(EventType.CRITICAL, lines));
        /// <inheritdoc cref="AsyncLogWriter.Blank(object?[])"/>
        public static void Blank(params object?[] lines) => Log.LogMessage(new(EventType.NONE, lines));
        #endregion AsyncLogWriter Methods

        #endregion Methods

        #region EventHandlers

        #region SettingsInterface
        private static void SettingsInterface_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || !_initialized) return;

            if (e.PropertyName.Equals(nameof(SettingsInterface.EnableLogging), StringComparison.Ordinal))
            {
                Log.EndpointEnabled = SettingsInterface.EnableLogging;
            }
            else if (e.PropertyName.Equals(nameof(SettingsInterface.LogPath), StringComparison.Ordinal))
            {
                lock (Log.Endpoint)
                {
                    ((FileEndpoint)Log.Endpoint).Path = SettingsInterface.LogPath;
                }
            }
            else if (e.PropertyName.Equals(nameof(SettingsInterface.LogFilter), StringComparison.Ordinal))
            {
                Log.EventTypeFilter = SettingsInterface.LogFilter;

                WriteRaw(MakeInitMessage("Filter Changed"));
            }
        }
        #endregion SettingsInterface

        #endregion EventHandlers
    }
}
