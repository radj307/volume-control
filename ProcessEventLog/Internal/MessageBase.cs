namespace ProcessEventLog.Internal
{
    public class MessageBase : ILogMessage
    {
        protected MessageBase(Filter.MessageType type, string msg)
        {
            _type = type;
            _msg = msg;
        }

        private readonly Filter.MessageType _type;
        private readonly string _msg;

        public Filter.MessageType Type { get => _type; }
        public string Message { get => _msg; }
    }
}