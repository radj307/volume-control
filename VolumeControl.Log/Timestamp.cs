using VolumeControl.Log.Enum;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{
    /// <summary>
    /// Implements <see cref="ITimestamp"/> using UTC format with (<see cref="MarginTimePoint"/> = 29, <see cref="MarginEventType"/> = 8)
    /// </summary>
    public class Timestamp : ITimestamp
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="dateTime">The time-point that this timestamp represents.</param>
        /// <param name="eventType">The event type that this timestamp represents.</param>
        public Timestamp(DateTime dateTime, EventType eventType)
        {
            TimePoint = dateTime;
            EventType = eventType;
        }

        #region Properties
        /// <inheritdoc/>
        public DateTime TimePoint { get; set; }
        /// <inheritdoc/>
        public EventType EventType { get; set; }
        /// <inheritdoc/>
        public int TimePointSegmentLength => LineSegmentLengthDateTime;
        /// <inheritdoc/>
        public int EventTypeSegmentLength => LineSegmentLengthEventType;
        /// <inheritdoc/>
        public int MarginSegmentLength => LineSegmentLengthMargin;
        #endregion

        #region Methods
        /// <inheritdoc cref="ITimestamp.MakeHeader(ITimestamp, string?)"/>
        public override string ToString() => ITimestamp.MakeHeader(this, FormatString);
        #endregion Methods

        #region Statics
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static string FormatString => Settings.TimestampFormat;
        public static int LineSegmentLengthDateTime => Settings.LineSegmentLengthDateTime;
        public static int LineSegmentLengthEventType => Settings.LineSegmentLengthEventType;
        public static int LineSegmentLengthMargin => Settings.LineSegmentLengthMargin;
        public static int LineHeaderLength => LineSegmentLengthDateTime + LineSegmentLengthEventType;
        public static int LineHeaderTotalLength => LineHeaderLength + LineSegmentLengthMargin;

        /// <summary>
        /// Gets a timestamp with the current time and a given <see cref="EventType"/>.
        /// </summary>
        /// <param name="eventType">The event type header to show at the end of the timestamp.</param>
        /// <returns><see cref="Timestamp"/> object with the current time and given type.</returns>
        public static Timestamp Now(EventType eventType) => new(DateTime.Now, eventType);
        /// <summary>
        /// Gets a blank string with the same length as a <see cref="Timestamp"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> entirely composed of space (' ') chars with the same length as a timestamp string.</returns>
        public static string Blank() => new(' ', LineHeaderTotalLength);
        public static string Definition() => $"";
        #endregion Statics
    }
}
