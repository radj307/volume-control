using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using VolumeControl.Log.Endpoints;
using VolumeControl.Log.Enum;
using VolumeControl.Log.Interfaces;

namespace VolumeControl.Log
{

    /// <summary>
    /// Basic log writer object.
    /// </summary>
    /// <remarks>This does all of the heavy-lifting string manipulation for the <see cref="Log"/> namespace.</remarks>
    public class LogWriter : INotifyPropertyChanged, ILogWriter, IDisposable
    {
        #region Constructors
        /// <inheritdoc cref="LogWriter"/>
        /// <param name="endpoint">An endpoint to use.</param>
        /// <param name="eventTypeFilter">An event type filter to use. This may be a combination of bitfield flags.</param>
        public LogWriter(IEndpoint endpoint, EventType eventTypeFilter)
        {
            this.Endpoint = endpoint;
            this.EventTypeFilter = eventTypeFilter;
            this.LastEventType = EventType.NONE;
        }
        #endregion Constructors

        #region InterfaceImplementation
        private bool disposedValue;
#       pragma warning disable CS0067
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067
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
        public bool FilterEventType(EventType eventType) => eventType.HasFlag(EventType.CRITICAL) || this.EventTypeFilter.HasFlag(eventType);
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
        public void Write(object text) => this.Endpoint.WriteRaw(text.ToString());
        /// <summary>Writes a line to the log file.</summary>
        /// <remarks>Using this method is very inefficient as it re-creates a <see cref="StreamWriter"/> each time; see the obsoletion warning.</remarks>
        /// <param name="line">Object to write.</param>
        [Obsolete($"Use the {nameof(Debug)}, {nameof(Info)}, {nameof(Warning)}, {nameof(Error)}, and {nameof(Fatal)} methods instead.")]
        public void WriteLine(object? line = null) => this.Endpoint.WriteRawLine(line?.ToString());
        #endregion WriteRaw

        private static List<DictionaryEntry>? UnpackExceptionData(IDictionary data)
        {
            if (data != null && data.Count > 0)
            {
                List<DictionaryEntry> l = new();
                foreach (DictionaryEntry kvp in data)
                    l.Add(kvp);
                return l;
            }
            return null;
        }

