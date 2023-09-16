using CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;

namespace VolumeControl.CoreAudio.Events
{
    /// <summary>
    /// Contains event data for the <see cref="VolumeChangedEventHandler"/> event type.
    /// </summary>
    public sealed class VolumeChangedEventArgs : EventArgs, IReadOnlyAudioControl
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="VolumeChangedEventArgs"/> instance with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="AudioVolumeNotificationData"/> object from the underlying event.</param>
        internal VolumeChangedEventArgs(AudioVolumeNotificationData data)
        {
            NativeVolume = data.MasterVolume;
            Volume = VolumeLevelConverter.FromNativeVolume(NativeVolume);
            Mute = data.Muted;
        }
        /// <summary>
        /// Creates a new <see cref="VolumeChangedEventArgs"/> instance with the given <paramref name="newVolume"/> &amp; <paramref name="newMute"/> values.
        /// </summary>
        /// <param name="newVolume">The new volume level.</param>
        /// <param name="newMute">The new mute state.</param>
        internal VolumeChangedEventArgs(float newVolume, bool newMute)
        {
            NativeVolume = newVolume;
            Volume = VolumeLevelConverter.FromNativeVolume(NativeVolume);
            Mute = newMute;
        }
        #endregion Constructor

        #region Properties
        /// <inheritdoc/>
        public float NativeVolume { get; }
        /// <inheritdoc/>
        public int Volume { get; }
        /// <inheritdoc/>
        public bool Mute { get; }
        #endregion Properties
    }
    /// <summary>
    /// Represents a method that is called when an audio instance's volume or mute state was changed.
    /// </summary>
    /// <param name="sender">The <see cref="IAudioControl"/> instance that sent the event.</param>
    /// <param name="e"></param>
    public delegate void VolumeChangedEventHandler(IAudioControl? sender, VolumeChangedEventArgs e);
}