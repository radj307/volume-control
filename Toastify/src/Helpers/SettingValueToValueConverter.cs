using System;
using System.Windows.Data;
using Toastify.Model;

namespace Toastify.Helpers
{
    public class SettingValueToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((ISettingValue)value)?.GetValue();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var genericType = typeof(SettingValue<>).MakeGenericType(targetType.GenericTypeArguments[0]);
            var converted = (ISettingValue)Activator.CreateInstance(genericType);
            converted.SetValue(value);
            return converted;
        }
    }
}