        /// <summary>Creates a formatted string representation of <paramref name="ex"/>.</summary>
        /// <param name="ex">An exception object.</param>
        /// <param name="linePrefix">An optional <see langword="string"/> to prepend to each line.</param>
        /// <param name="lineSuffix">An optional <see langword="string"/> to append to each line.</param>
        /// <param name="tabString">The string to use as a single tab. This is used when indenting subsequent lines.</param>
        /// <returns>A <see langword="string"/> representation of <paramref name="ex"/>.</returns>
        public static string FormatExceptionMessage(Exception ex, string? linePrefix = null, string lineSuffix = "\n", string tabString = "  ")
        {
            string m = string.Empty;
            string tabPrefix = linePrefix + tabString;

            m += $"{{{lineSuffix}";

            // Message
            m += $"{tabPrefix}'Message': '{ex.Message}'{lineSuffix}";
            m += $"{tabPrefix}'HResult': '{ex.HResult}'{lineSuffix}";

            // Source
            if (ex.Source != null)
                m += $"{tabPrefix}'Source': '{ex.Source}'{lineSuffix}";

            // TargetSite
            if (ex.TargetSite != null)
            {
                m += $"{tabPrefix}'TargetSite': {{{lineSuffix}";
                m += $"{tabPrefix}{tabString}'Name': '{ex.TargetSite.Name}'{lineSuffix}";
                if (ex.TargetSite.DeclaringType != null)
                    m += $"{tabPrefix}{tabString}'DeclaringType': '{ex.TargetSite.DeclaringType.FullName}'{lineSuffix}";
                m += $"{tabPrefix}{tabString}'Attributes': '{ex.TargetSite.Attributes:G}'{lineSuffix}";
                m += $"{tabPrefix}{tabString}'CallingConvention': '{ex.TargetSite.CallingConvention:G}'{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // Data
            if (UnpackExceptionData(ex.Data) is List<DictionaryEntry> data)
            { // exception has data entries, include them
                m += $"{tabPrefix}'Data':{{{lineSuffix}";
                foreach ((object key, object? val) in data)
                    m += $"{tabPrefix}{tabString}'{key}': '{val}'{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // Stack Trace
            if (ex.StackTrace != null)
            {
                m += $"{tabPrefix}'StackTrace': {{{lineSuffix}";
                int i = 0;
                foreach (string s in ex.StackTrace.Split('\n'))
                    m += $"{tabPrefix}{tabString}[{i++}] {s.Trim()}{lineSuffix}";
                m += $"{tabPrefix}}}{lineSuffix}";
            }

            // InnerException
            if (ex.InnerException != null)
                m += $"{tabPrefix}'InnerException': {FormatExceptionMessage(ex.InnerException, tabPrefix, lineSuffix, tabString)}{lineSuffix}";

            m += $"{linePrefix}}}";
            return m;
        }

        /// <summary>
        /// Writes a log message with a given timestamp.
        /// </summary>
        /// <param name="ts">The full timestamp as a string.</param>
        /// <param name="lines">Any number of lines of any object type.</param>
        public void WriteWithTimestamp(string ts, params object?[] lines)
        {
            if (!this.Endpoint.Enabled || lines.Length == 0)
                return;
            using TextWriter w = this.Endpoint.GetWriter()!;
            w.Write(ts);
            string tsBlank = MakeBlankTimestamp();
            string prefix = "";

            for (int i = 0, end = lines.Length; i < end; ++i)
            {
                if (i == 1)
                    prefix = tsBlank;

                w.Write(prefix);

                object? line = lines[i];

                if (line is null)
                {
                    continue;
                }
                else if (line is Exception ex)
                {
                    w.WriteLine($"{FormatExceptionMessage(ex, tsBlank)}");
                }
                else if (line is string s)
                { // Special handling for string types that removes newline characters; use an enumerable type for multiple lines.
                    if (s.Length > 0)
                        w.WriteLine($"{Regex.Replace(s, "\\r{0,1}\\n", $"\n{tsBlank}")}");
                }
                else if (line is IEnumerable enumerable)
                { // Special handling for enumerable types that prints each element on a new line.
                    IEnumerator? e = enumerable.GetEnumerator();

                    do
                    {
                        if (e.Current is object current)
                        { // print the first non-null line from the enumerable on the first line
                            w.WriteLine(e.Current);
                            break;
                        }
                    } while (e.MoveNext());

                    // Print the remaining lines
                    while (e.MoveNext())
                    {
                        if (e.Current is object current)
                            w.WriteLine($"{prefix}{current}");
                    }
                }
                else
                {
                    w.WriteLine(line);
                }
            }
            w.Flush();
            w.Close();
        }
        /// <param name="ts">The timestamp object.</param>
        /// <param name="lines">Any number of lines of any object type.</param>
        /// <inheritdoc cref="WriteWithTimestamp(string, object?[])"/>
        public void WriteWithTimestamp(ITimestamp ts, params object?[] lines) => this.WriteWithTimestamp(ts.ToString(), lines);

        /// <summary>Creates a trace message for the calling method using the <see cref="CallerMemberNameAttribute"/>, <see cref="CallerFilePathAttribute"/>, &amp; <see cref="CallerLineNumberAttribute"/> attributes.</summary>
        /// <remarks>Don't provide parameters or the attributes won't work.</remarks>
        /// <returns>A <see langword="string"/> containing the caller's name, filepath, and line number.</returns>
        public static string GetTrace([CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1) => $"{{ {callerMemberName}@'{callerFilePath}':{callerLineNumber} }}";

        /// <summary>Wrapper object for the <see cref="Conditional(ConditionalMessage[])"/> method.</summary>
        public struct ConditionalMessage
        {
            /// <inheritdoc cref="ConditionalMessage"/>
            /// <param name="type">The event type of this message.</param>
            /// <param name="message">Any number of objects to use as the message contents.</param>
            public ConditionalMessage(EventType type, params object?[] message)
            {
                this.EventType = type;
                this.Message = message;
            }
            /// <inheritdoc cref="ConditionalMessage"/>
            /// <param name="type">The event type of this message.</param>
            /// <param name="enumerable">Any enumerable type to use as the message contents.</param>
            public ConditionalMessage(EventType type, IEnumerable enumerable)
            {
                this.EventType = type;
                this.Message = enumerable;
            }
            /// <summary>
            /// The event type of this message.
            /// </summary>
            public EventType EventType { get; set; }
            /// <summary>
            /// The message contents.
            /// </summary>
            public IEnumerable Message { get; set; }
            /// <summary>
            /// Allows this object to be 'deconstructed' like a tuple.
            /// </summary>
            /// <param name="type">The event type.</param>
            /// <param name="enumerable">The message.</param>
            public void Deconstruct(out EventType type, out IEnumerable enumerable)
            {
                type = this.EventType;
                enumerable = this.Message;
            }
        }

        /// <summary>Conditional log message, allows you to write different messages depending on the current log filter level.</summary>
        /// <param name="messages">EventType-IEnumerable pairs. Only the message assoicated with the first pair whose event type is considered valid by <see cref="FilterEventType(EventType)"/> is printed.<br/>If none of the given event types are allowed, nothing happens.</param>
        public void Conditional(params ConditionalMessage[] messages)
        {
            for (int i = 0; i < messages.Length; ++i)
            {
                (EventType type, IEnumerable message) = messages[i];
                if (this.FilterEventType(type))
                {
                    this.WriteEvent(type, message);
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
            if (!this.Endpoint.Enabled || !this.FilterEventType(eventType))
                return;
            this.WriteWithTimestamp(this.MakeTimestamp(this.LastEventType = eventType), lines);
        }
        /// <summary>Writes a log message with a timestamp and log level header.</summary>
        /// <param name="pair">An event type, and any enumerable type.</param>
        public void WriteEvent((EventType, IEnumerable) pair) => this.WriteEvent(pair.Item1, pair.Item2);
        /// <summary>Writes a log message with a timestamp and log level header.</summary>
        /// <param name="eventType">An event type. Do not provide combinations of event type flags!</param>
        /// <param name="lines">Any enumerable type.</param>
        public void WriteEvent(EventType eventType, object?[] lines)
        {
            if (!this.Endpoint.Enabled || lines.Length == 0 || !this.FilterEventType(eventType))
                return;
            this.WriteWithTimestamp(this.MakeTimestamp(this.LastEventType = eventType), lines);
        }
        /// <summary>
        /// Write a formatted <see cref="EventType.DEBUG"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Debug(params object?[] lines) => this.WriteEvent(EventType.DEBUG, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.INFO"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Info(params object?[] lines) => this.WriteEvent(EventType.INFO, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.WARN"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Warning(params object?[] lines) => this.WriteEvent(EventType.WARN, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.ERROR"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Error(params object?[] lines) => this.WriteEvent(EventType.ERROR, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.FATAL"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Fatal(params object?[] lines) => this.WriteEvent(EventType.FATAL, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.CRITICAL"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Critical(params object?[] lines) => this.WriteEvent(EventType.CRITICAL, lines);
        /// <summary>
        /// Write a formatted <see cref="EventType.TRACE"/> message to the log endpoint.
        /// </summary>
        /// <param name="lines">Any number of objects. Each object will be written on a new line.</param>
        public void Trace(params object?[] lines) => this.WriteEvent(EventType.TRACE, lines);
        #endregion WriteEvent

        /// <summary>
        /// Appends the given lines to the log with a blank timestamp.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void Append(params object?[] lines)
        {
            if (!this.Endpoint.Enabled)
                return;
            this.WriteWithTimestamp(MakeBlankTimestamp(), lines);
        }
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void Followup(params object?[] lines) => this.Append(lines);
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="eventType">Message is only printed if this was the last event type to be printed.</param>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void FollowupIf(EventType eventType, params object?[] lines)
        {
            if (eventType.Equals(this.LastEventType))
                this.Append(lines);
        }
        /// <summary>
        /// Write a formatted followup message without a timestamp or event type to the log endpoint.
        /// </summary>
        /// <remarks>This is intended for writing quick follow-up messages that appear as if they were part of a previously written message.<br/>This function is unpredictable in multi-threaded environments.</remarks>
        /// <param name="predicate">A predicate that accepts the <see cref="LastEventType"/> to use as the condition.</param>
        /// <param name="lines">Any number of writable objects. Each element appears on a separate line.</param>
        public void FollowupIf(Predicate<EventType> predicate, params object?[] lines)
        {
            if (predicate(this.LastEventType))
                this.Append(lines);
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.Endpoint is IDisposable disp)
                        disp.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
