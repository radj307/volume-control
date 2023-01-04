using Newtonsoft.Json;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Small container for hotkey action setting data
    /// </summary>
    [JsonObject]
    public class HotkeyActionSetting
    {
        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        [JsonProperty]
        public object? Value { get; set; }
    }
}
