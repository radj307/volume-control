using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using VolumeControl.Hotkeys;

namespace VolumeControl.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(IList<BindableWindowsHotkey>), typeof(bool?))]
    public class CheckBoxColumnHeaderConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<BindableWindowsHotkey> hotkeys)
            {
                var l = hotkeys.Select(item => item.Registered).ToArray();
                if (l.Length == 0)
                    return false;
                var fst = l.First();
                foreach (var item in l)
                {
                    if (item != fst)
                        return null;
                }
                return fst;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
