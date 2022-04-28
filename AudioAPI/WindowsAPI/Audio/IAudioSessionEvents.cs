using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio
{
    /// <summary>
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionevents"/>
    /// </summary>
    [Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionEvents
    {
        /// <summary>
        /// The OnChannelVolumeChanged method notifies the client that the volume level of an audio channel in the session submix has changed.
        /// </summary>
        /// <param name="ChannelCount">The channel count. This parameter specifies the number of audio channels in the session submix.</param>
        /// <param name="NewChannelVolumeArray">Pointer to an array of volume levels. Each element is a value of type float that specifies the volume level for a particular channel. Each volume level is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation). The number of elements in the array is specified by the ChannelCount parameter. If an audio stream contains n channels, the channels are numbered from 0 to n– 1. The array element whose index matches the channel number, contains the volume level for that channel. Assume that the array remains valid only for the duration of the call.</param>
        /// <param name="ChangedChannel">The number of the channel whose volume level changed. Use this value as an index into the NewChannelVolumeArray array. If the session submix contains n channels, the channels are numbered from 0 to n– 1. If more than one channel might have changed (for example, as a result of a call to the IChannelAudioVolume::SetAllVolumes method), the value of ChangedChannel is (DWORD)(–1).</param>
        /// <param name="EventContext">The event context value. This is the same value that the caller passed to the IChannelAudioVolume::SetChannelVolume or IChannelAudioVolume::SetAllVolumes method in the call that initiated the change in volume level of the channel. For more information, see Remarks.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        //[PreserveSig]
        //int OnChannelVolumeChanged(int ChannelCount, float[] NewChannelVolumeArray, int ChangedChannel, ref Guid EventContext);
        /// <summary>
        /// The OnDisplayNameChanged method notifies the client that the display name for the session has changed.
        /// </summary>
        /// <param name="NewDisplayName">The new display name for the session. This parameter points to a null-terminated, wide-character string containing the new display name. The string remains valid for the duration of the call.</param>
        /// <param name="EventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetDisplayName in the call that changed the display name for the session. For more information, see Remarks.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        //[PreserveSig]
        //int OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] ref IntPtr NewDisplayName, ref Guid EventContext);
        // ...
        /// <summary>
        /// The OnSimpleVolumeChanged method notifies the client that the volume level or muting state of the audio session has changed.
        /// </summary>
        /// <param name="NewVolume">The new volume level for the audio session. This parameter is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation).</param>
        /// <param name="NewMute">The new muting state. If TRUE, muting is enabled. If FALSE, muting is disabled.</param>
        /// <param name="EventContext">The event context value. This is the same value that the caller passed to ISimpleAudioVolume::SetMasterVolume or ISimpleAudioVolume::SetMute in the call that changed the volume level or muting state of the session. For more information, see Remarks.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        int OnSimpleVolumeChanged(float NewVolume, bool NewMute, ref Guid EventContext);
    }
}
