using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts from any object type to bool depending on whether or not that object is null.
    /// </summary>
    /// <remarks>Returns true when the object is null.</remarks>
    [ValueConversion(typeof(Nullable), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value == null;
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value == null;
    }
}
