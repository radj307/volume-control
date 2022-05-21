using Semver;

namespace VolumeControl.Core.Extensions
{
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
    }
}
