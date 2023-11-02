namespace VolumeControl.Log
{
    /// <summary>
    /// Determines the header used to print formatted log messages.
    /// </summary>
    [Flags]
    public enum EventType : byte
    {
        /// <summary>
        /// Not an event type.
        /// </summary>
        NONE = 0,
        /// <summary>
        /// Message contains debugging information.
        /// </summary>
        /// <remarks>
        /// For debug info that is only useful in certain situations, see <see cref="TRACE"/>.
        /// </remarks>
        DEBUG = 1,
        /// <summary>
        /// Message contains information that isn't related to an error or warning.
        /// </summary>
        INFO = 2,
        /// <summary>
        /// Messages related to a warning or very minor error.
        /// </summary>
        WARN = 4,
        /// <summary>
        /// Messages related to an error.
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// Messages related to a significant error of critical importance.
        /// </summary>
        /// <remarks>
        /// For errors that the application cannot recover from, see <see cref="FATAL"/>.
        /// </remarks>
        CRITICAL = 16,
        /// <summary>
        /// Messages related to a significant error that the application cannot recover from.
        /// </summary>
        /// <remarks>
        /// For significant errors that did not cause the application to exit unexpectedly, see <see cref="CRITICAL"/>.
        /// </remarks>
        FATAL = 32,
        /// <summary>
        /// Message contains debugging information that is only situationally useful.
        /// </summary>
        /// <remarks>
        /// For debug info that is generally useful in most or all case, see <see cref="DEBUG"/>.
        /// </remarks>
        TRACE = 64,
        /// <summary>
        /// All event types.
        /// </summary>
        ALL = DEBUG | INFO | WARN | ERROR | CRITICAL | FATAL | TRACE,
    }
}
