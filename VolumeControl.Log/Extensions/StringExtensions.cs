namespace VolumeControl.Log.Extensions
{
    public static class StringExtensions
    {
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
