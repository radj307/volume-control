using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using VolumeControl.Log.Helpers;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{
    /// <summary>
    /// Asynchonously writes messages to the log endpoint.
    /// </summary>
    public sealed class AsyncLogWriter : ThreadedActionQueue, ILogWriter, INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="AsyncLogWriter"/> instance with the specified <paramref name="endpoint"/> and <paramref name="eventTypeFilter"/>.
        /// </summary>
        /// <param name="endpoint">A log endpoint instance.</param>
        /// <param name="eventTypeFilter">The default event type filter.</param>
        public AsyncLogWriter(IEndpointWriter endpoint, EventType eventTypeFilter) : base()
        {
            Endpoint = endpoint;
            _eventTypeFilter = eventTypeFilter;
        }
        static AsyncLogWriter()
        {
            TimestampLength = DateTimeFormatString.Length + 2;
            EventTypeLength = System.Enum.GetNames<EventType>().Select(s => s.Length).Max() + 4;
            BlankLineHeader = new(' ', TimestampLength + EventTypeLength);
        }
        #endregion Constructor

        #region Fields
        internal readonly IEndpointWriter Endpoint;
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
                    return Endpoint.IsWritingEnabled;
                }
            }
            set
            {
                lock (Endpoint)
                {
                    Endpoint.IsWritingEnabled = value;
                }
                NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Gets or sets the event type filter that determines which message types are visible for this log writer instance.
        /// </summary>
        public EventType EventTypeFilter
        {
            get => _eventTypeFilter;
            set
            {
                _eventTypeFilter = value;
                NotifyPropertyChanged();
            }
        }
        private EventType _eventTypeFilter;
        /// <summary>
        /// Gets or sets whether log messages are added to the queue to be written asynchronously, or written synchronously.
        /// </summary>
        /// <returns><see langword="true"/> when log messages are written asynchronously; otherwise <see langword="false"/>.</returns>
        public bool IsAsyncEnabled
        {
            get => _isAsyncEnabled;
            set
            {
                if (value == _isAsyncEnabled) return;

                if (value)
                { // enable async
                    _isAsyncEnabled = true;
                }
                else
                { // disable async
                    _isAsyncEnabled = false;
                    Flush();
                }
                NotifyPropertyChanged();
            }
        }
        private bool _isAsyncEnabled = true;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region (Private) WriteLogMessage
        private void WriteLogMessage(LogMessage message)
        {
            lock (Endpoint)
            {
                if (!Endpoint.IsWritingEnabled) return;

                using var writer = Endpoint.GetTextWriter();

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
        #endregion (Private) WriteLogMessage

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
            lineObject.GetType();

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
        /// <inheritdoc/>
        public bool FilterEventType(EventType eventType)
        {
            return eventType == EventType.NONE
                || AlwaysVisibleEventTypes.HasFlag(eventType)
                || EventTypeFilter.HasFlag(eventType);
        }
        #endregion FilterEventType

        #region ResetEndpoint
        /// <summary>
        /// Resets the endpoint to its default state by calling <see cref="IEndpointWriter.Reset"/>.
        /// </summary>
        public void ResetEndpoint()
        {
            lock (Endpoint)
            {
                Endpoint.Reset();
            }
        }
        /// <summary>
        /// Resets the endpoint to its default state by calling <see cref="IEndpointWriter.Reset"/>, and writes the specified <paramref name="firstLine"/>.
        /// </summary>
        /// <param name="firstLine">The first line to (synchronously) write to the log.</param>
        public void ResetEndpoint(string firstLine)
        {
            lock (Endpoint)
            {
                Endpoint.Reset();
                Endpoint.WriteLine(firstLine);
            }
        }
        #endregion ResetEndpoint

        #region LogMessage
        /// <inheritdoc/>
        public bool LogMessage(LogMessage logMessage)
        {
            if (!FilterEventType(logMessage.EventType))
                return false;

            if (IsAsyncEnabled)
            {
                Enqueue(() => WriteLogMessage(logMessage));
            }
            else // async is disabled
            {
                WriteLogMessage(logMessage);
            }
            return true;
        }
        #endregion LogMessage

        #region EventType Writer Methods
        /// <inheritdoc/>
        public bool Trace(params object?[] lines) => LogMessage(new(EventType.TRACE, lines));
        /// <inheritdoc/>
        public bool Trace(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.TRACE));
        /// <inheritdoc/>
        public bool Debug(params object?[] lines) => LogMessage(new(EventType.DEBUG, lines));
        /// <inheritdoc/>
        public bool Debug(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.DEBUG));
        /// <inheritdoc/>
        public bool Info(params object?[] lines) => LogMessage(new(EventType.INFO, lines));
        /// <inheritdoc/>
        public bool Info(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.INFO));
        /// <inheritdoc/>
        public bool Warning(params object?[] lines) => LogMessage(new(EventType.WARN, lines));
        /// <inheritdoc/>
        public bool Warning(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.WARN));
        /// <inheritdoc/>
        public bool Error(params object?[] lines) => LogMessage(new(EventType.ERROR, lines));
        /// <inheritdoc/>
        public bool Error(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.ERROR));
        /// <inheritdoc/>
        public bool Fatal(params object?[] lines) => LogMessage(new(EventType.FATAL, lines));
        /// <inheritdoc/>
        public bool Fatal(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.FATAL));
        /// <inheritdoc/>
        public bool Critical(params object?[] lines) => LogMessage(new(EventType.CRITICAL, lines));
        /// <inheritdoc/>
        public bool Critical(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.CRITICAL));
        /// <inheritdoc/>
        public bool Blank(params object?[] lines) => LogMessage(new(EventType.NONE, lines));
        /// <inheritdoc/>
        public bool Blank(LogMessage logMessage) => LogMessage(logMessage.SetEventType(EventType.NONE));
        #endregion EventType Writer Methods

        #region DisableAsyncNoFlush
        /// <summary>
        /// Sets the IsAsyncEnabled property to <see langword="false"/> without flushing the queue.
        /// </summary>
        /// <remarks>
        /// Queued messages will still be written, but any further messages will be written synchronously.
        /// </remarks>
        public void DisableAsyncNoFlush()
        {
            _isAsyncEnabled = false;
            NotifyPropertyChanged(nameof(IsAsyncEnabled));
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
