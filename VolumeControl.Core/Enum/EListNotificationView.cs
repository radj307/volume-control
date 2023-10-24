using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VolumeControl.Core.Enum
{
    /// <summary>
    /// Defines the viewing modes for a list notification window.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EListNotificationView
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// Control bar that shows a mute checkbox, volume slider, and volume textbox.
        /// </summary>
        ControlBar = 1,
        /// <summary>
        /// The selected item(s) in the list only.
        /// </summary>
        SelectedItemOnly = 2,
        /// <summary>
        /// All items in the list, including the selected item(s).
        /// </summary>
        AllItems = 4,
        /// <summary>
        /// Everything
        /// </summary>
        Everything = ControlBar | AllItems,
    }
}
