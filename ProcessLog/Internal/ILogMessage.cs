namespace ProcessLog.Internal
{
    public interface ILogMessage
    {
        public Filter.MessageType Type { get; }

        public string Message { get; }

        public string Serialize() => Type.GetHeader() + Message + '\n';
    }
}