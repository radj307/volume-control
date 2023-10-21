namespace VolumeControl.Core.Helpers
{
    /// <summary>
    /// Defines helper methods for creating nicely-formatted strings.
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
        public static string Indent(int maxLength, int usedLength, bool allowZeroLength = false)
            => Indent(maxLength, usedLength, ' ', allowZeroLength);
        #endregion Indent
    }
}
