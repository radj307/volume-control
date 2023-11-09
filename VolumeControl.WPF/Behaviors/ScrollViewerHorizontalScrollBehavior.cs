using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Scrolls horizontally when scrolling while holding SHIFT in <see cref="TextBoxBase"/> controls.
    /// </summary>
    public class ScrollViewerHorizontalScrollBehavior : Behavior<ScrollViewer>
    {
        #region MagnitudeProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="Magnitude"/>.
        /// </summary>
        public static readonly DependencyProperty MagnitudeProperty = DependencyProperty.Register(
            nameof(Magnitude),
            typeof(double),
            typeof(ScrollViewerHorizontalScrollBehavior),
            new PropertyMetadata(0.33));
        /// <summary>
        /// The magnitude modifier that controls the scroll distance. 1.0 is full strength, 0.0 is none.
        /// </summary>
        public double Magnitude
        {
            get => (double)GetValue(MagnitudeProperty);
            set => SetValue(MagnitudeProperty, value);
        }
        #endregion MagnitudeProperty

        #region Behavior Method Overrides
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
            WpfAddTiltScrollEventHook.AddPreviewMouseWheelHorizontalHandler(AssociatedObject, AssociatedObject_PreviewMouseWheelHorizontal);
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
            WpfAddTiltScrollEventHook.RemovePreviewMouseWheelHorizontalHandler(AssociatedObject, AssociatedObject_PreviewMouseWheelHorizontal);
        }
        #endregion Behavior Method Overrides

        #region EventHandlers
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - e.Delta * Magnitude);
                e.Handled = true;
            }
        }
        private void AssociatedObject_PreviewMouseWheelHorizontal(object sender, MouseWheelHorizontalEventArgs e)
        {
            AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - e.HorizontalDelta * Magnitude);
            e.Handled = true;
        }
        #endregion EventHandlers
    }
}
