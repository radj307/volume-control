using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Behavior that allows the mouse wheel to change the value of <see cref="Slider"/> controls.
    /// </summary>
    public class MouseWheelSliderBehavior : Behavior<Slider>
    {
        /// <summary>
        /// Gets or sets the amount to change the slider value by.
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        /// <summary>
        /// The amount to change the slider value by.
        /// </summary>
        public static readonly DependencyProperty AmountProperty
            = DependencyProperty.RegisterAttached(
                nameof(Amount), 
                typeof(double), 
                typeof(MouseWheelSliderBehavior), 
                new UIPropertyMetadata(0.0));

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Amount == 0.0) return;
            Slider slider = (Slider)sender;
            if (e.Delta > 0)
            {
                slider.Value += Amount;
            }
            else
            {
                slider.Value -= Amount;
            }
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
        }
    }
}
