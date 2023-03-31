namespace VolumeControl.Log.Enum
{
    /// <summary>
    /// Determines the header used to print formatted log messages.
    /// </summary>
    [Flags]
    public enum EventType : byte
    {
        /// <summary>
        /// No event types. This is the 0 value for the EventType flagset.
        /// This EventType does not have an associated header, and may only be used for type filtering.
        /// If used anyway, it produces this header:  "[????]"
        /// </summary>
        NONE = 0,
        /// <summary>
        /// A debugging message that should only be shown when in debug mode.
        /// Produces header:  "[DEBUG]"
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// An informational message.
        /// Produces header:  "[INFO]"
        /// </summary>
        INFO = 2,
        /// <summary>
        /// A warning message.
        /// Produces header:  "[WARN]"
        /// </summary>
        WARN = 4,
        /// <summary>
        /// An error message.
        /// Produces header:  "[ERROR]"
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// A fatal error message.
        /// Produces header:  "[FATAL]"
        /// </summary>
        FATAL = 16,
        /// <summary>
        /// All event types, except for DEBUG.
        /// This EventType does not have an associated header, and may only be used for type filtering.
        /// If used anyway, it produces this header:  "[????]"
        /// </summary>
        ALL_EXCEPT_DEBUG = INFO | WARN | ERROR | FATAL,
        /// <summary>
        /// All event types, including DEBUG.
        /// This EventType does not have an associated header, and may only be used for type filtering.
        /// If used anyway, it produces this header:  "[????]"
        /// </summary>
        ALL = ALL_EXCEPT_DEBUG | DEBUG,
        /// <summary>
        /// A critical message that does not necessarily indicate failure, but cannot be prevented from appearing in the log by user settings.
        /// Produces header:  "[CRITICAL]"
        /// </summary>
        CRITICAL = 32,
        /// <summary>
        /// Extremely situational debug information.
        /// Produces header:  "[TRACE]"
        /// </summary>
        TRACE = 64,
    }
}
