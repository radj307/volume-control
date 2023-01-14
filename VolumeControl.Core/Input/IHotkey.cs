using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input.Actions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core.Input
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
        /// Contains any extra parameters required by the currently selected action.
        /// </summary>
        [JsonProperty]
        public ObservableImmutableList<IHotkeyActionSetting>? ActionSettings { get; set; }

        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        event HotkeyActionPressedEventHandler? Pressed;
    }
}
