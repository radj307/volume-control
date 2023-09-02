using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VolumeControl.WPF
{
    /// <summary>
    /// <see cref="Behavior{T}"/> that enables the mouse wheel to change the value of controls based on <see cref="RangeBase"/>.
    /// </summary>
    public sealed class MouseWheelRangeBaseBehavior : Behavior<RangeBase>
    {
        #region AmountProperty
        /// <summary>
        /// The amount to change the slider value by.
        /// </summary>
        public static readonly DependencyProperty AmountProperty
            = DependencyProperty.RegisterAttached(
                nameof(Amount),
                typeof(double),
                typeof(MouseWheelRangeBaseBehavior),
                new UIPropertyMetadata(1.0));
        /// <summary>
        /// Gets or sets the amount to change the slider value by.
        /// </summary>
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }
        #endregion AmountProperty

        #region Methods
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Amount == 0.0) return;
            RangeBase rangeBase = (RangeBase)sender;
            if (e.Delta > 0)
            {
                rangeBase.Value += Amount;
            }
            else
            {
                rangeBase.Value -= Amount;
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
