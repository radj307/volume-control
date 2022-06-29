using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;

namespace VolumeControl.Core.Keyboard
{
    /// <summary>
    /// Represents a hotkey instance.
    /// </summary>
    [JsonObject]
    public interface IHotkey : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets this hotkey's unique ID number.
        /// </summary>
        [JsonIgnore]
        int ID { get; }
        /// <summary>
        /// Gets or sets the key associated with this hotkey.
        /// </summary>
        [JsonProperty]
        Key Key { get; set; }
        /// <summary>
        /// Gets or sets the modifier keys associated with this hotkey.
        /// </summary>
        [JsonProperty]
        Modifier Modifier { get; set; }
        /// <summary>
        /// Gets or sets whether this hotkey is registered (active).
        /// </summary>
        [JsonProperty]
        bool Registered { get; set; }

        /// <summary>
        /// Triggered when the hotkey is pressed.
        /// </summary>
        event HandledEventHandler? Pressed;
    }
}
