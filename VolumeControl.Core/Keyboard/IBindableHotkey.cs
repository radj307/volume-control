using Newtonsoft.Json;
using VolumeControl.Core.Keyboard.Actions;

namespace VolumeControl.Core.Keyboard
{
    /// <summary>
    /// Represents a <see cref="BindableHotkey"/>.
    /// </summary>
    [JsonObject]
    public interface IBindableHotkey : IHotkey
    {
        /// <summary>
        /// Gets or sets the name of this hotkey.
        /// </summary>
        [JsonProperty]
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the action associated with this hotkey.
        /// </summary>
        [JsonIgnore]
        IActionBinding? Action { get; set; }
    }
}
