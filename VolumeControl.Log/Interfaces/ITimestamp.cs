using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;

namespace VolumeControl.Log.Interfaces
{
    /// <summary>
    /// Interface for the timestamps on each log message, which contain a <see cref="DateTime"/> and <see cref="Enum.EventType"/>.
    /// </summary>
    public interface ITimestamp
    {
        /// <summary>
        /// Gets or sets the point in time that the timestamp represents.
        /// </summary>
        DateTime TimePoint { get; set; }
        /// <summary>
        /// Gets or sets the event type shown at the end of the timestamp in the log output.
        /// </summary>
        EventType EventType { get; set; }
        /// <summary>
        /// Gets the <see cref="TimePoint"/> margin size, which determines the amount of space reserved in the full timestamp for the <see cref="TimePoint"/>.
        /// </summary>
        int MarginTimePoint { get; }
        /// <summary>
        /// Gets the <see cref="EventType"/> margin size, which determines the amount of space reserved in the full timestamp for the <see cref="EventType"/>.
        /// </summary>
        int MarginEventType { get; }
        string ToString();
        /// <summary>
        /// Creates a log message header from a <see cref="ITimestamp"/> source interface.
        /// </summary>
        /// <param name="ts">The <see cref="Timestamp"/> to use for the header.</param>
        /// <returns>A <see cref="string"/> with the time-point and event type specified by the <see cref="ITimestamp"/>.</returns>
        static string MakeHeader(ITimestamp timestamp, string format)
        {
            string ts = timestamp.TimePoint.ToString(format);
            string head = timestamp.EventType.GetHeader();
            return $"{ts}{new string(' ', timestamp.MarginTimePoint - ts.Length)} {head}{new string(' ', timestamp.MarginEventType - head.Length)}";
        }
    }
}
