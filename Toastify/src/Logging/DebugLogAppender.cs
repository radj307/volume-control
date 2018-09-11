using System.Diagnostics;
using log4net.Appender;
using log4net.Core;

namespace Toastify.Logging
{
    // ReSharper disable once UnusedMember.Global
    public class DebugLogAppender : ConsoleAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            Debug.Write(this.RenderLoggingEvent(loggingEvent));
        }
    }
}