using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ToastifyAPI.GitHub.Model
{
    public class Emoji
    {
        #region Public Properties

        public string Name { get; set; }

        public string Url { get; set; }

        #endregion

        [NotNull]
        public string GetAsUnicodeString()
        {
            if (string.IsNullOrWhiteSpace(this.Url))
                return string.Empty;

            string codepoint = Regex.Replace(this.Url, @"^.*emoji/unicode/([a-fA-F0-9]+)\.png.*$", "$1", RegexOptions.Compiled);
            return int.TryParse(codepoint, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code) ? char.ConvertFromUtf32(code) : string.Empty;
        }
    }
}