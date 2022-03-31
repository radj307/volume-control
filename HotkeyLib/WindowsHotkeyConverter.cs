﻿using System.ComponentModel;
using System.Globalization;

namespace HotkeyLib
{
    public class WindowsHotkeyConverter : TypeConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value != null && destinationType == typeof(string))
                return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) => (value as WindowsHotkey) ?? base.ConvertFrom(context, culture, value);
    }
}