using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.Core.Interfaces;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents some kind of controllable audio playback instance.
    /// </summary>
    public interface IVolumeControl : IListDisplayable
    {
        /// <summary>
        /// Gets or sets the volume of this <see cref="IMeteredVolumeControl"/> instance as a <see cref="float"/> in the range <b>( 0.0 <i>(0%)</i> - 1.0 <i>(100%)</i> )</b><br/>
        /// This is the native volume range of this instance, into which the <see cref="Volume"/> property's value is scaled.
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        float NativeVolume { get; set; }
        /// <summary>
        /// Gets or sets the volume of this <see cref="IMeteredVolumeControl"/> instance as an <see cref="int"/> in the range <b>( 0 - 100 )</b>
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        int Volume { get; set; }
        /// <summary>
        /// Gets or sets this <see cref="IMeteredVolumeControl"/> instance's current mute-state.
        /// </summary>
        /// <returns><see langword="true"/> when this instance is muted; otherwise <see langword="false"/>.</returns>
        bool Muted { get; set; }

        #region Methods
        /// <summary>
        /// Increases <see cref="NativeVolume"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to increase the volume by.<br/>
        /// If the resulting volume exceeds the maximum boundary, it is automatically clamped.</param>
        /// <returns>The new value of the <see cref="NativeVolume"/> property.</returns>
        float IncreaseVolume(float amount);
        /// <summary>
        /// Increases <see cref="Volume"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to increase the volume by.<br/>
        /// If the resulting volume exceeds the maximum boundary, it is automatically clamped.</param>
        /// <returns>The new value of the <see cref="Volume"/> property.</returns>
        int IncreaseVolume(int amount);
        /// <summary>
        /// Decreases <see cref="NativeVolume"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.<br/>
        /// If the resulting volume exceeds the minimum boundary, it is automatically clamped.</param>
        /// <returns>The new value of the <see cref="NativeVolume"/> property.</returns>
        float DecreaseVolume(float amount);
        /// <summary>
        /// Decreases <see cref="Volume"/> by <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.<br/>
        /// If the resulting volume exceeds the minimum boundary, it is automatically clamped.</param>
        /// <returns>The new value of the <see cref="Volume"/> property.</returns>
        int DecreaseVolume(int amount);
        #endregion Methods

        #region Functions
        /// <summary>
        /// Creates a new array of controls with a mute checkbox, a volume slider, and a volume text box, all data-bound to the given <paramref name="inst"/>.
        /// </summary>
        /// <param name="inst">An object instance that implements <see cref="IVolumeControl"/>.</param>
        /// <param name="sliderWidth">The value to use for the <see cref="System.Windows.FrameworkElement.MinWidth"/> property on the volume slider.</param>
        /// <returns>An array of <see cref="Control"/> instances, data-bound to the given <paramref name="inst"/>.</returns>
        [STAThread]
        internal static Control[] MakeListDisplayableControlTemplate(IVolumeControl inst, int sliderWidth = 100)
        {
            List<Control> l = new();

            // Create the mute checkbox
            CheckBox muteCheckbox = new()
            {
                Margin = new(3, 1, 3, 1)
            };
            _ = muteCheckbox.SetBinding(CheckBox.IsCheckedProperty, new Binding(nameof(CheckBox.IsCheckedProperty))
            { // Bind IsChecked to the instance's 'Muted' property
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IVolumeControl.Muted)),
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
            _ = volSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(Slider.ValueProperty))
            {
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IVolumeControl.Volume)),
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
                Path = new System.Windows.PropertyPath(nameof(IVolumeControl.Volume)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.LostFocus
            });
            l.Add(volTextBox); //< add the volume textbox

            return l.ToArray();
        }
        #endregion Functions
    }
}
