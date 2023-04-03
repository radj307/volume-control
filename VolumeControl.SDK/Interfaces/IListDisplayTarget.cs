using System.Windows.Controls;
using System.Windows.Media;
using VolumeControl.Core.Interfaces;

namespace VolumeControl.SDK.Interfaces
{
    /// <summary>
    /// Represents a <see cref="ListDisplayTarget"/>.
    /// </summary>
    public interface IListDisplayTarget
    {
        /// <summary>
        /// Gets or sets the background color of the notification window.
        /// </summary>
        Brush? Background { get; set; }
        /// <summary>
        /// Gets or sets whether the user can change the SelectedItem property of the ListView control by clicking on an item in the list.
        /// </summary>
        /// <remarks>
        /// While this <i>is</i> used by the Audio Sessions &amp; Audio Devices display targets, this property is <b>not the same thing</b>; this controls whether the actual ListView control allows the user to change the selection; it does not work the other way around!
        /// </remarks>
        bool LockSelection { get; set; }
        /// <summary>
        /// Gets or sets an optional icon to show in the display target list.
        /// </summary>
        ImageSource? Icon { get; set; }
        /// <summary>
        /// Gets or sets the enumerable list of objects that implement <see cref="IListDisplayable"/> to show in the notification window.
        /// </summary>
        IEnumerable<IListDisplayable>? ItemsSource { get; set; }
        /// <summary>
        /// Gets or sets the currently-selected item in the ListView control.
        /// </summary>
        IListDisplayable? SelectedItem { get; set; }
        /// <summary>
        /// Gets or sets an optional array of controls to display in the notification window when the individual controls are disabled.
        /// </summary>
        Control[]? SelectedItemControls { get; set; }
    }
}
