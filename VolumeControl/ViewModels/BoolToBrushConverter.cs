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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b) return WhenTrue;
                else return WhenFalse;
            }
            else return WhenNull;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush b)
            {
                if (b.Equals(WhenTrue)) return true;
                else if (b.Equals(WhenFalse)) return false;
                else if (b.Equals(WhenNull)) return null!;
                else throw new ArgumentOutOfRangeException(nameof(value), value, $"Unexpected value of type {value.GetType().FullName} cannot be converted back to type {typeof(bool).Name} because it is not equal to either brush instance!");
            }
            else return null!;
        }
        #endregion ValueConverter
    }
}
