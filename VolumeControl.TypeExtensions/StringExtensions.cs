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
        public static SemVersion? GetSemVer(this string s)
        {
            if (SemVersion.TryParse(s.Trim(), SemVersionStyles.OptionalPatch, out SemVersion result))
                return result;
            return null;
        }
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
    }
}
