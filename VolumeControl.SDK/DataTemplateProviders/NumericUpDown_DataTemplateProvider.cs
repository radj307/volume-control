using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Data;
using VolumeControl.Core;
using VolumeControl.WPF.Behaviors;
using VolumeControl.WPF.Controls;

namespace VolumeControl.SDK.DataTemplateProviders
{
    /// <summary>
    /// Abstract <see cref="DataTemplateProvider"/> that implements <see cref="GetNumericUpDownFactory"/>.
    /// </summary>
    public abstract class NumericUpDown_DataTemplateProvider : DataTemplateProvider
    {
        /// <summary>
        /// Gets a new <see cref="FrameworkElementFactory"/> instance of a <see cref="NumericUpDown"/> control.
        /// </summary>
        /// <returns>A new <see cref="FrameworkElementFactory"/> instance with a <see cref="NumericUpDown"/> control.</returns>
        protected virtual FrameworkElementFactory GetNumericUpDownFactory()
        {
            var numericUpDownFactory = new FrameworkElementFactory(typeof(NumericUpDown));

            // Set appearance-related values
            numericUpDownFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            numericUpDownFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 1, 3, 1));

            // Bind NumericUpDown.Value => IActionSettingInstance.Value
            numericUpDownFactory.SetBinding(NumericUpDown.ValueProperty, new Binding(nameof(Core.Input.Actions.Settings.IActionSettingInstance.Value)));

            // Attach behaviors through the Loaded event
            numericUpDownFactory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((sender, e) =>
            {
                Interaction.GetBehaviors((NumericUpDown)sender).Add(new MouseWheelNumericUpDownBehavior());
            }));

            return numericUpDownFactory;
        }
    }
}
