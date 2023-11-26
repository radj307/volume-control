using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts from an enum value to <see cref="bool"/> depending on the flag value specified as a converter parameter.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumHasFlagConverter : IValueConverter
    {
        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="bool"/> value returned by Convert when the input value is <see langword="null"/>.
        /// </summary>
        public bool ConvertResultWhenNull { get; set; } = false;
        #endregion Properties

        #region IValueConverter
        /// <summary>
        /// Converts from an enum flag <paramref name="value"/> to <see cref="bool"/>.
        /// </summary>
        /// <remarks>
        /// When <paramref name="value"/> is <see langword="null"/>, the value of the ConvertResultWhenNull property is returned instead.
        /// </remarks>
        /// <returns><see langword="true"/> when <paramref name="value"/> has the <paramref name="parameter"/> flag set; otherwise, <see langword="false"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Provide the enum flag value(s) to check for as the ConverterParameter
            ArgumentNullException.ThrowIfNull(parameter);

            if (value == null) return ConvertResultWhenNull;

            var e_value = System.Convert.ToInt64(value);
            var flag = System.Convert.ToInt64(parameter);

            return (e_value & flag) != 0;
        }
        /// <summary>
        /// Converts from <see cref="bool"/> to the specified <paramref name="targetType"/> enum.
        /// </summary>
        /// <returns>The <paramref name="parameter"/> value as the specified <paramref name="targetType"/>.</returns>
        /// <exception cref="NotImplementedException"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(parameter);

            long result = 0;
            if ((bool)value)
                result |= System.Convert.ToInt64(parameter);

            return Enum.ToObject(targetType, result);
        }
        #endregion IValueConverter
    }
}
