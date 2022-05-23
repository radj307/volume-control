using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{
    /// <summary>
    /// This is an entirely optional object that maintains an internal <see cref="MemoryStream"/> and emulates the <see cref="LogWriter"/> object with a twist.<br/>
    /// This object can't flush the buffer to any endpoint directly, but can be passed to a <see cref="LogWriter"/> that will print the entire message in one operation.
    /// </summary>
    /// <remarks><b>Do not access this object asynchronously!</b></remarks>
    public class LogMessage : LogWriter, ILogWriter
    {
        /// <inheritdoc cref="LogMessage"/>
        /// <param name="filter">Bitfield flags that determine which types of log messages may be shown.</param>
        public LogMessage(EventType filter = EventType.ALL) : base(new MemoryEndpoint(true), filter)
        {
        }

        // TODO: Add the ability to write log messages without appending a newline so that messages can be followed up directly.
        //       This feature may only be available in LogMessage since it (must be) accessed synchronously
    }
}
