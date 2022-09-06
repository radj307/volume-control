using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VolumeControl.ViewModels
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToBrushConverter : DependencyObject, IValueConverter, INotifyPropertyChanged
    {
        #region WhenTrue
        public static readonly DependencyProperty WhenTrueProperty = DependencyProperty.Register(nameof(WhenTrue), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        public Brush WhenTrue
        {
            get => (this.GetValue(WhenTrueProperty) as Brush)!;
            set => this.SetValue(WhenTrueProperty, value);
        }
        #endregion WhenTrue

        #region WhenFalse
        public static readonly DependencyProperty WhenFalseProperty = DependencyProperty.Register(nameof(WhenFalse), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        public Brush WhenFalse
        {
            get => (this.GetValue(WhenFalseProperty) as Brush)!;
            set => this.SetValue(WhenFalseProperty, value);
        }
        #endregion WhenFalse

        #region WhenNull
        public static readonly DependencyProperty WhenNullProperty = DependencyProperty.Register(nameof(WhenNull), typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(new SolidColorBrush()));
        public Brush WhenNull
        {
            get => (this.GetValue(WhenNullProperty) as Brush)!;
            set => this.SetValue(WhenNullProperty, value);
        }
        #endregion WhenNull

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region ValueConverter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool b ? b ? this.WhenTrue : (object)this.WhenFalse : this.WhenNull;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush b)
            {
                return b.Equals(this.WhenTrue)
                    ? true
                    : b.Equals(this.WhenFalse)
                    ? false
                    : b.Equals(this.WhenNull)
                    ? null
                    : throw new ArgumentOutOfRangeException(nameof(value), value, $"Unexpected value of type {value.GetType().FullName} cannot be converted back to type {typeof(bool).Name} because it is not equal to either brush instance!");
            }
            else
            {
                return null!;
            }
        }
        #endregion ValueConverter
    }
}
