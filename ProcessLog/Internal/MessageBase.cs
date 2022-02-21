namespace ProcessLog.Internal
{
    public class MessageBase : ILogMessage
    {
        protected MessageBase(Filter.MessageType type, string msg)
        {
            _type = type;
            _msg = msg;
        }

        private Filter.MessageType _type;
        private string _msg;

        public Filter.MessageType Type { get => _type; }
        public string Message { get => _msg; }
    }
}