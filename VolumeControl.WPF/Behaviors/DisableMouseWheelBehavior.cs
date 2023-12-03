using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Behavior that prevent the mouse wheel from triggering events.
    /// </summary>
    public class DisableMouseWheelBehavior : Behavior<UIElement>
    {
        #region Properties
        /// <summary>
        /// Gets or sets whether the mouse scroll event is handled even when the associated object is
        ///  a combo box with its dropdown open.
        /// </summary>
        public bool AllowScrollingInComboBoxDropDown { get; set; } = true;
        #endregion Properties

        #region Behavior Method Overrides
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
        }
        #endregion Behavior Method Overrides

        #region EventHandlers
        private void AssociatedObject_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (AllowScrollingInComboBoxDropDown
                && sender is ComboBox comboBox
                && comboBox.IsDropDownOpen)
                return; //< don't prevent scrolling in combo box dropdown

            // prevent this element from receiving the scroll event
            e.Handled = true;

            // allow parent to receive the event
            var e2 = new System.Windows.Input.MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };
            AssociatedObject.RaiseEvent(e2);
        }
        #endregion EventHandlers
    }
}
