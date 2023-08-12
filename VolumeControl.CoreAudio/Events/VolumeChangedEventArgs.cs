using Audio.Helpers;
using Audio.Interfaces;
using CoreAudio;

namespace Audio.Events
{
    /// <summary>
    /// Contains event data for the <see cref="VolumeChangedEventHandler"/> event type.
    /// </summary>
    public sealed class VolumeChangedEventArgs : EventArgs, IReadOnlyVolumeControl
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="VolumeChangedEventArgs"/> instance with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="CoreAudio.AudioVolumeNotificationData"/> object from the underlying event.</param>
        public VolumeChangedEventArgs(AudioVolumeNotificationData data)
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
        public VolumeChangedEventArgs(float newVolume, bool newMute)
        {
            NativeVolume = newVolume;
            Volume = VolumeLevelConverter.FromNativeVolume(NativeVolume);
            Mute = newMute;
        }
        #endregion Constructor

        #region Properties
        public float NativeVolume { get; }
        public int Volume { get; }
        public bool Mute { get; }
        #endregion Properties
    }
    /// <summary>
    /// Represents a method that is called when an audio instance's volume or mute state was changed.
    /// </summary>
    /// <param name="sender">The <see cref="IVolumeControl"/> instance that sent the event.</param>
    /// <param name="e"></param>
    public delegate void VolumeChangedEventHandler(IVolumeControl? sender, VolumeChangedEventArgs e);
}