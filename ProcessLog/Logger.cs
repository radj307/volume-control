using ProcessLog.Internal;

namespace ProcessLog
{
    public class Logger : StreamWriter, IDisposable
    {
        public Logger(string path) : base(path, false, System.Text.Encoding.UTF8) { }

        public void WriteMessage(ILogMessage msg)
        {
            WriteLine(msg.Serialize());
        }
    }
}