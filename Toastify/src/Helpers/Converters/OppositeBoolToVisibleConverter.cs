using System;
using System.Windows;
using System.Windows.Data;

namespace Toastify.Helpers.Converters
{
    public class OppositeBoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Note: will throw a cast exception if you throw the wrong type at it. Good :)
            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}