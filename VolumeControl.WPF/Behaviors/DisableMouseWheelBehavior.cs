using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// Behavior that prevent the mouse wheel from triggering events.
    /// </summary>
    public class DisableMouseWheelBehavior : Behavior<Control>
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
            if (!AllowScrollingInComboBoxDropDown)
            {
                e.Handled = true;
                return;
            }
            if (sender is ComboBox comboBox)
            {
                if (!comboBox.IsDropDownOpen)
                    e.Handled = true;
            }
            else e.Handled = true;
        }
        #endregion EventHandlers
    }
}
