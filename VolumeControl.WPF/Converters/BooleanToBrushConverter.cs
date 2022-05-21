using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace VolumeControl.WPF.Converters
{
    /// <summary>
    /// Use as the base class for BoolToXXX style converters
    /// </summary>
    /// <remarks>From <see href="https://stackoverflow.com/a/17290508/8705305"/></remarks>
    /// <typeparam name="T"></typeparam>    
    public abstract class BoolToValueConverter<T> : MarkupExtension, IValueConverter
    {
        #region Constructors and Destructors

        protected BoolToValueConverter()
        {
            TrueValue = default!;
            FalseValue = default!;
        }

        #endregion

        #region Public Properties

        public T FalseValue { get; set; }

        public T TrueValue { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc/>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture) => (System.Convert.ToBoolean(value) ? TrueValue : FalseValue)!;

        // Override if necessary
        /// <inheritdoc/>
        public virtual object ConvertBack(object value, Type targetType,
                                          object parameter, CultureInfo culture) => value.Equals(TrueValue);

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        #endregion
    }
}
