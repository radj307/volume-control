using System.Reflection;

namespace VolumeControl.Core.Logging
{
    public class LogWriter : Endpoint
    {
        #region Constructors
        public LogWriter(string filename, EventType typeFilter = EventType.ALL_EXCEPT_DEBUG) : base(filename, FileAccess.Write)
        {
            _eventFilter = typeFilter;
        }
        #endregion Constructors

        #region Members
        private EventType _eventFilter;
        private readonly int _margin_time = 30, _margin_header = 10;
        #endregion Members

        #region Methods
        internal static string MakeTimestamp(DateTime timepoint, string format = "U")
            => timepoint.ToString(format);

        internal bool FilterMessage(EventType type)
            => _eventFilter.Contains(type);

        #region WriteMethods
        /// <summary>
        /// Write to the filestream.
        /// It is highly recommended that you do not use this function, as it doesn't conform to formatting rules.
        /// </summary>
        /// <param name="str">A string to write.</param>
        public void WriteRaw(string? str)
            => Writer.Write(str);
        /// <summary>
        /// Write to the filestream, and append a newline.
        /// It is highly recommended that you do not use this function, as it doesn't conform to formatting rules.
        /// </summary>
        /// <param name="str">A string to write.</param>
        public void WriteRawLine(string? str = null)
            => Writer.WriteLine(str);

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
                $"{indent}Message:         '{ex.Message}'\n" +
                $"{indent}Stack Trace:     '{ex.StackTrace}'";
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

        /// <summary>
        /// Write a formatted event message to the logfile.
        /// This includes a timestamp & message header.
        /// </summary>
        /// <param name="type">Determines the message header used.</param>
        /// <param name="msg">The message string.</param>
        private void WriteEventMessage(EventType type, object? msg)
        {
            if (FilterMessage(type))
            {
                WriteRawLine($"{GetFullHeader(type)} \'{msg}\'");
            }
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
                WriteRawLine($"{GetFullHeader(type)} \'{msg_lines[0]}\'");
                string indent = new(' ', _margin_time + _margin_header);
                for (int i = 1; i < msg_lines.Length; ++i)
                    WriteRawLine($"{indent} \'{msg_lines[i]}\'");
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
        /// Writes a formatted exception message to the logfile.
        /// </summary>
        /// <param name="type">An EventType to use for the message header.</param>
        /// <param name="ex">The Exception instance to write.</param>
        private void WriteException(EventType type, Exception ex)
            => WriteEventMessage(type, GetExceptionString(ex));

        /// <summary>
        /// Write a formatted debug exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionDebug(Exception ex)
            => WriteException(EventType.DEBUG, ex);
        /// <summary>
        /// Write a formatted informational exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionInfo(Exception ex)
            => WriteException(EventType.INFO, ex);
        /// <summary>
        /// Write a formatted warning exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionWarn(Exception ex)
            => WriteException(EventType.WARN, ex);
        /// <summary>
        /// Write a formatted error exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionError(Exception ex)
            => WriteException(EventType.ERROR, ex);
        /// <summary>
        /// Write a formatted fatal error exception to the logfile.
        /// </summary>
        /// <param name="ex">The exception message to print.</param>
        public void WriteExceptionFatal(Exception ex)
            => WriteException(EventType.FATAL, ex);
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
        public EventType EventTypeFilter
        {
            get => _eventFilter;
            set => _eventFilter = value;
        }
        #endregion Properties
    }
}
