using VolumeControl.CoreAudio.Interfaces;
using VolumeControl.CoreAudio.Structs;

namespace VolumeControl.CoreAudio.Helpers
{
    /// <summary>
    /// Extension methods for any class that implements the <see cref="IAudioControl"/> interface.
    /// </summary>
    public static class AudioControlExtensions
    {
        /// <summary>
        /// Increases the native volume by the specified <paramref name="amount"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to add to the native volume.</param>
        public static void IncreaseNativeVolume(this IAudioControl audioControl, float amount)
            => audioControl.NativeVolume += amount;
        /// <summary>
        /// Decreases the native volume by the specified <paramref name="amount"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to subtract from the native volume.</param>
        public static void DecreaseNativeVolume(this IAudioControl audioControl, float amount)
            => audioControl.NativeVolume -= amount;
        /// <summary>
        /// Increases the volume level by the specified <paramref name="amount"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to add to the current volume level.</param>
        public static void IncreaseVolume(this IAudioControl audioControl, int amount)
            => audioControl.Volume += amount;
        /// <summary>
        /// Decreases the volume level by the specified <paramref name="amount"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="amount">The amount to subtract from the current volume level.</param>
        public static void DecreaseVolume(this IAudioControl audioControl, int amount)
            => audioControl.Volume -= amount;
        /// <summary>
        /// Sets the specified <paramref name="muteState"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="muteState"><see langword="true"/> to mute; <see langword="false"/> to unmute.</param>
        public static void SetMute(this IAudioControl audioControl, bool muteState)
            => audioControl.Mute = muteState;
        /// <summary>
        /// Toggles the mute state.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        public static void ToggleMute(this IAudioControl audioControl)
            => audioControl.Mute = !audioControl.Mute;
        /// <summary>
        /// Sets the volume level &amp; mute state to the values in the specified <paramref name="volumeState"/>, if they're set.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="volumeState"><see cref="VolumeState"/> instance containing the values to set.</param>
        public static void SetState(this IAudioControl audioControl, VolumeState volumeState)
        {
            if (volumeState.HasVolume)
                audioControl.Volume = volumeState.Volume;
            if (volumeState.HasMute)
                audioControl.Mute = volumeState.Mute;
        }
        /// <summary>
        /// Sets the volume level &amp; mute state to the values in the specified <paramref name="other"/> instance.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="other">Another <see cref="IAudioControl"/> instance to get the volume &amp; mute state values from..</param>
        public static void SetState(this IAudioControl audioControl, IAudioControl other)
        {
            audioControl.Volume = other.Volume;
            audioControl.Mute = other.Mute;
        }
        /// <summary>
        /// Sets the volume level &amp; mute state to the ones in the specified <paramref name="readOnlyAudioControl"/>.
        /// </summary>
        /// <param name="audioControl">(implicit) <see cref="IAudioControl"/> instance.</param>
        /// <param name="readOnlyAudioControl"><see cref="IReadOnlyAudioControl"/> instance containing the values to set.</param>
        public static void SetState(this IAudioControl audioControl, IReadOnlyAudioControl readOnlyAudioControl)
        {
            audioControl.Volume = readOnlyAudioControl.Volume;
            audioControl.Mute = readOnlyAudioControl.Mute;
        }
    }
}