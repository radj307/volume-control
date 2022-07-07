using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace VolumeControl.WPF.Markup
{
    /// <summary>
    /// XAML Extension that adds the ability to use <see cref="IValueConverter"/> implementations without binding to an actual source.
    /// </summary>
    public class ConvertExtension : MarkupExtension
    {
        /// <summary>Constructs a new instance of </summary>
        public ConvertExtension()
        {

        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => throw new NotImplementedException();
    }
}
