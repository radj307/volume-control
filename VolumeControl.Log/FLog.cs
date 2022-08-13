using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log
{
    /// <summary>Global static log manager object.</summary>
    public static class FLog
    {
        #region Fields
        private static LogWriter _log = null!;
        private static SettingsInterface Settings => SettingsInterface.Default;
        #endregion Fields

        #region Properties
        /// <summary>
        /// The LogWriter instance used by FLog.
        /// </summary>
        public static LogWriter Log
        {
            get
            {
                if (!Initialized)
                    Initialize();
                return _log;
            }
            private set => _log = value;
        }
        /// <summary>
        /// Sets whether or not the log is enabled.
        /// </summary>
        public static bool EnableLog
        {
            get
            {
                if (!Initialized)
                    Initialize();
                return _log.Endpoint.Enabled;
            }
            private set
            {
                if (!Initialized)
                    Initialize();
                _log.Endpoint.Enabled = value;
            }
        }
        /// <summary>
        /// Gets or sets the event type filter that determines which event types are allowed to be written to the log endpoint.
        /// </summary>
        /// <remarks><b>Messages with an <see cref="EventType"/> that isn't present in this bitflag are discarded!</b></remarks>
        public static EventType EventFilter { get; private set; }
        /// <summary>
        /// Get or set the log filepath.
        /// </summary>
        public static string? FilePath { get; private set; }
        /// <summary>
        /// True when <see cref="Initialize"/> has been called, and the log is ready.
        /// </summary>
        public static bool Initialized { get; private set; } = false;
        #endregion Properties

        #region Methods
        private static void Initialize()
        {
            if (Initialized)
                throw new Exception("Cannot call FLog.Initialize() or FLog.CustomInitialize() multiple times!");
            Initialized = true;

            // get the full filepath to the log
            FilePath = Settings.LogPath;
            // Set the event type filter
            EventFilter = Settings.LogFilter;

            var endpoint = new FileEndpoint(FilePath, Settings.EnableLogging);

            if (endpoint.Enabled && Settings.LogClearOnInitialize)
            {
                endpoint.Reset(); //< clear the log file
            }

#           if DEBUG
            EventFilter = EventType.ALL;            //< show all log messages
            endpoint.Enabled = true;                //< force enable logging
#           endif

            CreateLog(endpoint);
            WriteInitMessage("Initialized");

            Settings.PropertyChanged += HandlePropertyChanged!;
        }
        private static void WriteInitMessage(string log_____) => Log.WriteLine($"{Settings.LogTimestampFormat}{new string(' ', Timestamp.LineHeaderTotalLength - Settings.LogTimestampFormat.Length)}=== Log {log_____} @ {DateTime.UtcNow:U} ===  {{ Filter: {(byte)EventFilter} ({EventFilter:G}) }}");
        private static void CreateLog(IEndpoint endpoint) => Log = new(endpoint, EventFilter);
        private static void HandlePropertyChanged(object sender, EventArgs e)
        {
            bool enabled = Log.Endpoint.Enabled;
            if (enabled != Settings.EnableLogging || FilePath != Settings.LogPath || EventFilter != Settings.LogFilter)
            {
                EventFilter = Settings.LogFilter;
                FilePath = Settings.LogPath;
                enabled = Settings.EnableLogging;

                var endpoint = new FileEndpoint(FilePath, enabled);

                CreateLog(endpoint);
                WriteInitMessage("Re-Initialized");
            }
        }
        #endregion Methods
    }
}
