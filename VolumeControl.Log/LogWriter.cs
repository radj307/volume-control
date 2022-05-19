using System.Runtime.CompilerServices;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;
using VolumeControl.Log.Interfaces;
using VolumeControl.Log.Properties;

namespace VolumeControl.Log
{
    /// <summary>
    /// Basic log writer object.
    /// </summary>
    /// <remarks>This does all of the heavy-lifting (string manipulation) for the <see cref="Log"/> namespace.</remarks>
    public class LogWriter : ILogWriter
    {
        #region Constructors
        public LogWriter(IEndpoint endpoint, EventType eventTypeFilter)
        {
            Endpoint = endpoint;
            EventTypeFilter = eventTypeFilter;
            LastEventType = EventType.NONE;
        }
        #endregion Constructors

        #region InterfaceImplementation
        /// <summary>
        /// Gets or sets the log endpoint object used for output.
        /// </summary>
        public IEndpoint Endpoint { get; set; }
        /// <summary>
        /// Gets or sets the bitfield filter for event types.
        /// </summary>
        public EventType EventTypeFilter { get; set; }
        /// <summary>
        /// Gets the last type of event to be printed to the log.
        /// </summary>
        public EventType LastEventType { get; private set; }
        /// <inheritdoc/>
        public bool FilterEventType(EventType eventType) => EventTypeFilter.HasFlag(eventType);
        /// <inheritdoc/>
        public ITimestamp MakeTimestamp(EventType eventType) => Timestamp.Now(eventType);
        public static string MakeBlankTimestamp() => Timestamp.Blank();
        #endregion InterfaceImplementation

        #region Methods
        #region WriteRaw
        public void Write(object text) => Endpoint.WriteRaw(text.ToString());
        public void WriteLine(object? line = null) => Endpoint.WriteRawLine(line?.ToString());
        #endregion WriteRaw

        public void WriteWithTimestamp(string ts, params object?[] lines)
        {
            if (!Endpoint.Enabled || lines.Length == 0)
                return;
            using StreamWriter w = Endpoint.GetWriter()!;
            w.AutoFlush = false;
            w.Write(ts);
            string tsBlank = MakeBlankTimestamp();
            for (int i = 0, end = lines.Length; i < end; ++i)
            {
                var line = lines[i];

                if (line is null)
                    continue;
                else if (line is Exception ex)
                    w.WriteLine($"{(i == 0 ? "" : tsBlank)}{ex.ToString(tsBlank.Length)}");
                else if (line is string s)
                {
                    if (s.Length == 0)
                        continue;
                    else w.WriteLine($"{(i == 0 ? "" : tsBlank)}{s}");
                }
                else
                    w.WriteLine($"{(i == 0 ? "" : tsBlank)}{line}");
            }
            w.Flush();
            w.Close();
        }
        public void WriteWithTimestamp(ITimestamp ts, params object?[] lines) => WriteWithTimestamp(ts.ToString(), lines);

        public static string GetTrace([CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1) => $"{{ {callerMemberName}@'{callerFilePath}':{callerLineNumber} }}";

        #region WriteEvent
        public void WriteEvent(EventType eventType, object?[] lines)
        {
            if (!Endpoint.Enabled || lines.Length == 0 || !FilterEventType(eventType))
                return;
            WriteWithTimestamp(MakeTimestamp(LastEventType = eventType), lines);
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
        #endregion WriteEvent

        #region WriteException
        public void WriteException(EventType ev, Exception exception, object? message = null)
        {
            if (!Endpoint.Enabled)
                return;
            if (message == null)
                WriteEvent(ev, new[] { exception });
            else
                WriteEvent(ev, new[] { message, exception });
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
        #endregion WriteException

        /// <summary>
        /// Appends the given lines to the log with a blank timestamp.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void Append(params object?[] lines)
        {
            if (!Endpoint.Enabled)
                return;
            WriteWithTimestamp(MakeBlankTimestamp(), lines);
        }
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void Followup(params object?[] lines) => Append(lines);
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="eventType">Message is only printed if this was the last event type to be printed.</param>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void FollowupIf(EventType eventType, params object?[] lines)
        {
            if (eventType.Equals(LastEventType))
                Append(lines);
        }
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="predicate">A predicate that accepts the <see cref="LastEventType"/> to use as the condition.</param>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void FollowupIf(Predicate<EventType> predicate, params object?[] lines)
        {
            if (predicate(LastEventType))
                Append(lines);
        }
        #endregion Methods
    }
}
