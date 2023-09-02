using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;
using VolumeControl.WPF.Controls;

namespace VolumeControl.WPF
{
    /// <summary>
    /// <see cref="Behavior{T}"/> that enables the mouse wheel to change the value of <see cref="NumericUpDown"/> controls.
    /// </summary>
    public sealed class MouseWheelNumericUpDownBehavior : Behavior<NumericUpDown>
    {
        #region AmountProperty
        /// <summary>
        /// The amount to change the slider value by.
        /// </summary>
        public static readonly DependencyProperty AmountProperty
            = DependencyProperty.RegisterAttached(
                nameof(Amount),
                typeof(decimal),
                typeof(MouseWheelNumericUpDownBehavior),
                new UIPropertyMetadata(1m));
        /// <summary>
        /// Gets or sets the amount to change the slider value by.
        /// </summary>
        public decimal Amount
        {
            get => (decimal)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
        #endregion AmountProperty

        #region Methods
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Amount == 0m) return;
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            if (e.Delta > 0)
            {
                numericUpDown.Value += Amount;
            }
            else
            {
                numericUpDown.Value -= Amount;
            }
        }
        #endregion Methods

        #region Method Overrides
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
        #endregion Method Overrides
    }
}
