using System;
using System.Globalization;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Toastify.Helpers
{
    internal static class ColorHelper
    {
        #region Static Members

        /// <summary>
        ///     Hexadecimal to <see cref="Color" /> converter.
        /// </summary>
        /// <param name="hexColor"> Hex color. </param>
        /// <returns> A <see cref="Color" />. </returns>
        public static Color HexToColor([NotNull] string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            byte alpha = 0;
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            try
            {
                if (hexColor.Length == 8)
                {
                    // #AARRGGBB
                    alpha = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                    red = byte.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    green = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                    blue = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return Color.FromArgb(alpha, red, green, blue);
        }

        public static string ColorToHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        #endregion
    }
}