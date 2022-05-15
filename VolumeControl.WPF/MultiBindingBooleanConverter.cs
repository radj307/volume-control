using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace VolumeControl.WPF
{
    public class MultiBindingBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(i => System.Convert.ToBoolean(i));
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            int len = targetTypes.Length;

            object[] arr = new object[len];

            bool val = System.Convert.ToBoolean(value);

            for (int i = 0; i < len; ++i)
            {
                arr[i] = val;
            }
            return arr;
        }
    }
}
