using Semver;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extensions for string types.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Parses a string containing a version number in semantic versioning 2 format.
        /// </summary>
        public static SemVersion? GetSemVer(this string? s) => s is null ? null : SemVersion.TryParse(s.Trim(), SemVersionStyles.OptionalPatch, out SemVersion result) ? result : null;
        /// <summary>
        /// Removes all chars that <paramref name="pred"/> returns true for.
        /// </summary>
        public static string RemoveIf(this string s, Predicate<char> pred)
        {
            for (int i = s.Length - 1; i >= 0; --i)
            {
                if (pred(s[i]))
                    s = s.Remove(i, i);
            }

            return s;
        }
        /// <summary>
        /// Removes all preceeding/trailing occurrences of the specified characters from a string.
        /// </summary>
        /// <param name="s">The input string.</param>
        /// <param name="trimChars">Any number of characters in a string.</param>
        /// <returns>String with all preceeding/trailing characters from trimChars removed.</returns>
        public static string Trim(this string s, string trimChars)
            => s.Trim(trimChars.ToCharArray());
        /// <summary>
        /// Gets the <see langword="char"/> at <paramref name="index"/>, or <paramref name="defaultChar"/> if the index is out of range.
        /// </summary>
        /// <param name="s">The string that this extension method was called on.</param>
        /// <param name="index">The target index within the string to access.</param>
        /// <param name="defaultChar">A character to return when the index is out-of-range.</param>
        /// <returns>The character at <paramref name="index"/> in <paramref name="s"/> if the index is within range; otherwise <paramref name="defaultChar"/>.</returns>
        public static char AtIndexOrDefault(this string s, int index, char defaultChar) => index < s.Length ? s[index] : defaultChar;
        /// <summary>
        /// Gets the <see langword="char"/> at <paramref name="index"/>, or the result of the <see langword="default"/> keyword if the index is out of range.
        /// </summary>
        /// <returns>The character at <paramref name="index"/> in <paramref name="s"/> if the index is within range; otherwise <see langword="default"/>.</returns>
        /// <inheritdoc cref="AtIndexOrDefault(string, int, char)"/>
        public static char AtIndexOrDefault(this string s, int index) => AtIndexOrDefault(s, index, default);
        /// <summary>
        /// Check if <paramref name="s"/> equals any of the given <paramref name="compare"/> strings using <paramref name="sCompareType"/> comparison.
        /// </summary>
        /// <param name="s">A string.</param>
        /// <param name="sCompareType">The <see cref="StringComparison"/> to use.</param>
        /// <param name="compare">Any number of strings to compare to <paramref name="s"/>.</param>
        /// <returns><see langword="true"/> when <paramref name="s"/> equals at least one of the <paramref name="compare"/> strings; otherwise <see langword="false"/></returns>
        public static bool EqualsAny(this string s, StringComparison sCompareType, params string[] compare) => compare.Any(c => s.Equals(c, sCompareType));
        /// <summary>
        /// Check if <paramref name="s"/> equals any of the given <paramref name="compare"/> strings using <see cref="StringComparison.Ordinal"/> comparison.
        /// </summary>
        /// <inheritdoc cref="EqualsAny(string, StringComparison, string[])"/>
        public static bool EqualsAny(this string s, params string[] compare) => EqualsAny(s, StringComparison.Ordinal, compare);
    }
}
