using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VolumeControl.WPF.Controls
{
    /// <summary>
    /// A <see cref="DataGrid"/> that ignores KeyDown events for <see cref="Key.Enter"/>, allowing other controls to receive them without the focused control changing.
    /// </summary>
    public class DataGridWithoutNavKeys : DataGrid
    {
        #region DisableEnterNavigationProperty
        /// <summary>
        /// The <see cref="DependencyProperty"/> for <see cref="DisableEnterNavigation"/>.
        /// </summary>
        public static readonly DependencyProperty DisableEnterNavigationProperty = DependencyProperty.Register(
            nameof(DisableEnterNavigation),
            typeof(bool),
            typeof(DataGridWithoutNavKeys),
            new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether up/down navigation with <see cref="Key.Enter"/> is disabled or not.
        /// </summary>
        /// <returns><see langword="true"/> when Enter key navigation is disabled; otherwise <see langword="false"/>.</returns>
        public bool DisableEnterNavigation
        {
            get => (bool)GetValue(DisableEnterNavigationProperty);
            set => SetValue(DisableEnterNavigationProperty, value);
        }
        #endregion DisableEnterNavigationProperty

        #region Method Overrides
        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (DisableEnterNavigation)
            { // prevent enter key from triggering up/down navigation
                switch (e.Key)
                {
                case Key.Enter:
                    switch (e.KeyboardDevice.Modifiers)
                    {
                    case ModifierKeys.Shift: //< up navigation
                    case ModifierKeys.None: //< down navigation
                        return; //< prevent keypress from being received by this control
                    }
                    break;
                }
            }

            base.OnKeyDown(e);
        }
        #endregion Method Overrides
    }
}
