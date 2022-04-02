namespace VolumeControl.Log
{
    public class LogWriter : FileEndpoint
    {
        #region Constructors
        public LogWriter(string filename, EventType typeFilter = EventType.ALL_EXCEPT_DEBUG) : base(filename)
        {
            _eventFilter = typeFilter;
            Reset();

            // Set the margin sizes for each of the log entry's elements.
            _margin_time = 29;
            _margin_header = 8;
        }
        #endregion Constructors

        #region Members
        private EventType _eventFilter;
        /// <summary>
        /// The maximum length of the timestamp when printing to the log file.
        /// </summary>
        private readonly int _margin_time;
        /// <summary>
        /// The maximum length of the event type header when printing to the log file.
        /// </summary>
        private readonly int _margin_header;
        #endregion Members

        #region Methods
        /// <summary>
        /// Get a formatted timestamp with the given time point.
        /// </summary>
        /// <param name="timepoint">Input Timepoint</param>
        /// <param name="format">Optional format override. Default is UTC.</param>
        /// <returns>String containing just the timestamp with no extra spacing or indentation.</returns>
        internal static string MakeTimestamp(DateTime timepoint, string format = "U")
            => timepoint.ToString(format);

        /// <summary>
        /// Check if the given message type is allowed by the current message filter.
        /// </summary>
        /// <param name="type">The message type to check.</param>
        /// <returns>True when the message type IS allowed.</returns>
        internal bool FilterMessage(EventType type)
            => _eventFilter.Contains(type);

        #region WriteMethods
        /// <summary>
        /// Recursively converts an exception and all of its inner exceptions into a single printable string.
        /// </summary>
        /// <param name="ex">Input Exception object.</param>
        /// <param name="indentSize">The number of space characters to insert before each line.</param>
        /// <param name="indentStep">The amount to increase the indentSize by for every inner exception when recursing.</param>
        /// <returns>The formatted exception message, stack trace, and inner exceptions, spanning over multiple lines.</returns>
        internal static string GetExceptionStringRecursive(Exception ex, int indentSize, int indentStep, int recurseCount = 0, int maxRecurse = 5)
        {
            string indent = new(' ', indentSize);
            string s =
                $"Message:         '{ex.Message}'";
            if (ex.StackTrace?.Length > 0)
                s += $"\n{indent}Stack Trace:     '{ex.StackTrace}'";
            if (ex.Data.Count > 0)
            {
                s += $"\n{indent}Exception Data: {{";
                string innerIndent = new(' ', indentSize + indentStep);
                foreach (var it in ex.Data)
                {
                    string? itStr = it.ToString();
                    if (itStr != null && itStr.Length > 0)
                        s += $"{innerIndent}{itStr}\n";
                }
                s += $"{indent}}}";
            }
            if (ex.InnerException != null && ++recurseCount < maxRecurse)
                s += $"\n{(ex.InnerException != null ? $"Inner Exception:\n{GetExceptionStringRecursive(ex.InnerException, indentSize + indentStep, indentStep, recurseCount, maxRecurse)}" : "")}";
            return s;
        }
        /// <summary>
        /// Converts an exception into a single printable string, and optionally recurse for each sub-exception.
        /// The message spans over multiple lines.
        /// </summary>
        /// <param name="ex">Input Exception object.</param>
        /// <param name="recurse"><list type="table">
        /// <item><term>true</term><description>Recursively includes all inner exceptions in the returned string.</description></item>
        /// <item><term>false</term><description>Ignores all inner exceptions if they exist.</description></item>
        /// </list></param>
        /// <param name="indentSize">The number of space characters to insert before each line. This is only used when recurse is true.</param>
        /// <param name="indentStep">The amount to increase the indentSize by for every inner exception when recursing. This is only used when recurse is true.</param>
        /// <returns>The formatted exception message, stack trace, and inner exceptions if set to recurse, spanning over multiple lines.</returns>
        internal static string GetExceptionString(Exception ex, bool recurse = true, int indentSize = 0, int indentStep = 2)
            => GetExceptionStringRecursive(ex, indentSize, indentStep, 0, recurse ? 5 : 1);
        /// <summary>
        /// Get a string with a formatted header containing the current timestamp and the event type header.
        /// </summary>
        /// <param name="type">Determines the message header used.</param>
        /// <returns>The formatted header, with a length of (_margin_time + _margin_header)</returns>
        internal string GetFullHeader(EventType type)
        {
            string ts = Timestamp, head = type.GetHeader();
            return $"{ts}{new string(' ', _margin_time - ts.Length)} {head}{new string(' ', _margin_header - head.Length)}";
        }
        internal string GetBlankHeader()
        {
            return new string(' ', 1 + _margin_time + _margin_header);
        }

        /// <summary>
        /// Write multiple lines to the log file at once.
        /// </summary>
        /// <param name="type">Event type. Used in the header, and checking the event filter.</param>
        /// <param name="lines">String array where each element is a line.</param>
        /// <param name="autoIndent">When true, indentation will automatically be added to ensure log messages are aligned.</param>
        public void WriteLines(EventType type, string[] lines, bool autoIndent = true)
        {
            if (FilterMessage(type) && lines.Length > 0)
            {
                string indent = string.Empty;
                string header = GetFullHeader(type);
                if (autoIndent)
                    indent = GetBlankHeader();

                var writer = GetWriter(FileAccess.Write, FileShare.Read);
                writer.Write($"{header}{lines[0]}");
                foreach (string line in lines[1..])
                {
                    writer.WriteLine($"{indent}{line}");
                }
                writer.Close();
            }
        }

        /// <summary>
        /// Write a formatted event message to the logfile.
        /// This includes a timestamp & message header.
        /// </summary>
        /// <param name="type">Determines the message header used.</param>
        /// <param name="msg">The message string.</param>
        private void WriteEventMessage(EventType type, object? msg)
        {
            if (FilterMessage(type))
                WriteRawLine($"{GetFullHeader(type)} {msg}");
        }
        /// <summary>
        /// Write a formatted event message to the logfile.
        /// This includes a timestamp & message header.
        /// </summary>
        /// <param name="type">Determines the message header used.</param>
        /// <param name="msg_lines">An array of strings where each string uses one line.</param>
        private void WriteEventMessage(EventType type, object[] msg_lines)
        {
            if (msg_lines.Length > 0 && FilterMessage(type))
            {
                WriteRawLine($"{GetFullHeader(type)} {msg_lines[0]}");
                string indent = GetBlankHeader();
                for (int i = 1; i < msg_lines.Length; ++i)
                    WriteRawLine($"{indent}{msg_lines[i]}");
            }
        }

        /// <summary>
        /// Write a formatted debug message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteDebug(object? msg = null)
            => WriteEventMessage(EventType.DEBUG, msg);
        /// <summary>
        /// Write a formatted multi-line debug message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteDebug(object[] msg)
            => WriteEventMessage(EventType.DEBUG, msg);
        /// <summary>
        /// Write a formatted multi-line debug message to the logfile.
        /// This overload allows specifying the first line independently from the rest of the message, which allows you to print regular arrays with meaningful labels.
        /// </summary>
        /// <param name="fstLine">The text that will appear on the first line, directly after the timestamp and message header.</param>
        /// <param name="msg">The rest of the message.</param>
        public void WriteDebug(object fstLine, object[] msg)
            => WriteEventMessage(EventType.DEBUG, new[] { fstLine }.Concat(msg));
        /// <summary>
        /// Write a formatted informational message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteInfo(object? msg = null)
            => WriteEventMessage(EventType.INFO, msg);
        /// <summary>
        /// Write a formatted multi-line informational message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteInfo(object[] msg)
            => WriteEventMessage(EventType.INFO, msg);
        /// <summary>
        /// Write a formatted multi-line informational message to the logfile.
        /// This overload allows specifying the first line independently from the rest of the message, which allows you to print regular arrays with meaningful labels.
        /// </summary>
        /// <param name="fstLine">The text that will appear on the first line, directly after the timestamp and message header.</param>
        /// <param name="msg">The rest of the message.</param>
        public void WriteInfo(object fstLine, object[] msg)
            => WriteEventMessage(EventType.INFO, new[] { fstLine }.Concat(msg));
        /// <summary>
        /// Write a formatted warning message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteWarning(object? msg = null)
            => WriteEventMessage(EventType.WARN, msg);
        /// <summary>
        /// Write a formatted multi-line warning message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteWarning(object[] msg)
            => WriteEventMessage(EventType.WARN, msg);
        /// <summary>
        /// Write a formatted multi-line warning message to the logfile.
        /// This overload allows specifying the first line independently from the rest of the message, which allows you to print regular arrays with meaningful labels.
        /// </summary>
        /// <param name="fstLine">The text that will appear on the first line, directly after the timestamp and message header.</param>
        /// <param name="msg">The rest of the message.</param>
        public void WriteWarning(object fstLine, object[] msg)
            => WriteEventMessage(EventType.WARN, new[] { fstLine }.Concat(msg));
        /// <summary>
        /// Write a formatted error message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteError(object? msg = null)
            => WriteEventMessage(EventType.ERROR, msg);
        /// <summary>
        /// Write a formatted multi-line error message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteError(object[] msg)
            => WriteEventMessage(EventType.ERROR, msg);
        /// <summary>
        /// Write a formatted multi-line error message to the logfile.
        /// This overload allows specifying the first line independently from the rest of the message, which allows you to print regular arrays with meaningful labels.
        /// </summary>
        /// <param name="fstLine">The text that will appear on the first line, directly after the timestamp and message header.</param>
        /// <param name="msg">The rest of the message.</param>
        public void WriteError(object fstLine, object[] msg)
            => WriteEventMessage(EventType.ERROR, new[] { fstLine }.Concat(msg));
        /// <summary>
        /// Write a formatted fatal error message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteFatal(object? msg = null)
            => WriteEventMessage(EventType.FATAL, msg);
        /// <summary>
        /// Write a formatted multi-line fatal error message to the logfile.
        /// </summary>
        /// <param name="msg">The message string.</param>
        public void WriteFatal(object[] msg)
            => WriteEventMessage(EventType.FATAL, msg);
        /// <summary>
        /// Write a formatted multi-line fatal error message to the logfile.
        /// This overload allows specifying the first line independently from the rest of the message, which allows you to print regular arrays with meaningful labels.
        /// </summary>
        /// <param name="fstLine">The text that will appear on the first line, directly after the timestamp and message header.</param>
        /// <param name="msg">The rest of the message.</param>
        public void WriteFatal(object fstLine, object[] msg)
            => WriteEventMessage(EventType.FATAL, new[] { fstLine }.Concat(msg));

        /// <summary>
        /// Writes a formatted exception message to the logfile.
        /// </summary>
        /// <param name="type">An EventType to use for the message header.</param>
        /// <param name="ex">The Exception instance to write.</param>
        private void WriteException(EventType type, Exception ex)
            => WriteEventMessage(type, GetExceptionString(ex));
        private void WriteException(EventType type, string fstLine, Exception ex)
            => WriteEventMessage(type, $"{fstLine}\n{GetBlankHeader()}{GetExceptionString(ex)}");
        /// <summary>
        /// Write a formatted debug exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionDebug(Exception ex)
            => WriteException(EventType.DEBUG, ex);
        public void WriteExceptionDebug(string fstLine, Exception ex)
            => WriteException(EventType.DEBUG, fstLine, ex);
        /// <summary>
        /// Write a formatted informational exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionInfo(Exception ex)
            => WriteException(EventType.INFO, ex);
        public void WriteExceptionInfo(string fstLine, Exception ex)
            => WriteException(EventType.INFO, fstLine, ex);
        /// <summary>
        /// Write a formatted warning exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionWarn(Exception ex)
            => WriteException(EventType.WARN, ex);
        public void WriteExceptionWarn(string fstLine, Exception ex)
            => WriteException(EventType.WARN, fstLine, ex);
        /// <summary>
        /// Write a formatted error exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionError(Exception ex)
            => WriteException(EventType.ERROR, ex);
        public void WriteExceptionError(string fstLine, Exception ex)
            => WriteException(EventType.ERROR, fstLine, ex);
        /// <summary>
        /// Write a formatted fatal error exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionFatal(Exception ex)
            => WriteException(EventType.FATAL, ex);
        /// <summary>
        /// Write a formatted fatal error exception to the logfile.
        /// </summary>
        /// <param name="fstLine">Text shown on the first line, directly after the timestamp and message header.</param>
        /// <param name="ex"></param>
        public void WriteExceptionFatal(string fstLine, Exception ex)
            => WriteException(EventType.FATAL, fstLine, ex);
        #endregion WriteMethods

        #endregion Methods

        #region Properties
        /// <summary>
        /// Get the current timestamp, using the format string "U" for standard UTC format.
        /// </summary>
        internal static string Timestamp
        {
            get => MakeTimestamp(DateTime.Now, "U");
        }
        /// <summary>
        /// Get or set the message type filter.
        /// </summary>
        public EventType EventTypeFilter
        {
            get => _eventFilter;
            set => _eventFilter = value;
        }
        #endregion Properties
    }
}
