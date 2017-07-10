using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Toastify.Helpers
{
    public class BoolAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Aggregate(true, (current, value) => current && (bool)value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}