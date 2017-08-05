using System;
using System.Globalization;
using System.Windows.Data;

namespace Toastify.Helpers
{
    public class HexColorToAlphaValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hexColor = (string)value ?? string.Empty;
            return byte.Parse(hexColor.Substring(1, 2), NumberStyles.AllowHexSpecifier);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}