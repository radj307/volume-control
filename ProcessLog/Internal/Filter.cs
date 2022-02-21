namespace ProcessLog.Internal
{
    public static class Filter
    {
        [Flags]
        public enum MessageType : byte
        {
            /// <summary>
            /// Hides all message types.
            /// </summary>
            NONE = 0,
            /// <summary>
            /// Shows debug messages.
            /// </summary>
            DEBUG = 1,
            /// <summary>
            /// Shows unimportant informational messages.
            /// </summary>
            INFO = 2,
            /// <summary>
            /// Shows important informational messages.
            /// </summary>
            LOG = 4,
            /// <summary>
            /// Shows important messages for the user
            /// </summary>
            MSG = 8,
            /// <summary>
            /// Shows non-critical errors and warnings that the user should take note of.
            /// </summary>
            WARN = 16,
            /// <summary>
            /// Shows critical errors that were handled successfully.
            /// </summary>
            ERROR = 32,
            /// <summary>
            /// Shows fatal errors that cannot be recovered from.
            /// This includes ALL crashes.
            /// </summary>
            FATAL = 64,
            /// <summary>
            /// Shows all message types.
            /// </summary>
            ALL = DEBUG | INFO | LOG | MSG | WARN | ERROR | FATAL,
        }

        public static byte ToByte(this MessageType mt) => (byte)mt;

        public static bool AllowsMessageType(MessageType filterLevel, MessageType msgType) => (filterLevel.ToByte() & msgType.ToByte()) != 0;

        public static bool Allows(this MessageType filter, MessageType msgType) => AllowsMessageType(filter, msgType);
        public static bool Denies(this MessageType filter, MessageType msgType) => !AllowsMessageType(filter, msgType);

        public static string GetHeader(this MessageType mt)
        {
            return (Enum.GetName(typeof(MessageType), mt) ?? "[ ??? ]") + '\t';
        }
    }
}