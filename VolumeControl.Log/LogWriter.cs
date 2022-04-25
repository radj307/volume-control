using System.Diagnostics;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{
    public class LogWriter : ILogWriter
    {
        public LogWriter(IEndpoint endpoint, EventType eventTypeFilter)
        {
            Endpoint = endpoint;
            EventTypeFilter = eventTypeFilter;
            Endpoint.Reset();
        }

        public IEndpoint Endpoint { get; set; }
        public EventType EventTypeFilter { get; set; }

        public bool FilterEventType(EventType eventType) => EventTypeFilter.HasFlag(eventType);
        public ITimestamp MakeTimestamp(EventType eventType) => Timestamp.Now(eventType);

        public void Write(object text) => Endpoint.WriteRaw(text.ToString());
        public void WriteLine(object? line = null) => Endpoint.WriteRawLine(line?.ToString());

        public void WriteEvent(EventType eventType, params object?[] lines)
        {
            if (lines.Length == 0 || !FilterEventType(eventType))
                return;
            string ts = MakeTimestamp(eventType).ToString();
            WriteLine($"{ts}{lines[0]}");
            if (lines.Length > 1)
            {
                // replace the timestamp with an empty one of the same length
                ts = new string(' ', ts.Length);
                for (int i = 1; i < lines.Length; ++i)
                {
                    var line = lines[i];
                    if (line == null)
                        continue;
                    else if (line is Exception ex)
                    {
                        WriteLine($"{ts}{ex.ToString(ts)}");
                    }
                    else
                    {
                        WriteLine($"{ts}{line}");
                    }
                }
            }
        }
        /// <summary>
        /// Write a formatted <see cref="EventType.DEBUG"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects, each written on a new line.</param>
        public void Debug(params object?[] lines) => WriteEvent(EventType.DEBUG, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.INFO"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects, each written on a new line.</param>
        public void Info(params object?[] lines) => WriteEvent(EventType.INFO, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.WARN"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects, each written on a new line.</param>
        public void Warning(params object?[] lines) => WriteEvent(EventType.WARN, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.ERROR"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects, each written on a new line.</param>
        public void Error(params object?[] lines) => WriteEvent(EventType.ERROR, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.FATAL"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects, each written on a new line.</param>
        public void Fatal(params object?[] lines) => WriteEvent(EventType.FATAL, lines);
        public void WriteException(EventType ev, Exception exception, object? message = null)
        {
            if (message == null)
                WriteEvent(ev, exception);
            WriteEvent(ev, message, exception);
        }
        /// <summary>
        /// Write a formatted <see cref="EventType.DEBUG"/> exception message to the log endpoint.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="message">An optional header message to use instead of the exception's message. <i>(The exception message is still shown.)</i></param>
        public void DebugException(Exception exception, object? message = null) => WriteException(EventType.DEBUG, exception, message);
        /// <summary>
        /// Write a formatted <see cref="EventType.INFO"/> exception message to the log endpoint.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="message">An optional header message to use instead of the exception's message. <i>(The exception message is still shown.)</i></param>
        public void InfoException(Exception exception, object? message = null) => WriteException(EventType.INFO, exception, message);
        /// <summary>
        /// Write a formatted <see cref="EventType.WARN"/> exception message to the log endpoint.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="message">An optional header message to use instead of the exception's message. <i>(The exception message is still shown.)</i></param>
        public void WarningException(Exception exception, object? message = null) => WriteException(EventType.WARN, exception, message);
        /// <summary>
        /// Write a formatted <see cref="EventType.ERROR"/> exception message to the log endpoint.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="message">An optional header message to use instead of the exception's message. <i>(The exception message is still shown.)</i></param>
        public void ErrorException(Exception exception, object? message = null) => WriteException(EventType.ERROR, exception, message);
        /// <summary>
        /// Write a formatted <see cref="EventType.FATAL"/> exception message to the log endpoint.
        /// </summary>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="message">An optional header message to use instead of the exception's message. <i>(The exception message is still shown.)</i></param>
        public void FatalException(Exception exception, object? message = null) => WriteException(EventType.FATAL, exception, message);
    }
}
