using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF.Behaviors
{
    /// <summary>
    /// <see cref="Behavior{T}"/> that enables the mouse wheel to increment/decrement the selected item in a <see cref="ListView"/>.
    /// </summary>
    public sealed class MouseWheelListViewBehavior : Behavior<ListView>
    {
        #region Methods
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var listView = (ListView)sender;

            int index = listView.SelectedIndex;
            int maxIndex = listView.Items.Count - 1;

            if (e.Delta < 0.0)
            { // scroll up
                ++index;

                if (index > maxIndex)
                    index = 0;
            }
            else
            { // scroll down
                --index;

                if (index < 0)
                    index = maxIndex;
            }

            listView.SelectedIndex = index;
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
