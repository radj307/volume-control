using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using VolumeControl.Core;
using VolumeControl.Core.Interfaces;
using XamlTimers;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents some kind of controllable audio playback instance.
    /// </summary>
    /// <remarks>
    /// This interface 'derives' from <see cref="IListDisplayable"/>.
    /// </remarks>
    public interface IVolumeControl : IListDisplayable
    {
        /// <summary>
        /// Gets the current peak meter level.
        /// </summary>
        float PeakMeterValue { get; }
        /// <summary>
        /// Gets or sets the volume of this <see cref="IAudioControl"/> instance as a <see cref="float"/> in the range <b>( 0.0 <i>(0%)</i> - 1.0 <i>(100%)</i> )</b><br/>
        /// This is the native volume range of this instance, into which the <see cref="Volume"/> property's value is scaled.
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        float NativeVolume { get; set; }
        /// <summary>
        /// Gets or sets the volume of this <see cref="IAudioControl"/> instance as an <see cref="int"/> in the range <b>( 0 - 100 )</b>
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        int Volume { get; set; }
        /// <summary>
        /// Gets or sets this <see cref="IAudioControl"/> instance's current mute-state.
        /// </summary>
        /// <returns><see langword="true"/> when this instance is muted; otherwise <see langword="false"/>.</returns>
        bool Muted { get; set; }

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
            _ = volSlider.SetBinding(Slider.TagProperty, new Binding(nameof(Slider.TagProperty))
            { // Bind the slider's tag property to the peak meter value
                Source = inst,
                Path = new System.Windows.PropertyPath(nameof(IVolumeControl.PeakMeterValue)),
                Mode = BindingMode.OneWay
            });
            // Create a XamlTimer for the volSlider's peak meter
            IntervalUpdateBinding volPeakMeterUpdater = new()
            {
                Property = Slider.TagProperty
            };
            _ = BindingOperations.SetBinding(volPeakMeterUpdater, IntervalUpdateBinding.EnableTimerProperty, new Binding(nameof(IntervalUpdateBinding.EnableTimerProperty))
            { // Bind the EnableTimerProperty to Config.ShowPeakMeters
                Source = (Config.Default as Config)!,
                Path = new System.Windows.PropertyPath(nameof(Config.ShowPeakMeters)),
                Mode = BindingMode.OneWay,
            });
            _ = BindingOperations.SetBinding(volPeakMeterUpdater, IntervalUpdateBinding.IntervalProperty, new Binding(nameof(IntervalUpdateBinding.IntervalProperty))
            { // Bind the IntervalProperty to Config.PeakMeterUpdateIntervalMs
                Source = (Config.Default as Config)!,
                Path = new System.Windows.PropertyPath(nameof(Config.PeakMeterUpdateIntervalMs)),
                Mode = BindingMode.OneWay
            });
            // Add the XamlTimer behavior to the volSlider
            Microsoft.Xaml.Behaviors.Interaction.GetBehaviors(volSlider).Add(volPeakMeterUpdater);
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
    }
}
