using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.CoreAudio.Interfaces;

namespace VolumeControl.Helpers
{
    internal static class IAudioControlExtensions
    {
        /// <summary>
        /// Creates a new array of controls with a mute checkbox, a volume slider, and a volume text box, all data-bound to the given <paramref name="inst"/>.
        /// </summary>
        /// <param name="inst">An object instance that implements <see cref="IAudioControl"/>.</param>
        /// <param name="sliderWidth">The value to use for the <see cref="System.Windows.FrameworkElement.MinWidth"/> property on the volume slider.</param>
        /// <returns>An array of <see cref="Control"/> instances, data-bound to the given <paramref name="inst"/>.</returns>
        [STAThread]
        internal static Control[] MakeListDisplayableControlTemplate(this IAudioControl inst, int sliderWidth = 100)
        {
            List<Control> l = new();

            // Create the mute checkbox
            CheckBox muteCheckbox = new()
            {
                Margin = new(3, 1, 3, 1)
            };
            _ = muteCheckbox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding(nameof(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty))
            { // Bind IsChecked to the instance's 'Muted' property
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IAudioControl.Mute)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            l.Add(muteCheckbox); //< add the mute state checkbox

            // Create the volume slider
            Slider volSlider = new()
            {
                MinWidth = sliderWidth,
                Minimum = 0.0,
                Maximum = 100.0,
                Margin = new(3, 1, 3, 1)
            };
            _ = volSlider.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty, new Binding(nameof(System.Windows.Controls.Primitives.RangeBase.ValueProperty))
            {
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IAudioControl.Volume)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            l.Add(volSlider); //< add the volume slider

            // Create the volume textbox
            TextBox volTextBox = new()
            {
                MinWidth = 35,
                Margin = new(3, 1, 3, 1),
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                BorderThickness = new(0, 0, 0, 0)
            };
            _ = volTextBox.SetBinding(TextBox.TextProperty, new Binding(nameof(TextBox.TextProperty))
            {
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IAudioControl.Volume)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            });
            l.Add(volTextBox); //< add the volume textbox

            return l.ToArray();
        }
    }
}
