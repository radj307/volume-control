using Newtonsoft.Json;
using System.Windows.Input;
using VolumeControl.Core.Enum;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Acts as a temporary wrapper for the <see cref="BindableHotkey"/> class so that the JSON parser can read and write it without attempting to register hotkeys before the API is initialized.
    /// </summary>
    [JsonObject]
    public struct BindableHotkeyJsonWrapper
    {
        /// <summary>
        /// Creates a new <see cref="BindableHotkeyJsonWrapper"/> instance.
        /// </summary>
        public BindableHotkeyJsonWrapper() { }

        /// <summary>
        /// The hotkey's name
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Whether the hotkey is registered or not.
        /// </summary>
        [JsonProperty]
        public bool Registered { get; set; } = false;
        /// <summary>
        /// The primary key
        /// </summary>
        [JsonProperty]
        public Key Key { get; set; } = Key.None;
        /// <summary>
        /// The modifier keys
        /// </summary>
        [JsonProperty]
        public Modifier Modifier { get; set; } = Modifier.None;
        /// <summary>
        /// The name of the action
        /// </summary>
        [JsonProperty]
        public string? ActionIdentifier { get; set; } = null;
    }
}
