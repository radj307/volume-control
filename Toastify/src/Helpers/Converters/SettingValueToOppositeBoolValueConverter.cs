using System;
using System.Windows.Data;
using Toastify.Model;

namespace Toastify.Helpers.Converters
{
    public class SettingValueToOppositeBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return !((SettingValue<bool>)value)?.Value;
            }
            catch
            {
                dynamic dynamicValue = value;
                return !dynamicValue?.Value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SettingValue<bool>(!(bool)(value ?? false));
        }
    }
}