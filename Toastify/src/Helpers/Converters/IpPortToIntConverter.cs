using System;
using System.Globalization;
using System.Windows.Data;

// ReSharper disable BuiltInTypeReferenceStyle
namespace Toastify.Helpers.Converters
{
    public class IpPortToIntConverter : IValueConverter
    {
        private const UInt16 @default = 80;

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            if (string.IsNullOrWhiteSpace(s))
                s = @default.ToString();

            return UInt16.TryParse(s, out UInt16 ret) ? ret : @default;
        }
    }
}