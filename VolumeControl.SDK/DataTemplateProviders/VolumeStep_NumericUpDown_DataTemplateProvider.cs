using System.Windows;
using VolumeControl.WPF.Controls;

namespace VolumeControl.SDK.DataTemplateProviders
{
    /// <summary>
    /// Ready-to-use <see cref="Core.DataTemplateProvider"/> that creates a <see cref="NumericUpDown"/> control with a MinValue of 1 and a MaxValue of 100.
    /// </summary>
    /// <remarks>
    /// To set your own min &amp; max values, override the <see cref="NumericUpDown_DataTemplateProvider"/> class.
    /// </remarks>
    public class VolumeStep_NumericUpDown_DataTemplateProvider : NumericUpDown_DataTemplateProvider
    {
        /// <inheritdoc/>
        protected override DataTemplate ProvideDataTemplate()
        {
            var numericUpDownFactory = GetNumericUpDownFactory();

            // Set Min/Max values
            numericUpDownFactory.SetValue(NumericUpDown.MinValueProperty, 1m);
            numericUpDownFactory.SetValue(NumericUpDown.MaxValueProperty, 100m);

            return new DataTemplate(typeof(int)) { VisualTree = numericUpDownFactory };
        }
    }
}
