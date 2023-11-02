using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Behavior that prevent the mouse wheel from triggering events.
    /// </summary>
    public class DisableMouseWheelBehavior : Behavior<Control>
    {
        #region Behavior Method Overrides
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
        #endregion Behavior Method Overrides

        #region EventHandlers
        private void AssociatedObject_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            => e.Handled = true;
        #endregion EventHandlers
    }
}
