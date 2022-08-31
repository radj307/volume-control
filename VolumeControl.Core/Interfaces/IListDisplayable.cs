using System.Windows.Controls;
using System.Windows.Media;

namespace VolumeControl.Core.Interfaces
{
    /// <summary>
    /// Represents an object that, when it is a member of a list of similar items, can be displayed in the ListNotification window.
    /// </summary>
    public interface IListDisplayable
    {
        /// <summary>
        /// Text to display on this list item.
        /// </summary>
        string DisplayText { get; set; }
        /// <summary>
        /// Optional list of controls to display on this list item.
        /// </summary>
        Control[]? DisplayControls { get; }
        /// <summary>
        /// Icon to display on this list item.
        /// </summary>
        ImageSource? DisplayIcon { get; }
    }
}
