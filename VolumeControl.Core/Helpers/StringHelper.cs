using System.Reflection;
using System.Text.RegularExpressions;

namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Defines helper methods &amp; extension methods for creating nicely-formatted strings.
    /// </summary>
    public static class StringHelper
    {
        #region MakeAlternatingPattern
        /// <summary>
        /// Returns a new <see cref="string"/> containing an alternating pattern of the specified characters with the specified <paramref name="length"/>.
        /// </summary>
        /// <remarks>
        /// This method is used to create spacer strings for text-based displays rendered with monospace fonts.
        /// The specified <paramref name="dotChar"/> will never be the first or last character, so if the specified <paramref name="length"/> is 2 or less, the returned string will only contain <paramref name="fillChar"/>s.<br/>
        /// To automatically calculate the needed length, see the <see cref="IndentWithPattern"/> method.
        /// </remarks>
        /// <param name="length">The length of the returned string.</param>
        /// <param name="fillChar">The pattern fill character.</param>
        /// <param name="dotChar">The pattern dot character.</param>
        /// <param name="alignRight">When <see langword="true"/>, the pattern is aligned to the right; otherwise when <see langword="false"/> it is aligned to the left.</param>
        /// <returns>An alternating pattern of the specified <paramref name="fillChar"/> &amp; <paramref name="dotChar"/> with the specified <paramref name="length"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> was negative.</exception>
        public static string MakeAlternatingPattern(int length, char fillChar = ' ', char dotChar = '.', bool alignRight = true)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length cannot be negative!");
            else if (length <= 2)
                return new string(fillChar, length);

            char[] chars = new char[length];
            bool alternator = false;

            for (int i = 0; i < length; ++i, alternator = !alternator)
            {
                switch (alternator) //< using a proper switch statement here is the fastest way to do this
                {
                case true:
                    chars[i] = dotChar;
                    break;
                case false:
                    chars[i] = fillChar;
                    break;
                }
            }

            // make sure the dots don't actually touch either side
            int lastIndex = length - 1;
            if (chars[lastIndex] == dotChar)
                chars[lastIndex] = fillChar;

            // align pattern to the right
            if (alignRight) Array.Reverse(chars);

            return new(chars);
        }
        #endregion MakeAlternatingPattern

        #region IndentWithPattern
        /// <summary>
        /// Returns an alternating pattern long enough to fill the specified <paramref name="maxLength"/> after subtracting the <paramref name="usedLength"/>.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="usedLength"/> is greater than the <paramref name="maxLength"/>, the returned string contains exactly 1 <paramref name="fillChar"/> instead of throwing an exception.
        /// </remarks>
        /// <param name="maxLength">The length of the space to fill.</param>
        /// <param name="usedLength">The number of characters out of the specified <paramref name="maxLength"/> that have already been used.</param>
        /// <param name="fillChar">The pattern fill character.</param>
        /// <param name="dotChar">The pattern dot character.</param>
        /// <param name="allowZeroLength">When <see langword="true"/>, the returned string can be empty; otherwise when <see langword="false"/>, it always has at least one character.</param>
        /// <param name="alignRight">When <see langword="true"/>, the pattern is aligned to the right; otherwise when <see langword="false"/> it is aligned to the left.</param>
        /// <returns>An indentation <see cref="string"/> made up of alternating characters with the requested length.</returns>
        public static string IndentWithPattern(int maxLength, int usedLength = 0, char fillChar = ' ', char dotChar = '.', bool allowZeroLength = false, bool alignRight = true)
        {
            int length = maxLength - usedLength;

            return length <= 0
                ? allowZeroLength ? string.Empty : new(fillChar, 1)
                : MakeAlternatingPattern(length, fillChar, dotChar, alignRight);
        }
        #endregion IndentWithPattern

        #region Indent
        /// <summary>
        /// Returns a string long enough to fill the specified <paramref name="maxLength"/> after subtracting the specified <paramref name="usedLength"/>.
        /// </summary>
        /// <param name="maxLength">The length of the space to fill.</param>
        /// <param name="usedLength">The number of characters out of the specified <paramref name="maxLength"/> that have already been used.</param>
        /// <param name="fillChar">The character to use.</param>
        /// <param name="allowZeroLength">When <see langword="true"/>, the returned string can be empty; otherwise when <see langword="false"/>, it always has at least one character.</param>
        /// <returns>An indentation <see cref="string"/> with the requested length.</returns>
        public static string Indent(int maxLength, int usedLength, char fillChar = ' ', bool allowZeroLength = false)
        {
            int length = maxLength - usedLength;

            return length <= 0
                ? allowZeroLength ? string.Empty : new(fillChar, 1)
                : new(fillChar, length);
        }
        /// <inheritdoc cref="Indent(int, int, char, bool)"/>
        public static string Indent(int maxLength, int usedLength, bool allowZeroLength)
            => Indent(maxLength, usedLength, ' ', allowZeroLength);
        #endregion Indent

        #region GetFullMethodName
        /// <summary>
        /// Defines the components of a method name.
        /// </summary>
        [Flags]
        public enum MethodNamePart
        {
            /// <summary>
            /// Shows only the declaring type name and the method name, with empty brackets.
            /// </summary>
            None = 0,
            /// <summary>
            /// Shows namespace qualifiers for all types.
            /// </summary>
            FullTypeNames = 1,
            /// <summary>
            /// Shows namespace qualifiers for the declaring type.
            /// </summary>
            FullDeclaringTypeName = 2,
            /// <summary>
            /// Shows generic type parameters.
            /// </summary>
            GenericParameters = 4,
            /// <summary>
            /// Shows parameter types.
            /// </summary>
            ParameterTypes = 8,
            /// <summary>
            /// Shows parameter names.
            /// </summary>
            ParameterNames = 16,
            /// <summary>
            /// Shows parameter types and names.
            /// </summary>
            Parameters = ParameterTypes | ParameterNames,
            /// <summary>
            /// Shows the return type, but only if it isn't <see cref="void"/>.
            /// </summary>
            NonVoidReturnType = 32,
            /// <summary>
            /// Shows the return type, even if it is <see cref="void"/>.
            /// </summary>
            VoidReturnType = 64,
            /// <summary>
            /// Shows the return type.
            /// </summary>
            ReturnType = NonVoidReturnType | VoidReturnType,
        }
        /// <summary>
        /// Gets the name of the method with the specified <paramref name="includedNameComponents"/>.
        /// </summary>
        /// <param name="method">(implicit) The <see cref="MethodInfo"/> object of the method to get the name of.</param>
        /// <param name="includedNameComponents">The parts of the method name to include.</param>
        /// <returns>The name of the <paramref name="method"/> with the <paramref name="includedNameComponents"/> shown.</returns>
        public static string GetFullMethodName(this MethodInfo method, MethodNamePart includedNameComponents)
        {
            string fullName = string.Empty;
            bool showFullTypeNames = includedNameComponents.HasFlag(MethodNamePart.FullTypeNames);

            // type name
            if (method.DeclaringType != null)
            {
                fullName += showFullTypeNames || includedNameComponents.HasFlag(MethodNamePart.FullDeclaringTypeName)
                    ? method.DeclaringType.FullName
                    : method.DeclaringType.Name;
                fullName += '.';
            }
            // method name
            fullName += method.Name;
            // generic parameters
            if (method.IsGenericMethod && includedNameComponents.HasFlag(MethodNamePart.GenericParameters))
            {
                // generic opening bracket
                fullName += '<';
                // generics
                var genericParams = method.GetGenericMethodDefinition().GetGenericArguments();
                for (int i = 0, max = genericParams.Length; i < max; ++i)
                {
                    // generic name
                    fullName += genericParams[i].Name;

                    // separator
                    if (i + 1 < max)
                        fullName += ", ";
                }
                // generic closing bracket
                fullName += '>';
            }
            // opening bracket
            fullName += '(';
            // parameters
            bool showParamTypes = includedNameComponents.HasFlag(MethodNamePart.ParameterTypes);
            bool showParamNames = includedNameComponents.HasFlag(MethodNamePart.ParameterNames);
            if (showParamTypes || showParamNames)
            {
                var parameters = method.GetParameters();
                for (int i = 0, max = parameters.Length; i < max; ++i)
                {
                    // parameter type
                    if (showParamTypes)
                    {
                        var paramType = parameters[i].ParameterType;
                        fullName += showFullTypeNames ? paramType.FullName : paramType.Name;
                    }
                    // parameter name
                    if (showParamNames)
                    {
                        if (showParamTypes)
                            fullName += ' ';
                        fullName += parameters[i].Name;
                    }
                    // separator
                    if (i + 1 < max)
                        fullName += ", ";
                }
            }
            // closing bracket
            fullName += ')';
            // return type
            bool showNonVoidReturnType = includedNameComponents.HasFlag(MethodNamePart.NonVoidReturnType);
            bool showVoidReturnType = includedNameComponents.HasFlag(MethodNamePart.VoidReturnType);
            bool returnTypeIsVoid = method.ReturnType.Equals(typeof(void));
            if ((showNonVoidReturnType && !returnTypeIsVoid) || (showVoidReturnType && returnTypeIsVoid))
            {
                fullName += " => ";
                fullName += showFullTypeNames ? method.ReturnType.FullName : method.ReturnType.Name;
            }

            return fullName;
        }
        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// <param name="method">(implicit) The <see cref="MethodInfo"/> object of the method to get the name of.</param>
        /// <returns>The full name of the <paramref name="method"/>, including the full declaring typename, generic parameters, parameters, and return type (if not <see cref="void"/>).</returns>
        public static string GetFullMethodName(this MethodInfo method)
            => GetFullMethodName(method, MethodNamePart.FullDeclaringTypeName | MethodNamePart.GenericParameters | MethodNamePart.Parameters | MethodNamePart.NonVoidReturnType);
        #endregion GetFullMethodName

        #region PartialFormat
        /// <summary>
        /// Emulates <see cref="string.Format(string, object?[])"/>, but allows partial formatting for specific arguments rather than all of them at once.
        /// </summary>
        public static string PartialFormat(string format, params (int, object)[] formatArgs)
        {
            var matches = Regex.Matches(format, @"{(\d+)(,-{0,1}\d+){0,1}(:[\w!@#$%^&*()_+-=\/\\|;:'"",.<>?`~]+){0,1}}");

            if (matches.Count == 0)
                return format;

            int providedCount = formatArgs.Length;

            bool TryGetArgForIndex(int index, out object arg)
            {
                for (int i = 0; i < providedCount; ++i)
                {
                    var (k, v) = formatArgs[i];
                    if (k == index)
                    {
                        arg = v;
                        return true;
                    }
                }
                arg = null!;
                return false;
            }
            List<object> args = new();

            foreach (var m in (IEnumerable<Match>)matches)
            {
                var argIndex = int.Parse(m.Groups[1].Value);

                if (TryGetArgForIndex(argIndex, out var arg))
                    args.Add(string.Format(m.Value.Replace(m.Groups[1].Value, "0"), arg));
                else args.Add(m.Value);
            }

            return string.Format(format, args.ToArray());
        }
        #endregion PartialFormat
    }
}
