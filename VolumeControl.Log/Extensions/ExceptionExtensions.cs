namespace VolumeControl.Log.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToString(this Exception ex, string multiLineBlank) => GetExceptionString(ex, multiLineBlank);

        /// <summary>
        /// Gets or sets the value of <see cref="Settings.Default.EnableStackTrace"/>.<br></br>
        /// This controls whether exception messages in the log include a stack trace.
        /// </summary>
        internal static bool EnableStackTrace => Properties.Settings.Default.EnableStackTrace;
        /// <summary>
        /// Gets or sets the value of <see cref="Settings.Default.EnableStackTraceLineCount"/>.<br></br>
        /// This controls whether the line number prefix is visible in the stack trace of exception messages.<br></br>
        /// Does nothing if <see cref="Settings.Default.EnableStackTrace"/> is false.
        /// </summary>
        internal static bool EnableStackTraceLineCount => Properties.Settings.Default.EnableStackTraceLineCount;

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
            string s = ex.Message;
            if (EnableStackTrace && ex.StackTrace?.Length > 0)
            {
                s += '\n'; // newline
                string st = ex.StackTrace;
                for (int i = st.IndexOf('\n'), lastNewLine = 0, len = st.Length, ln = 0; i != -1 && i < len; lastNewLine = i, i = st.IndexOf('\n', i + 1), ++ln)
                {
                    s += $"{indent}  {(EnableStackTraceLineCount ? $"[{ln}]\t" : "")}{st[lastNewLine..i].Trim("\n ")}\n";
                }
            }
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
        /// <param name="ts">Timestamp object.</param>
        /// <param name="recurse"><list type="table">
        /// <item><term>true</term><description>Recursively includes all inner exceptions in the returned string.</description></item>
        /// <item><term>false</term><description>Ignores all inner exceptions if they exist.</description></item>
        /// </list></param>
        /// <param name="indentSize">The number of space characters to insert before each line. This is only used when recurse is true.</param>
        /// <param name="indentStep">The amount to increase the indentSize by for every inner exception when recursing. This is only used when recurse is true.</param>
        /// <returns>The formatted exception message, stack trace, and inner exceptions if set to recurse, spanning over multiple lines.</returns>
        internal static string GetExceptionString(Exception ex, string blank, bool recurse = true)
            => GetExceptionStringRecursive(ex, blank.Length, 2, 0, recurse ? 5 : 1);
    }
}
