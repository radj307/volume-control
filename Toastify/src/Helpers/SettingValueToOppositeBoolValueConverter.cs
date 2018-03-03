using System;
using System.Windows.Data;
using Toastify.Model;

namespace Toastify.Helpers
{
    public class SettingValueToOppositeBoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !((SettingValue<bool>)value)?.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SettingValue<bool>(!(bool)(value ?? false));
        }
    }
}