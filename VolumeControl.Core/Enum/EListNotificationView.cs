﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VolumeControl.Core.Enum
{
    /// <summary>
    /// Defines the viewing modes for a list notification window.
    /// </summary>
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ENotificationViewMode
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// The control bar that includes a mute checkbox, volume slider, and volume level textbox.
        /// </summary>
        ControlBar = 1, //< DO NOT RENAME THIS WITHOUT ALSO RENAMING IT IN ALL TRANSLATION CONFIGS
        /// <summary>
        /// Only the selected item(s).
        /// </summary>
        SelectedItemOnly = 2, //< DO NOT RENAME THIS WITHOUT ALSO RENAMING IT IN ALL TRANSLATION CONFIGS
        /// <summary>
        /// The full list of items.
        /// </summary>
        AllItems = 4, //< DO NOT RENAME THIS WITHOUT ALSO RENAMING IT IN ALL TRANSLATION CONFIGS
        /// <summary>
        /// Everything.
        /// </summary>
        Everything = ControlBar | AllItems,
    }
}
