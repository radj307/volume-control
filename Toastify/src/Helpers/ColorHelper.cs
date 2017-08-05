using System.Drawing;
using System.Globalization;

namespace Toastify.Helpers
{
    internal static class ColorHelper
    {
        /// <summary>
        /// Hexadecimal to <see cref="Color"/> converter.
        /// </summary>
        /// <param name="hexColor"> Hex color. </param>
        /// <returns> A <see cref="Color"/>. </returns>
        public static Color HexToColor(string hexColor)
        {
            //Remove # if present
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            byte alpha = 0;
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            if (hexColor.Length == 8)
            {
                //#RRGGBB
                alpha = byte.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                red = byte.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                green = byte.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                blue = byte.Parse(hexColor.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            }

            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}