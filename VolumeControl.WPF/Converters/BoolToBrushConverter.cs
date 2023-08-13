using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Converts between <see cref="bool"/> and given <see cref="Brush"/> instances.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : DependencyObject, IValueConverter
    {
        #region WhenTrue
        /// <summary>
        /// The <see cref="Brush"/> returned when the value is <see langword="true"/>.
        /// </summary>
        public static readonly DependencyProperty WhenTrueProperty = DependencyProperty.Register(nameof(WhenTrue), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        /// <summary>
        /// Gets or sets the <see cref="Brush"/> returned when the value is <see langword="true"/>.
        /// </summary>
        public Brush WhenTrue
        {
            get => (this.GetValue(WhenTrueProperty) as Brush)!;
            set => this.SetValue(WhenTrueProperty, value);
        }
        #endregion WhenTrue

        #region WhenFalse
        /// <summary>
        /// The <see cref="Brush"/> returned when the value is <see langword="false"/>.
        /// </summary>
        public static readonly DependencyProperty WhenFalseProperty = DependencyProperty.Register(nameof(WhenFalse), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        /// <summary>
        /// Gets or sets the <see cref="Brush"/> returned when the value is <see langword="false"/>.
        /// </summary>
        public Brush WhenFalse
        {
            get => (this.GetValue(WhenFalseProperty) as Brush)!;
            set => this.SetValue(WhenFalseProperty, value);
        }
        #endregion WhenFalse

        #region WhenNull
        /// <summary>
        /// The <see cref="Brush"/> returned when the value is <see langword="null"/>.
        /// </summary>
        public static readonly DependencyProperty WhenNullProperty = DependencyProperty.Register(nameof(WhenNull), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        /// <summary>
        /// Gets or sets the <see cref="Brush"/> returned when the value is <see langword="null"/>.
        /// </summary>
        public Brush WhenNull
        {
            get => (this.GetValue(WhenNullProperty) as Brush)!;
            set => this.SetValue(WhenNullProperty, value);
        }
        #endregion WhenNull

        #region ValueConverter
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool b ? b ? this.WhenTrue : (object)this.WhenFalse : this.WhenNull;
        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush b)
            {
                return (b.Equals(this.WhenTrue)
                    ? true
                    : b.Equals(this.WhenFalse)
                    ? false
                    : b.Equals(this.WhenNull)
                    ? null
                    : throw new ArgumentOutOfRangeException(nameof(value), value, $"Unexpected value of type {value.GetType().FullName} cannot be converted back to type {typeof(bool).Name} because it is not equal to either brush instance!"))!;
            }
            else
            {
                return null!;
            }
        }
        #endregion ValueConverter
    }
}
