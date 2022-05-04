using VolumeControl.Log.Enum;

namespace VolumeControl.Log.Extensions
{
    /// <summary>
    /// Extension methods for the EventType enumerator.
    /// </summary>
    public static class EventTypeExtensions
    {
        public static string GetHeader(this EventType eventType)
            => eventType switch
            {
                EventType.DEBUG => "[DEBUG]",
                EventType.INFO => "[INFO]",
                EventType.WARN => "[WARN]",
                EventType.ERROR => "[ERROR]",
                EventType.FATAL => "[FATAL]",
                _ => "[????]",
            };
        public static byte ID(this EventType eventType)
            => (byte)eventType;
        public static bool Contains(this EventType eventType, EventType compare)
            => (eventType & compare) != 0;
        public static EventType Reset(this ref EventType eventType)
            => eventType = EventType.NONE;
        public static EventType Set(this ref EventType eventType, EventType add)
            => eventType |= add;
        public static EventType Unset(this ref EventType eventType, EventType add)
            => eventType &= ~add;
    }
}
