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
        public DateTime TimePoint { get; set; }
        public EventType EventType { get; set; }
        public int MarginTimePoint { get => 29; }
        public int MarginEventType { get => 8; }
        #endregion

        #region Methods
        public override string ToString() => ITimestamp.MakeHeader(this, "U");
        #endregion Methods

        #region Statics
        /// <summary>
        /// Gets a timestamp with the current time and a given <see cref="EventType"/>.
        /// </summary>
        /// <param name="eventType">The event type header to show at the end of the timestamp.</param>
        /// <returns><see cref="Timestamp"/> object with the current time and given type.</returns>
        public static Timestamp Now(EventType eventType) => new(DateTime.Now, eventType);
        #endregion Statics
    }
}
