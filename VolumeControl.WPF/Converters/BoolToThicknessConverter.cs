using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts from bool types to thickness types.
    /// </summary>
    public class BoolToThicknessConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null ? new Thickness(2) : (object)new Thickness(0);
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    /// <summary>
    /// Converts from bool types to visibility types.
    /// </summary>
    public class BoolToCheckVisibleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null || (value is bool b && b) ? Visibility.Visible : (object)Visibility.Hidden;
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
