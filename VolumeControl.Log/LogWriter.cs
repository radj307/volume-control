using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Extensions;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{
    /// <summary>
    /// Basic log writer object.
    /// </summary>
    /// <remarks>This does all of the heavy-lifting (string manipulation) for the <see cref="Log"/> namespace.</remarks>
    public class LogWriter : ILogWriter, IDisposable
    {
        private bool disposedValue;
        #region Constructors
        /// <inheritdoc cref="LogWriter"/>
        /// <param name="endpoint">An endpoint to use.</param>
        /// <param name="eventTypeFilter">An event type filter to use. This may be a combination of bitfield flags.</param>
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
        /// <summary>Gets a blank timestamp to use as indentation.</summary>
        /// <returns>A string of the same length as a timestamp, filled entirely with space characters.</returns>
        public static string MakeBlankTimestamp() => Timestamp.Blank();
        #endregion InterfaceImplementation

        #region Methods
        #region WriteRaw
        /// <summary>Writes some text to the log file.</summary>
        /// <remarks>Using this method is very inefficient as it re-creates a <see cref="StreamWriter"/> each time; see the obsoletion warning.</remarks>
        /// <param name="text">Object to write.</param>
        [Obsolete($"Use the {nameof(Debug)}, {nameof(Info)}, {nameof(Warning)}, {nameof(Error)}, and {nameof(Fatal)} methods instead.")]
        public void Write(object text) => Endpoint.WriteRaw(text.ToString());
        /// <summary>Writes a line to the log file.</summary>
        /// <remarks>Using this method is very inefficient as it re-creates a <see cref="StreamWriter"/> each time; see the obsoletion warning.</remarks>
        /// <param name="line">Object to write.</param>
        [Obsolete($"Use the {nameof(Debug)}, {nameof(Info)}, {nameof(Warning)}, {nameof(Error)}, and {nameof(Fatal)} methods instead.")]
        public void WriteLine(object? line = null) => Endpoint.WriteRawLine(line?.ToString());
        #endregion WriteRaw

        /// <summary>
        /// Creates and returns a log message object using this log writer as a base.
        /// </summary>
        /// <returns><see cref="LogMessage"/> class.</returns>
        public LogMessage GetMessage() => new(EventTypeFilter) { LastEventType = LastEventType };

        /// <summary>
        /// Writes a log message with a given timestamp.
        /// </summary>
        /// <param name="ts">The full timestamp as a string.</param>
        /// <param name="lines">Any number of lines of any object type.</param>
        public void WriteWithTimestamp(string ts, params object?[] lines)
        {
            if (!Endpoint.Enabled || lines.Length == 0)
                return;
            using TextWriter w = Endpoint.GetWriter()!;
            w.Write(ts);
            string tsBlank = MakeBlankTimestamp();
            for (int i = 0, end = lines.Length; i < end; ++i)
            {
                object? line = lines[i];

                if (line is null)
                {
                    continue;
                }
                else if (line is Exception ex)
                {
                    w.WriteLine($"{(i == 0 ? "" : tsBlank)}{ex.ToString(tsBlank.Length)}");
                }
                else if (line is string s)
                {
                    if (s.Length > 0)
                        w.WriteLine($"{(i == 0 ? "" : tsBlank)}{s}");
                }
                else if (line is IEnumerable enumerable)
                {
                    foreach (object? item in enumerable)
                    {
                        if (item != null)
                            w.WriteLine($"{(i == 0 ? "" : tsBlank)}{item}");
                    }
                }
                else if (line is ILogWriter logWriter)
                {
                    using TextReader? r = logWriter.Endpoint.GetReader();
                    if (r == null)
                        break;
                    string logBuffer = Regex.Replace(r.ReadToEnd().Trim(), "\\r*\\n", $"\n{tsBlank}");
                    r.Dispose();
                    if (logBuffer.Length == 0)
                        break;
                    w.Write(logBuffer);
                }
                else
                {
                    w.WriteLine($"{(i == 0 ? "" : tsBlank)}{line}");
                }
            }
            w.Flush();
            w.Close();
        }
        /// <param name="ts">The timestamp object.</param>
        /// <param name="lines">Any number of lines of any object type.</param>
        /// <inheritdoc cref="WriteWithTimestamp(string, object?[])"/>
        public void WriteWithTimestamp(ITimestamp ts, params object?[] lines) => WriteWithTimestamp(ts.ToString(), lines);
        /// <summary>Gets a stack trace message using attributes.</summary>
        /// <remarks>Don't provide parameters.</remarks>
        public static string GetTrace([CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1) => $"{{ {callerMemberName}@'{callerFilePath}':{callerLineNumber} }}";

        /// <summary>Wrapper object for the <see cref="Conditional(ConditionalMessage[])"/> method.</summary>
        public struct ConditionalMessage
        {
            public ConditionalMessage(EventType type, params object?[] message)
            {
                EventType = type;
                Message = message;
            }
            public ConditionalMessage(EventType type, IEnumerable enumerable)
            {
                EventType = type;
                Message = enumerable;
            }
            public EventType EventType { get; set; }
            public IEnumerable Message { get; set; }

            public void Deconstruct(out EventType type, out IEnumerable enumerable)
            {
                type = EventType;
                enumerable = Message;
            }
        }
        /// <summary>Conditional log message, allows you to write different messages depending on the current log filter level.</summary>
        /// <param name="messages">EventType-IEnumerable pairs. Only the message assoicated with the first pair whose event type is considered valid by <see cref="FilterEventType(EventType)"/> is printed.<br/>If none of the given event types are allowed, nothing happens.</param>
        public void Conditional(params ConditionalMessage[] messages)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                (EventType type, IEnumerable message) = messages[i];
                if (FilterEventType(type))
                {
                    WriteEvent(type, message);
                    return;
                }
            }
        }

        #region WriteEvent
        /// <summary>Writes a log message with a timestamp and log level header.</summary>
        /// <param name="eventType">An event type. Do not provide combinations of event type flags!</param>
        /// <param name="lines">Any enumerable type.</param>
        public void WriteEvent(EventType eventType, IEnumerable lines)
        {
            if (!Endpoint.Enabled || !FilterEventType(eventType))
                return;
            WriteWithTimestamp(MakeTimestamp(LastEventType = eventType), lines);
        }
        /// <summary>Writes a log message with a timestamp and log level header.</summary>
        /// <param name="pair">An event type, and any enumerable type.</param>
        public void WriteEvent((EventType, IEnumerable) pair) => WriteEvent(pair.Item1, pair.Item2);
        /// <summary>Writes a log message with a timestamp and log level header.</summary>
        /// <param name="eventType">An event type. Do not provide combinations of event type flags!</param>
        /// <param name="lines">Any enumerable type.</param>
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
        ///
        [Obsolete($"Use {nameof(WriteEvent)} instead, it supports exceptions.")]
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

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Endpoint is IDisposable disp)
                        disp.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
