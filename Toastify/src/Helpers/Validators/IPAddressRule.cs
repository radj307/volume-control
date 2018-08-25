using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Toastify.Helpers.Validators
{
    public class IPAddressRule : ValidationRule
    {
        #region Static Fields and Properties

        public static readonly Regex IPRegex = new Regex(@"^[0-9]{1,3}(\.[0-9]{1,3}){3}$", RegexOptions.Compiled);

        #endregion

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return ValidationResult.ValidResult;

            string ip = (string)value;
            if (IPRegex.IsMatch(ip))
            {
                string[] parts = ip.Split('.');
                foreach (string part in parts)
                {
                    int n = int.Parse(part);
                    if (n > 255)
                        return new ValidationResult(false, $"\"{part}\" is not a valid number for an IPv4 address.");
                }

                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Insert a valid IPv4 address.");
        }
    }
}