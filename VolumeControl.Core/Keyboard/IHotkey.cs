using System.ComponentModel;
using System.Windows.Input;

namespace VolumeControl.Core.Keyboard
{
    /// <summary>
    /// Represents a hotkey instance.
    /// </summary>
    public interface IHotkey : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets this hotkey's unique ID number.
        /// </summary>
        int ID { get; }
        /// <summary>
        /// Gets or sets the key associated with this hotkey.
        /// </summary>
        Key Key { get; set; }
        /// <summary>
        /// Gets or sets the modifier keys associated with this hotkey.
        /// </summary>
        Modifier Modifier { get; set; }
        /// <summary>
        /// Gets or sets whether this hotkey is registered (active).
        /// </summary>
        bool Registered { get; set; }

        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        event HandledEventHandler? Pressed;
    }
}
