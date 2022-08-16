using Newtonsoft.Json;
using VolumeControl.Core.Input.Actions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Represents a <see cref="BindableHotkey"/>.
    /// </summary>
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
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Alt"/> bit in the <see cref="IHotkey.Modifier"/> property.
        /// </summary>
        [JsonIgnore]
        bool Alt { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Ctrl"/> bit in the <see cref="IHotkey.Modifier"/> property.
        /// </summary>
        [JsonIgnore]
        bool Ctrl { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Shift"/> bit in the <see cref="IHotkey.Modifier"/> property.
        /// </summary>
        [JsonIgnore]
        bool Shift { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.Super"/> bit in the <see cref="IHotkey.Modifier"/> property.
        /// </summary>
        [JsonIgnore]
        bool Win { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Modifier.NoRepeat"/> bit in the <see cref="IHotkey.Modifier"/> property.
        /// </summary>
        [JsonIgnore]
        bool NoRepeat { get; set; }
    }
}
