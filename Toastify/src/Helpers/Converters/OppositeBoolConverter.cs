using System;
using System.Globalization;
using System.Windows.Data;

namespace Toastify.Helpers.Converters
{
    public class OppositeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Note: don't check validity etc, since we'll need to throw an exception anyway, may as well let
            //       that exception be here and not waste time :)
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}