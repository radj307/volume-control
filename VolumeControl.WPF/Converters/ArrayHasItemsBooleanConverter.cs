using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts from an arbitrary array of items to a boolean value that depends on whether the array is empty or not.<br/>
    /// When the array is empty, returns <see langword="false"/>; when the array has at least one item, returns <see langword="true"/>.
    /// </summary>
    public class ArrayHasItemsBooleanConverter : IValueConverter
    {
        /// <summary>
        /// The default return value when the input type is invalid. (not a list)
        /// </summary>
        public bool ResultOnInvalidInputType { get; set; } = false;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Array array)
            {
                return array.Length > 0;
            }
            else return ResultOnInvalidInputType;
        }
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
