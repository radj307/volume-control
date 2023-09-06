using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF
{
    /// <summary>
    /// <see cref="Behavior{T}"/> that enables the mouse wheel to increment/decrement the selected item in a <see cref="ListView"/>.
    /// </summary>
    public sealed class MouseWheelListViewBehavior : Behavior<ListView>
    {
        #region Methods
        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView listView = (ListView)sender;

            int count = listView.Items.Count;
            int index = listView.SelectedIndex;

            if (e.Delta < 0.0)
            {
                if (index >= count - 1)
                    index = 0;
                else index++;
            }
            else
            {
                if (index <= 0)
                    index = count - 1;
                else index--;
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
