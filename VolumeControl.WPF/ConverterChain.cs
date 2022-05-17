using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace VolumeControl.WPF
{
    public class ConverterChain : List<IValueConverter>, IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in this)
            {
                if (item is IValueConverter converter)
                {
                    value = converter.Convert(value, targetType, parameter, culture);
                }
            }
            return value;
        }
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException(nameof(ConverterChain.ConvertBack));
    }
}
