using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    public class HorizontalScrollBehavior : Behavior<TextBoxBase>
    {
        public static readonly DependencyProperty MagnitudeProperty = DependencyProperty.Register(
            nameof(Magnitude),
            typeof(double),
            typeof(HorizontalScrollBehavior),
            new PropertyMetadata(0.33));
        /// <summary>
        /// The magnitude modifier that controls the scroll distance. 1.0 is full strength, 0.0 is none.
        /// </summary>
        public double Magnitude
        {
            get => (double)GetValue(MagnitudeProperty);
            set => SetValue(MagnitudeProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
        }

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset - e.Delta * Magnitude);
                e.Handled = true;
            }
        }
    }
}
