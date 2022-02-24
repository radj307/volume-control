using ProcessEventLog.Internal;

namespace ProcessEventLog
{
    public class DebugMessage : MessageBase { public DebugMessage(string msg) : base(Filter.MessageType.DEBUG, msg) { } }
    public class InfoMessage : MessageBase { public InfoMessage(string msg) : base(Filter.MessageType.INFO, msg) { } }
    public class LogMessage : MessageBase { public LogMessage(string msg) : base(Filter.MessageType.LOG, msg) { } }
    public class Message : MessageBase { public Message(string msg) : base(Filter.MessageType.MSG, msg) { } }
    public class WarningMessage : MessageBase { public WarningMessage(string msg) : base(Filter.MessageType.WARN, msg) { } }
    public class ErrorMessage : MessageBase { public ErrorMessage(string msg) : base(Filter.MessageType.ERROR, msg) { } }
    public class FatalErrorMessage : MessageBase { public FatalErrorMessage(string msg) : base(Filter.MessageType.FATAL, msg) { } }
    public class CustomMessage : MessageBase { public CustomMessage(Filter.MessageType type, string msg) : base(type, msg) { } }
}
