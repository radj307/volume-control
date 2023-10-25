using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace VolumeControl.Log
{
    /// <summary>
    /// Helper methods for converting exceptions into nicely formatted strings.
    /// </summary>
    /// <remarks>
    /// This is used internally by <see cref="AsyncLogWriter"/> to format exceptions.
    /// </remarks>
    public static class ExceptionMessageHelper
    {
        #region MessageParts
        /// <summary>
        /// Defines the sections in a formatted exception message string.
        /// </summary>
        [Flags]
        public enum MessageParts
        {
            /// <summary>
            /// Nothing.
            /// </summary>
            None = 0,
            /// <summary>
            /// The <see cref="Exception.Message"/> property.
            /// </summary>
            Message = 1,
            /// <summary>
            /// The <see cref="Exception.HResult"/> property.
            /// </summary>
            HResult = 2,
            /// <summary>
            /// The <see cref="Type"/> of the exception object.
            /// </summary>
            ExceptionType = 4,
            /// <summary>
            /// The <see cref="Exception.HelpLink"/> property.
            /// </summary>
            HelpLink = 8,
            /// <summary>
            /// Properties defined in derived exception objects.
            /// </summary>
            CustomProperties = 16,
            /// <summary>
            /// The <see cref="Exception.Source"/> property.
            /// </summary>
            Source = 32,
            /// <summary>
            /// The <see cref="Exception.TargetSite"/> property.
            /// </summary>
            TargetSite = 64,
            /// <summary>
            /// The <see cref="Exception.Data"/> property.
            /// </summary>
            Data = 128,
            /// <summary>
            /// The <see cref="Exception.StackTrace"/> property.
            /// </summary>
            StackTrace = 256,
            /// <summary>
            /// The <see cref="Exception.InnerException"/> property.
            /// </summary>
            InnerException = 512,
            /// <summary>
            /// All other values in the <see cref="MessageParts"/> enumeration.
            /// </summary>
            All = Message | HResult | ExceptionType | HelpLink | CustomProperties | Source | TargetSite | Data | StackTrace | InnerException,
        }
        #endregion MessageParts

        #region Methods

        #region MakeExceptionMessage
        /// <summary>
        /// Creates a nicely-formatted exception message from the specified <paramref name="exception"/> object.
        /// </summary>
        /// <param name="exception"><see cref="Exception"/> object to create a message for.</param>
        /// <param name="linePrefix"><see cref="string"/> prefix inserted before every line except for the first one.</param>
        /// <param name="endline"><see cref="string"/> suffix appended to every line except for the last one.</param>
        /// <param name="tabLength">The number of spaces to use for each tab character.</param>
        /// <param name="includedParts">Value(s) from the <see cref="MessageParts"/> enumeration that determines what is included in the message.</param>
        /// <returns>Psuedo-JSON formatted exception message as a <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> was <see langword="null"/>.</exception>
        public static string MakeExceptionMessage(Exception exception, string linePrefix, string endline, int tabLength, MessageParts includedParts)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
            else if (includedParts == MessageParts.None)
                return $"{{{endline}{linePrefix}}}";

            var sb = new StringBuilder();

            // create helper strings
            string tab = tabLength > 0 ? new(' ', tabLength) : string.Empty;
            string tabPrefix = linePrefix + tab;

            var exceptionType = exception.GetType();

            // opening bracket
            sb.Append('{' + endline);

            // Message:
            if (includedParts.HasFlag(MessageParts.Message))
                sb.Append($"{tabPrefix}\"Message\": \"{exception.Message}\"{endline}");

            // HResult:
            if (includedParts.HasFlag(MessageParts.HResult) && !exception.HResult.Equals(-2146233088)) //< only include HResult when it isn't set to the default one
                sb.Append($"{tabPrefix}\"HResult\": \"{exception.HResult}\"{endline}");

            // ExceptionType (custom):
            if (includedParts.HasFlag(MessageParts.ExceptionType))
            {
                var exTypeName = exceptionType.ToString(); //< we do this to handle generic types correctly
                if (!exception.Message.Contains(exTypeName, StringComparison.Ordinal)) //< include when not using default Message that includes the typename in it
                    sb.Append($"{tabPrefix}\"ExceptionType\": \"{exTypeName}\"{endline}");
            }

            // HelpLink
            if (includedParts.HasFlag(MessageParts.HelpLink) && exception.HelpLink != null)
                sb.Append($"{tabPrefix}\"HelpLink\": \"{exception.HelpLink}\"{endline}");

            // Custom Properties:
            if (includedParts.HasFlag(MessageParts.CustomProperties))
            {
                // this approach is necessary because derived exceptions may override properties
                //  that were already included (and properly formatted) elsewhere in the message:
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                Type baseExceptionType = typeof(Exception);
                // enumerate inherited types until we reach the base Exception type
                for (Type? type = exceptionType; type != null && type != baseExceptionType; type = type.BaseType)
                {
                    // get properties declared in this type
                    foreach (var propInfo in type.GetProperties(bindingFlags))
                    {
                        // skip unreadable properties
                        if (!propInfo.CanRead) continue;

                        // ensure this property does not override a base property
                        if (propInfo.GetMethod!.GetBaseDefinition().DeclaringType?.Equals(type) ?? false)
                        { // this property is not an override
                            var value = propInfo.GetValue(exception);

                            // skip properties with null/empty values
                            if (value == null || (value is string s && s.Length == 0))
                                continue;

                            sb.Append($"{tabPrefix}\"{propInfo.Name}\": \"{value}\"{endline}");
                        }
                    }
                }
            }

            // Source:
            if (includedParts.HasFlag(MessageParts.Source) && exception.Source != null)
            {
                sb.Append($"{tabPrefix}\"Source\": \"{exception.Source}\"{endline}");
            }

            // TargetSite:
            if (includedParts.HasFlag(MessageParts.TargetSite))
            {
                try
                {
                    var subsb = new StringBuilder();
                    if (exception.TargetSite != null)
                    {
                        subsb.Append($"{tabPrefix}\"TargetSite\": {{{endline}");
                        subsb.Append($"{tabPrefix}{tab}\"Name\": \"{exception.TargetSite.Name}\"{endline}");
                        if (exception.TargetSite.DeclaringType != null)
                            subsb.Append($"{tabPrefix}{tab}\"DeclaringType\": \"{exception.TargetSite.DeclaringType.FullName}\"{endline}");
                        subsb.Append($"{tabPrefix}{tab}\"Attributes\": \"{exception.TargetSite.Attributes:G}\"{endline}");
                        subsb.Append($"{tabPrefix}{tab}\"CallingConvention\": \"{exception.TargetSite.CallingConvention:G}\"{endline}");
                        subsb.Append($"{tabPrefix}}}{endline}");
                    }
                    sb.Append(subsb);
                }
                catch (TypeLoadException typeLoadException) //< accessing TargetSite may rarely trigger TypeLoadExceptions
                {
                    sb.Append($"{tabPrefix}\"TargetSite\": (ERROR - TypeLoadException {typeLoadException.TypeName}) {MakeExceptionMessage(typeLoadException, tabPrefix + tab, endline, tabLength, includedParts)}{endline}");
                }
            }

            // Data:
            if (includedParts.HasFlag(MessageParts.Data) && exception.Data != null && exception.Data.Count > 0)
            {
                sb.Append($"{tabPrefix}\"Data\": {{{endline}");
                foreach (DictionaryEntry kvp in exception.Data)
                {
                    sb.Append($"{tabPrefix}{tab}\"{kvp.Key}\": \"{kvp.Value}\"{endline}");
                }
                sb.Append($"{tabPrefix}}}{endline}");
            }

            // StackTrace:
            if (includedParts.HasFlag(MessageParts.StackTrace) && exception.StackTrace != null)
            {
                var traces = exception.StackTrace.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (traces.Length > 0)
                {
                    sb.Append($"{tabPrefix}\"StackTrace\": {{{endline}");

                    for (int i = 0, i_max = traces.Length; i < i_max; ++i)
                    {
                        sb.Append($"{tabPrefix}{tab}[{i}] {traces[i]}{endline}");
                    }

                    sb.Append($"{tabPrefix}}}{endline}");
                }
            }

            // InnerException:
            if (includedParts.HasFlag(MessageParts.InnerException) && exception.InnerException != null)
            {
                sb.Append($"{tabPrefix}\"InnerException\": {MakeExceptionMessage(exception.InnerException, tabPrefix, endline, tabLength, includedParts)}{endline}");
            }

            // closing bracket
            sb.Append(linePrefix + '}');

            return sb.ToString();
        }
        /// <inheritdoc cref="MakeExceptionMessage(Exception, string, string, int, MessageParts)"/>
        public static string MakeExceptionMessage(Exception exception, string linePrefix = "", int tabLength = 2, MessageParts includedParts = MessageParts.All)
            => MakeExceptionMessage(exception, linePrefix, Environment.NewLine, tabLength, includedParts);
        /// <summary>
        /// Creates a single-line exception message from the specified <paramref name="exception"/> object.
        /// </summary>
        /// <inheritdoc cref="MakeExceptionMessage(Exception, string, string, int, MessageParts)"/>
        public static string MakeSerialExceptionMessage(Exception exception, MessageParts includedParts = MessageParts.All)
            => MakeExceptionMessage(exception, linePrefix: string.Empty, string.Empty, 0, includedParts);
        #endregion MakeExceptionMessage

        #endregion Methods
    }
}
