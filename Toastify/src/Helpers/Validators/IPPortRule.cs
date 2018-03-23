using System;
using System.Globalization;
using System.Windows.Controls;

// ReSharper disable BuiltInTypeReferenceStyle
namespace Toastify.Helpers.Validators
{
    public class IPPortRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return ValidationResult.ValidResult;

            if (UInt16.TryParse((string)value, out UInt16 port) && port > 0)
                return ValidationResult.ValidResult;

            return new ValidationResult(false, "Insert a valid IPv4 port number.");
        }
    }
}