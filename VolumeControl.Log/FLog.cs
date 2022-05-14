using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;
using VolumeControl.Log.Properties;

namespace VolumeControl.Log
{
    public static class FLog
    {
        #region Members
        private static string _filepath = string.Empty;
        private static EventType _filter = EventType.NONE;
        private static bool _initialized = false;
        private static LogWriter _log = null!;
        #endregion Members

        #region Properties
        /// <summary>
        /// The LogWriter instance used by FLog.
        /// </summary>
        /// <remarks>Using this before calling the <see cref="Initialize"/> function will throw an exception!</remarks>
        public static LogWriter Log
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return _log;
            }
            internal set => _log = value;
        }
        /// <summary>
        /// Sets whether or not the log is enabled.
        /// </summary>
        /// <remarks>Using this before calling the <see cref="Initialize"/> function will throw an exception!</remarks>
        public static bool EnableLog
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return _log.Endpoint.Enabled;
            }
            set
            {
                if (!_initialized)
                    Initialize();
                _log.Endpoint.Enabled = value;
            }
        }
        /// <summary>
        /// Gets or sets the event type filter that determines which event types are allowed to be written to the log endpoint.
        /// </summary>
        /// <remarks><b>Messages with an <see cref="EventType"/> that isn't present in this bitflag are discarded!</b></remarks>
        public static EventType EventFilter
        {
            get => _filter;
            set => _filter = value;
        }
        /// <summary>
        /// Get or set the log filepath.
        /// </summary>
        public static string FilePath
        {
            get => _filepath;
            set => _filepath = value;
        }
        /// <summary>
        /// True when <see cref="Initialize"/> has been called, and the log is ready.
        /// </summary>
        public static bool Initialized => _initialized;
        #endregion Properties

        #region Methods
        public static void Initialize()
        {
            if (_initialized)
                throw new Exception("Cannot call FLog.Initialize() multiple times!");
            _initialized = true;

            // Add properties to the settings file
            Settings.Default.Save();
            Settings.Default.Reload();

            // get the full filepath to the log
            FilePath = Path.Combine(Settings.Default.LogDir, Settings.Default.LogFileName);
            // Set the event type filter
            EventFilter = (EventType)Settings.Default.LogAllowedEventTypeFlag;

            var endpoint = new FileEndpoint(FilePath, Settings.Default.EnableLogging);

#           if DEBUG
            FilePath = Settings.Default.LogFileName;//< use working directory
            EventFilter = EventType.ALL;            //< show all log messages
            endpoint.Enabled = true;                //< force enable logging
#           endif

            if (endpoint.Enabled)
            {
                endpoint.Reset(); //< clear the log file
            }

            Log = new(endpoint, EventFilter);

            Log.WriteLine($"{Settings.Default.TimestampFormat}{new string(' ', Timestamp.LineHeaderTotalLength - Settings.Default.TimestampFormat.Length)}=== Log Initialized ===  {{ Filter: {EventFilter.ID()} ({EventFilter:G}) }})");
        }
        #endregion Methods
    }
}
