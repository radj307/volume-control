using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Exceptions;

namespace VolumeControl.Log
{
    /// <summary>
    /// Static file logger class.
    /// </summary>
    /// <remarks>
    /// The <see cref="Initialize"/> method must be called before accessing any properties.
    /// </remarks>
    public static class FLog
    {
        #region Fields
        private static bool _initialized = false;
        private static AsyncLogWriter _logWriter = null!;
        #endregion Fields

        #region Properties
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
        /// <summary>
        /// Gets whether this log has been initialized yet or not.
        /// </summary>
        /// <remarks>
        /// This does not throw exceptions.
        /// </remarks>
        public static bool IsInitialized => _initialized;
        #endregion Properties

        #region Methods

        #region (Private)
        /// <summary>
        /// Creates a new string that contains the log init message that appears on the first line of the log.
        /// </summary>
        /// <param name="verb">Displayed in the header as "Log (verb)"</param>
        private static string MakeInitMessage(string verb)
            => $"{AsyncLogWriter.DateTimeFormatString}{AsyncLogWriter.Indent(AsyncLogWriter.TimestampLength, AsyncLogWriter.DateTimeFormatString.Length)}{new string(' ', AsyncLogWriter.EventTypeLength)}=== Log {verb} @ {DateTime.UtcNow:U} ===  {{ LogFilter: {(int)Log.EventTypeFilter} ({Log.EventTypeFilter:G}) }}";
        private static void WriteLineRaw(string text)
        {
            lock (Log.Endpoint)
            {
                Log.Endpoint.WriteLine(text);
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
        public static void Initialize(string path, bool enable, EventType logFilter, bool deleteExisting = true)
        {
            if (_initialized) // already initialized
                throw new InvalidOperationException($"{nameof(FLog)} is already initialized! {nameof(Initialize)}() can only be called once.");

            _logWriter = new(new PersistentFileEndpoint(path, enable), logFilter);

            _initialized = true; //< Log is valid here

            if (deleteExisting)
            {
                Log.ResetEndpoint(MakeInitMessage("Initialized"));
            }
            else WriteLineRaw(MakeInitMessage("Initialized"));
        }
        #endregion Initialize

        #region ChangeLogPath
        /// <summary>
        /// Sets the log path to a new location.
        /// </summary>
        /// <param name="newPath">The filepath to change the log path to.</param>
        public static void ChangeLogPath(string newPath)
        {
            if (!_initialized)
                throw MakeLogNotInitializedException();

            ((FileEndpoint)Log.Endpoint).Path = newPath;
        }
        #endregion ChangeLogPath

        #region AsyncLogWriter Methods
        /// <inheritdoc cref="AsyncLogWriter.FilterEventType(EventType)"/>
        public static bool FilterEventType(EventType eventType) => Log.FilterEventType(eventType);
        /// <inheritdoc cref="AsyncLogWriter.LogMessage(Log.LogMessage)"/>
        public static void LogMessage(LogMessage logMessage) => Log.LogMessage(logMessage);
        /// <inheritdoc cref="AsyncLogWriter.Trace(object?[])"/>
        public static bool Trace(params object?[] lines) => Log.LogMessage(new(EventType.TRACE, lines));
        /// <inheritdoc cref="AsyncLogWriter.Trace(Log.LogMessage)"/>
        public static bool Trace(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.TRACE));
        /// <inheritdoc cref="AsyncLogWriter.Debug(object?[])"/>
        public static bool Debug(params object?[] lines) => Log.LogMessage(new(EventType.DEBUG, lines));
        /// <inheritdoc cref="AsyncLogWriter.Debug(Log.LogMessage)"/>
        public static bool Debug(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.DEBUG));
        /// <inheritdoc cref="AsyncLogWriter.Info(object?[])"/>
        public static bool Info(params object?[] lines) => Log.LogMessage(new(EventType.INFO, lines));
        /// <inheritdoc cref="AsyncLogWriter.Info(Log.LogMessage)"/>
        public static bool Info(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.INFO));
        /// <inheritdoc cref="AsyncLogWriter.Warning(object?[])"/>
        public static bool Warning(params object?[] lines) => Log.LogMessage(new(EventType.WARN, lines));
        /// <inheritdoc cref="AsyncLogWriter.Warning(Log.LogMessage)"/>
        public static bool Warning(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.WARN));
        /// <inheritdoc cref="AsyncLogWriter.Error(object?[])"/>
        public static bool Error(params object?[] lines) => Log.LogMessage(new(EventType.ERROR, lines));
        /// <inheritdoc cref="AsyncLogWriter.Error(Log.LogMessage)"/>
        public static bool Error(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.ERROR));
        /// <inheritdoc cref="AsyncLogWriter.Fatal(object?[])"/>
        public static bool Fatal(params object?[] lines) => Log.LogMessage(new(EventType.FATAL, lines));
        /// <inheritdoc cref="AsyncLogWriter.Fatal(Log.LogMessage)"/>
        public static bool Fatal(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.FATAL));
        /// <inheritdoc cref="AsyncLogWriter.Critical(object?[])"/>
        public static bool Critical(params object?[] lines) => Log.LogMessage(new(EventType.CRITICAL, lines));
        /// <inheritdoc cref="AsyncLogWriter.Critical(Log.LogMessage)"/>
        public static bool Critical(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.CRITICAL));
        /// <inheritdoc cref="AsyncLogWriter.Blank(object?[])"/>
        public static bool Blank(params object?[] lines) => Log.LogMessage(new(EventType.NONE, lines));
        /// <inheritdoc cref="AsyncLogWriter.Blank(Log.LogMessage)"/>
        public static bool Blank(LogMessage logMessage) => Log.LogMessage(logMessage.SetEventType(EventType.NONE));
        #endregion AsyncLogWriter Methods

        #endregion Methods
    }
}
