using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;

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
                if (!_initialized) throw new Exception("Cannot write log messages; Log wasn't initialized! (Call FLog.Initialize() first!)");
                else return _log;
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
                if (!_initialized) throw new Exception("Cannot check if the log is enabled; Log wasn't initialized! (Call FLog.Initialize() first!)");
                else return _log.Endpoint.Enabled;
            }
            set
            {
                if (!_initialized) throw new Exception("Cannot enable or disable log; Log wasn't initialized! (Call FLog.Initialize() first!)");
                else _log.Endpoint.Enabled = value;
            }
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
        /// Get or set the log event type filter.
        /// </summary>
        public static EventType Filter
        {
            get => _filter;
            set => _filter = value;
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
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            _filepath = Properties.Settings.Default.logfile;
#           if DEBUG // always show all log messages in debug mode, ignore properties
            _filter = EventType.ALL;
#           else
            _filter = (EventType)Properties.Settings.Default.logfilter;
#           endif

            // this is ok in C# because who needs logic
            var endpoint = new FileEndpoint(_filepath);

#           if DEBUG // force enable log when running in DEBUG configuration
            endpoint.Enabled = true;
#           endif

            if (endpoint.Enabled || Properties.Settings.Default.EnableLog)
            {
                endpoint.Enabled = true;
                endpoint.Reset();
            }
            Log = new(endpoint, _filter);


            Log.Info(
                "FLog.Initialize() Completed:",
                $"logfile   = '{_filepath}'",
                $"logfilter = '{_filter.ID()}'"
            );
        }
        #endregion Methods
    }
}
