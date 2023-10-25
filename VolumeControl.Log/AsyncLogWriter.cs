using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;

namespace VolumeControl.Log
{
    /// <summary>
    /// Asynchonously writes messages to the log endpoint.
    /// </summary>
    public sealed class AsyncLogWriter : ThreadedLogger, ILogWriter, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AsyncLogWriter"/> instance with the specified <paramref name="endpoint"/> and <paramref name="eventTypeFilter"/>.
        /// </summary>
        /// <param name="endpoint">A log endpoint instance.</param>
        /// <param name="eventTypeFilter">The default event type filter.</param>
        public AsyncLogWriter(IEndpoint endpoint, EventType eventTypeFilter) : base()
        {
            Endpoint = endpoint;
            EventTypeFilter = eventTypeFilter;
        }
        static AsyncLogWriter()
        {
            TimestampLength = DateTimeFormatString.Length + 2;
            EventTypeLength = System.Enum.GetNames<EventType>().Select(s => s.Length).Max() + 4;
            BlankLineHeader = new(' ', TimestampLength + EventTypeLength);
        }
        #endregion Constructor

        #region Fields
        internal readonly IEndpoint Endpoint;
        /// <summary>
        /// The string that defines the format of timestamps.
        /// </summary>
        public const string DateTimeFormatString = "HH:mm:ss:fff";
        /// <summary>
        /// Length of the timestamp segment of the line header.
        /// </summary>
        /// <remarks>
        /// The timestamp segment comes before the event type.
        /// </remarks>
        public static readonly int TimestampLength;
        /// <summary>
        /// Length of the event type segment of the line header.
        /// </summary>
        /// <remarks>
        /// The event type segment comes after the timestamp.
        /// </remarks>
        public static readonly int EventTypeLength;
        /// <summary>
        /// A blank string with the same length as a line header.
        /// </summary>
        public static readonly string BlankLineHeader;
        /// <summary>
        /// Event types that are always visible. <see cref="EventType.NONE"/> is implicitly included.
        /// </summary>
        public const EventType AlwaysVisibleEventTypes = EventType.FATAL | EventType.CRITICAL;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets whether the endpoint is enabled or not.
        /// </summary>
        public bool EndpointEnabled
        {
            get
            {
                lock (Endpoint)
                {
                    return Endpoint.Enabled;
                }
            }
            set
            {
                lock (Endpoint)
                {
                    Endpoint.Enabled = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the event type filter that determines which message types are visible for this log writer instance.
        /// </summary>
        public EventType EventTypeFilter { get; set; }
        /// <summary>
        /// Gets or sets whether log messages are added to the queue to be written asynchronously, or written synchronously.
        /// </summary>
        /// <returns><see langword="true"/> when log messages are written asynchronously; otherwise <see langword="false"/>.</returns>
        public bool IsAsyncEnabled
        {
            get => _isAsyncEnabled;
            set
            {
                if (_isAsyncEnabled == value) return;

                if (value)
                { // enable async
                    _isAsyncEnabled = true;
                }
                else
                { // disable async
                    _isAsyncEnabled = false;
                    Flush();
                }
            }
        }
        private bool _isAsyncEnabled = true;
        #endregion Properties

        #region WriteLogMessage Override
        /// <inheritdoc/>
        protected override void WriteLogMessage(LogMessage message)
        {
            if (!FilterEventType(message.EventType))
            { // event type is not enabled; don't show it
                return;
            }

            lock (Endpoint)
            {
                if (!Endpoint.Enabled) return;

                using var writer = Endpoint.GetWriter();

                if (writer == null) return;

                // write the line header
                writer.Write(MakeLineHeader(DateTime.Now, message.EventType));

                bool skipIndentOnNextLine = false;

                // enumerate the lines in the message
                for (int i = 0, max = message.Lines.Count; i < max; ++i)
                {
                    object lineObject = message.Lines[i]!;

                    if (i >= 1) // indent subsequent lines
                    {
                        if (skipIndentOnNextLine) //< don't indent this line
                            skipIndentOnNextLine = false;
                        else
                            writer.Write(BlankLineHeader);
                    }

                    // convert the object to a string & trim whitespace
                    var line = ObjectToString(lineObject)?.Trim();

                    if (line == null) continue;

                    // if the next line is an exception & this line is a single-line string that
                    //  ends with a colon, use this line as a prefix for the formatted exception
                    if (i + 1 < max && lineObject is string && message.Lines[i + 1] is Exception && line.EndsWith(':') && !line.Contains('\n', StringComparison.Ordinal))
                    {
                        writer.Write(line + ' ');
                        skipIndentOnNextLine = true;
                    }
                    else if (line.EndsWith('\n'))
                        writer.Write(line);
                    else
                        writer.WriteLine(line);
                }

                writer.Flush();
            }
        }
        #endregion WriteLogMessage Override

        #region Methods

        #region (Private/Internal)
        private static string MakeLineHeader(DateTime dateTime, EventType eventType)
        {
            string timeStampString = dateTime.ToString(DateTimeFormatString);
            string eventTypeString = eventType.Equals(EventType.NONE) ? "" : $"[{System.Enum.GetName(eventType)}]";

            return $"{timeStampString}{Indent(TimestampLength, timeStampString.Length)}{eventTypeString}{Indent(EventTypeLength, eventTypeString.Length)}";
        }
        private static string InsertLinePrefixes(string s)
        { // insert blank line headers after each newline to preserve formatting
            return Regex.Replace(s, @"(?:(\r{0,1})\n)+(?!$)", "$1\n" + BlankLineHeader, RegexOptions.Compiled);
        }
        private static string? ObjectToString(object lineObject, bool isRecursed = false)
        {
            if (lineObject is Exception ex)
            { // expand exception
                return ExceptionMessageHelper.MakeExceptionMessage(ex, BlankLineHeader, Environment.NewLine, 2, ExceptionMessageHelper.MessageParts.All);
            }
            else if (lineObject is string s)
            { // object is a string
                if (s.Length > 0)
                {
                    return isRecursed ? s : InsertLinePrefixes(s);
                }
            }
            else if (lineObject is IEnumerable enumerable)
            { // expand enumerable types
                StringBuilder sb = new();

                foreach (var item in enumerable)
                {
                    var objectString = ObjectToString(item, true)?.TrimEnd('\n', '\r'); //< RECURSE

                    if (objectString == null || objectString.Length == 0) continue;

                    sb.AppendLine(objectString);
                }

                return isRecursed ? sb.ToString() : InsertLinePrefixes(sb.ToString());
            }
            else return lineObject.ToString();
            return null;
        }
        internal static string Indent(int maxLength, int usedLength)
        {
            int length = maxLength - usedLength;

            return length <= 0
                ? ""
                : new(' ', length);
        }
        #endregion (Private/Internal)

        #region FilterEventType
        /// <summary>
        /// Checks if messages with the specified <paramref name="eventType"/> are shown in the log.
        /// </summary>
        /// <param name="eventType">An <see cref="EventType"/> to check.</param>
        /// <returns><see langword="true"/> when the <paramref name="eventType"/> is shown; otherwise <see langword="false"/>.</returns>
        public bool FilterEventType(EventType eventType)
        {
            return eventType == EventType.NONE || AlwaysVisibleEventTypes.HasFlag(eventType) || EventTypeFilter.HasFlag(eventType);
        }
        #endregion FilterEventType

        #region ResetEndpoint
        /// <summary>
        /// Resets the endpoint to its default state by calling <see cref="IEndpoint.Reset"/>.
        /// </summary>
        public void ResetEndpoint()
        {
            lock (Endpoint)
            {
                Endpoint.Reset();
            }
        }
        /// <summary>
        /// Resets the endpoint to its default state by calling <see cref="IEndpoint.Reset"/>, and writes the specified <paramref name="firstLine"/>.
        /// </summary>
        /// <param name="firstLine">The first line to (synchronously) write to the log.</param>
        public void ResetEndpoint(string firstLine)
        {
            lock (Endpoint)
            {
                Endpoint.Reset();
                Endpoint.WriteRawLine(firstLine);
            }
        }
        #endregion ResetEndpoint

        #region LogMessage
        /// <summary>
        /// Writes the specified <paramref name="logMessage"/> to the log.
        /// </summary>
        /// <remarks>
        /// When IsAsyncEnabled is <see langword="true"/>, the message is added to the queue and written asynchronously; otherwise, the message is blocking and the message is written synchronously.
        /// </remarks>
        /// <param name="logMessage"></param>
        public new void LogMessage(LogMessage logMessage)
        {
            if (IsAsyncEnabled)
            {
                base.LogMessage(logMessage);
            }
            else // async is disabled
            {
                WriteLogMessage(logMessage);
            }
        }
        #endregion LogMessage

        #region EventType Writer Methods
        /// <summary>
        /// Queues writing a <see cref="EventType.TRACE"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Trace(params object?[] lines) => LogMessage(new(EventType.TRACE, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.DEBUG"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Debug(params object?[] lines) => LogMessage(new(EventType.DEBUG, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.INFO"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Info(params object?[] lines) => LogMessage(new(EventType.INFO, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.WARN"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Warning(params object?[] lines) => LogMessage(new(EventType.WARN, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.ERROR"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Error(params object?[] lines) => LogMessage(new(EventType.ERROR, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.FATAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Fatal(params object?[] lines) => LogMessage(new(EventType.FATAL, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.CRITICAL"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Critical(params object?[] lines) => LogMessage(new(EventType.CRITICAL, lines));
        /// <summary>
        /// Queues writing a <see cref="EventType.NONE"/> message with the specified <paramref name="lines"/>.
        /// </summary>
        /// <remarks>
        /// This produces a message with no event type header, but a timestamp is still shown.
        /// </remarks>
        /// <param name="lines">Any number of objects. Each object will be written on a new line. <see langword="null"/> objects and blank strings are skipped.</param>
        public void Blank(params object?[] lines) => LogMessage(new(EventType.NONE, lines));
        #endregion EventType Writer Methods

        #region DisableAsyncNoFlush
        /// <summary>
        /// Sets the IsAsyncEnabled property to <see langword="false"/> without flushing the queue.
        /// </summary>
        public void DisableAsyncNoFlush()
        {
            _isAsyncEnabled = false;
        }
        #endregion DisableAsyncNoFlush

        #endregion Methods

        #region IDisposable Implementation
        /// <summary>
        /// Disposes of the endpoint &amp; thread.
        /// </summary>
        ~AsyncLogWriter() => Dispose();
        /// <inheritdoc/>
        public new void Dispose()
        {
            if (Endpoint is IDisposable d)
                d.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
