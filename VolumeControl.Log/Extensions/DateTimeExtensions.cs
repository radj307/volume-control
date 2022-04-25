using VolumeControl.Log.Enum;

namespace VolumeControl.Log.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Create a <see cref="Timestamp"/> from a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="dateTime">The timepoint to use for the timestamp.</param>
        /// <returns><see cref="Timestamp"/> object that acts as a convenience wrapper when creating log messages.</returns>
        public static Timestamp ToTimestamp(this DateTime dateTime, EventType eventType) => new(dateTime, eventType);
    }
}
