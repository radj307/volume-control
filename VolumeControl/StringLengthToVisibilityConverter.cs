using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VolumeControl
{
    public class StringLengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.Equals(typeof(Visibility)) && value is string s)
            {
                return s.Length == 0 ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
