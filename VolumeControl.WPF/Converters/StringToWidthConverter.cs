using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts a given string to width.
    /// </summary>
    [ValueConversion(typeof(double), typeof(double))]
    public class StringToWidthConverter : IValueConverter
    {
        private Typeface? _typeface = null;

        public string String { get; set; } = string.Empty;
        public CultureInfo CultureInfo { get; set; } = CultureInfo.CurrentCulture;
        public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;
        public FontFamily FontFamily { get; set; } = new("Segoe UI");
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public FontWeight FontWeight { get; set; } = FontWeights.Regular;
        public FontStretch FontStretch { get; set; } = FontStretches.Medium;
        public double FontSize { get; set; } = 12;
        public double DIP { get; set; } = 1.0;
        internal Typeface Typeface => _typeface ??= new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double v)
            {
                FormattedText t = new FormattedText(String, CultureInfo, FlowDirection, Typeface, FontSize, new SolidColorBrush(), DIP);
                return t.WidthIncludingTrailingWhitespace;
            }
            else throw new ArgumentException($"{nameof(StringToWidthConverter)} expects type '{typeof(double).FullName}', received '{value.GetType().FullName}'");
        }
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
