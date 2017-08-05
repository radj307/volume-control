using System;
using Toastify.Common;

namespace Toastify.Core
{
    [Flags]
    public enum ToastifyVolumeControlMode
    {
        /// <summary>
        /// The volume will changed only inside Spotify.
        /// </summary>
        [ComboBoxItem("Spotify", "Use Spotify's volume control.")]
        Spotify = 1,

        /// <summary>
        /// The volume will be changed in the WindowsVolumeMixer and will affect system volume.
        /// </summary>
        [ComboBoxItem("Windows Volume Mixer (device)", "Use the Windows Volume Mixer.\nThis affects the global system volume.")]
        SystemGlobal = 1 << 1,

        /// <summary>
        /// The volume will be changed in the WindowsVolumeMixer and will affect just Spotify.
        /// </summary>
        [ComboBoxItem("Windows Volume Mixer (Spotify)", "Use the Windows Volume Mixer.\nThis only affects Spotify's volume.")]
        SystemSpotifyOnly = SystemGlobal | Spotify
    }
}