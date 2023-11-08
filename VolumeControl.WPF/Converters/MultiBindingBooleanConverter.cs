using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// <see cref="IMultiValueConverter"/> for multiple boolean inputs that accepts and returns a list of booleans.<br/>
    /// Any non-boolean types are converted using <see cref="System.Convert.ToBoolean(object)"/>.
    /// </summary>
    public sealed class MultiBindingBooleanConverter : IMultiValueConverter
    {
        readonly TypeConverter _boolConverter = TypeDescriptor.GetConverter(typeof(bool));

        private bool ConvertToBool(object? obj)
        {
            try
            {
                return obj != null && System.Convert.ToBoolean(obj);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert value \"{obj}\" of type \"{obj?.GetType()}\" to bool!", ex);
            }
        }
        /// <inheritdoc/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter is null || System.Convert.ToBoolean(parameter))
                return values.All(ConvertToBool);
            else
                return values.Any(ConvertToBool);
        }
        /// <inheritdoc/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            int len = targetTypes.Length;

            object[] arr = new object[len];

            bool val = (bool)value;

            for (int i = 0; i < len; ++i)
            {
                arr[i] = val;
            }
            return arr;
        }
    }
}
