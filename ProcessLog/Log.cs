using ProcessLog.Internal;

namespace ProcessLog
{
    public static class Log
    {
        public static readonly Logger Writer = new(Path.GetTempFileName().ToString());

        public static void Write(string msg) => Writer.Write(msg);
        public static void WriteLine(string msg) => Writer.WriteLine(msg);
        public static void WriteMessage(ILogMessage msg) => Writer.WriteMessage(msg);
    }
}