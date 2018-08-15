using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Toastify.Common;

namespace Toastify.Core
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ToastifyVolumeControlMode
    {
        /// <summary>
        /// The volume will be changed in the WindowsVolumeMixer and will affect system volume.
        /// </summary>
        [ComboBoxItem("Windows Volume Mixer (global volume)", "Use the Windows Volume Mixer.\nThis affects the global system volume.")]
        SystemGlobal = 1 << 1,

        /// <summary>
        /// The volume will be changed in the WindowsVolumeMixer and will affect just Spotify.
        /// </summary>
        [ComboBoxItem("Windows Volume Mixer (Spotify only)", "Use the Windows Volume Mixer.\nThis only affects Spotify's volume.")]
        SystemSpotifyOnly = SystemGlobal | 1,

        // The Spotify volume control mode has been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
        //[ComboBoxItem("Spotify", "Use Spotify's volume control.")]
        //Spotify = SystemGlobal | 1,
    }
}