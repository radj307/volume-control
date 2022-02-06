using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio
{
    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISimpleAudioVolume
    {
        /// <summary>
        /// Set the current volume level.
        /// </summary>
        /// <param name="fLevel">0-100 Volume Level</param>
        /// <param name="eventContext">A Guid ref providing context.</param>
        /// <returns>int</returns>
        [PreserveSig]
        int SetMasterVolume(float fLevel, ref Guid eventContext);
        /// <summary>
        /// Get the current volume level.
        /// </summary>
        /// <param name="pfLevel">Output float to hold the current audio level.</param>
        /// <returns>int</returns>
        [PreserveSig]
        int GetMasterVolume(out float pfLevel);
        /// <summary>
        /// Set the current mute state.
        /// </summary>
        /// <param name="bMute">Mute is enabled when true.</param>
        /// <param name="eventContext">A Guid ref providing context.</param>
        /// <returns>int</returns>
        [PreserveSig]
        int SetMute(bool bMute, ref Guid eventContext);
        /// <summary>
        /// Get the current mute state.
        /// </summary>
        /// <param name="pbMute">Output boolean</param>
        /// <returns>int</returns>
        [PreserveSig]
        int GetMute(out bool pbMute);
    }
}
