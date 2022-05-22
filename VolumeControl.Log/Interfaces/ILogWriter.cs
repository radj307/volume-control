using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log.Interfaces
{
    /// <summary>
    /// Represents a log writer object, which is responsible for exposing logging methods, formatting the input, and writing it to an endpoint.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Gets or sets the endpoint for data written to the log.
        /// </summary>
        /// <remarks>This is a wrapper for the raw I/O functions used by the logging functions.</remarks>
        IEndpoint Endpoint { get; set; }
        /// <summary>
        /// Gets a timestamp in the correct format for this log writer instance.
        /// </summary>
        /// <returns>An <see cref="ITimestamp"/> representing the current time.</returns>
        ITimestamp MakeTimestamp(EventType eventType);
        /// <summary>
        /// Determines whether the given <see cref="EventType"/> should be written to the <see cref="Endpoint"/> or not.
        /// </summary>
        /// <param name="eventType">The event type to filter.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>The message should be written to the log endpoint.</description></item>
        /// <item><term>false</term><description>The message should be discarded.</description></item>
        /// </list></returns>
        bool FilterEventType(EventType eventType);
    }
}
