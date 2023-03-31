using VolumeControl.Log.Enum;

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
        /// Gets the number of characters of each line dedicated to displaying the value of <see cref="TimePoint"/>.<br/>
        /// <b>Note that this mush account for the spacing between the TimePoint and the EventType as there is no implicit padding!</b>
        /// </summary>
        /// <remarks>This, combined with <see cref="EventTypeSegmentLength"/> determines the total length of the header portion of each line./// For the total number of characters required to align text with the message portion of each line, include <see cref="MarginSegmentLength"/> as well.</remarks>
        int TimePointSegmentLength { get; }
        /// <summary>
        /// Gets the number of characters of each line dedicated to displaying the value of <see cref="EventType"/>.<br/>
        /// <b>Note that this must account for any additional formatting done to event types like wrapping them with square brackets!</b>
        /// </summary>
        /// <remarks>This, combined with <see cref="TimePointSegmentLength"/> determines the total length of the header portion of each line.<br/>/// For the total number of characters required to align text with the message portion of each line, include <see cref="MarginSegmentLength"/> as well.</remarks>
        int EventTypeSegmentLength { get; }
        /// <summary>
        /// Gets the number of space characters between line headers and their associated messages.
        /// </summary>
        int MarginSegmentLength { get; }

        private string GetTimePoint(string? format)
        {
            string time = this.TimePoint.ToString(format);
            return $"{time}{new string(' ', this.TimePointSegmentLength - time.Length)}";
        }
        private string GetEventType()
        {
            var eventType = this.EventType;
            
            if (eventType.HasFlag(EventType.CRITICAL) && !eventType.Equals(EventType.CRITICAL))
                eventType &= ~EventType.CRITICAL; //< remove the critical flag if present

            string head = eventType switch
            {
                EventType.TRACE => "[TRACE]",
                EventType.DEBUG => "[DEBUG]",
                EventType.INFO => "[INFO]",
                EventType.WARN => "[WARN]",
                EventType.ERROR => "[ERROR]",
                EventType.FATAL => "[FATAL]",
                EventType.CRITICAL => "[CRITICAL]",
                _ => "[????]",
            };
            return $"{head}{new string(' ', this.EventTypeSegmentLength - head.Length)}";
        }
        private string GetMargin() => $"{(this.MarginSegmentLength > 0 ? new string(' ', this.MarginSegmentLength) : "")}";

        /// <summary>
        /// Creates a log message header from a <see cref="ITimestamp"/> source interface.
        /// </summary>
        /// <inheritdoc cref="DateTime.ToString(string?)"/>
        /// <returns>A <see cref="string"/> with the time-point and event type specified by the <see cref="ITimestamp"/>.</returns>
        static string MakeHeader(ITimestamp timestamp, string? format) => $"{timestamp.GetTimePoint(format)}{timestamp.GetEventType()}{timestamp.GetMargin()}";
        /// <inheritdoc cref="MakeHeader(ITimestamp, string?)"/>
        string ToString();
    }
}
