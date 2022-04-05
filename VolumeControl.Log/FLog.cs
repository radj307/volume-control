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
        private static LogWriter<FileEndpoint> _log = null!;
        #endregion Members

        #region Properties
        /// <summary>
        /// The LogWriter instance used by FLog.
        /// Note: Using this before calling the 'Initialize()' function will throw an exception!
        /// </summary>
        public static LogWriter<FileEndpoint> Log
        {
            get
            {
                if (!_initialized) throw new Exception("Cannot write log messages; Log wasn't initialized! (Call FLog.Initialize() first!)");
                else return _log;
            }
            internal set => _log = value;
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
        /// True when FLog.Initialize() has been called, and the log is ready.
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

            Log = new(new FileEndpoint(_filepath), _filter);
            

            Log.WriteInfo(new string[] {
                "FLog.Initialize() Completed:",
                $"logfile   = '{_filepath}'",
                $"logfilter = '{_filter.ID()}'"
            });
        }
        public static void SaveSettings()
        {
            Properties.Settings.Default.logfile = _filepath;
            Properties.Settings.Default.logfilter = _filter.ID();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            if (_log != null)
            {
                Log.WriteInfo(new string[] {
                    "FLog.SaveSettings() Completed:",
                    $"logfile   = '{_filepath}'",
                    $"logfilter = '{_filter.ID()}'"
                });
            }
        }
        #endregion Methods
    }
}
