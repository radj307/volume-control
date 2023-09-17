using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts from an arbitrary list of items to a boolean value that depends on whether the list is empty or not.<br/>
    /// When the list is empty, returns <see langword="false"/>; when the list has at least one item, returns <see langword="true"/>.
    /// </summary>
    [ValueConversion(typeof(IList), typeof(bool))]
    public class ListHasItemsBooleanConverter : IValueConverter
    {
        /// <summary>
        /// The default return value when the input type is invalid. (not a list)
        /// </summary>
        public bool ResultOnInvalidInputType { get; set; } = false;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList list)
            {
                return list.Count > 0;
            }
            else if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    return true;
                }
                return false;
            }
            else return ResultOnInvalidInputType;
        }
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new InvalidOperationException($"{typeof(ListHasItemsBooleanConverter).FullName}: Cannot convert back to a list!");
    }
}
