using Newtonsoft.Json;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Represents a setting for a hotkey action.
    /// </summary>
    public interface IHotkeyActionSetting
    {
        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        [JsonIgnore]
        string Label { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ValueType"/> of <see cref="Value"/> accepted by this setting.<br/>
        /// Setting this to <see langword="null"/> will allow <see cref="Value"/> to be set to any type.
        /// </summary>
        [JsonIgnore]
        Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        object? Value { get; set; }
        /// <summary>
        /// Gets or sets the description of this setting, which is shown in a tooltip in the action settings window.
        /// </summary>
        [JsonIgnore]
        string? Description { get; set; }
    }
}
