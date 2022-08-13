using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <inheritdoc cref="IValueConverter"/>
    public class TagDoubleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0.0;
            Type? valueType = value.GetType();
            if (valueType == typeof(double))
                return (double)value;
            else if (valueType == typeof(float))
                return (float)value;
            return 0.0;
        }
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
