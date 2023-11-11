using CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.CoreAudio.Interfaces;

namespace VolumeControl.CoreAudio.Events
{
    /// <summary>
    /// Contains event data for the <see cref="VolumeChangedEventHandler"/> event type.
    /// </summary>
    public sealed class VolumeChangedEventArgs : EventArgs
    {
        #region Constructor
        internal VolumeChangedEventArgs(float newNativeVolume, bool newMute)
        {
            NativeVolume = newNativeVolume;
            Volume = VolumeLevelConverter.FromNativeVolume(NativeVolume);
            Mute = newMute;
        }
        internal VolumeChangedEventArgs(int newVolume, bool newMute)
        {
            NativeVolume = VolumeLevelConverter.ToNativeVolume(newVolume);
            Volume = newVolume;
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