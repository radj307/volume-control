using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// This converter removes specific rounded corners depending on the <see cref="Squared"/> property.<br/>
    /// Converts from <see cref="CornerRadius"/> to <see cref="CornerRadius"/>.
    /// </summary>
    [ValueConversion(typeof(CornerRadius), typeof(CornerRadius))]
    public class SquaredCornerRadiusConverter : IValueConverter
    {
        /// <summary>Determines which corners are set to radius 0.</summary>
        public Corner Squared { get; set; } = Corner.None;

        /// <returns><paramref name="value"/> with squared corners as specified by <see cref="Squared"/>.</returns>
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Corner corners = this.Squared;
            if (parameter is Corner c)
                corners = c;

            var cr = (CornerRadius)value;

            if (corners.HasFlag(Corner.TopLeft))
                cr.TopLeft = 0;
            if (corners.HasFlag(Corner.TopRight))
                cr.TopRight = 0;
            if (corners.HasFlag(Corner.BottomLeft))
                cr.BottomLeft = 0;
            if (corners.HasFlag(Corner.BottomRight))
                cr.BottomRight = 0;

            return cr;
        }
        /// <returns>The unmodified <paramref name="value"/> parameter.</returns>
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
