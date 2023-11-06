namespace VolumeControl.CoreAudio.Interfaces
{
    /// <summary>
    /// Represents an audio instance with a read-only volume level and mute state.
    /// </summary>
    public interface IReadOnlyAudioControl
    {
        /// <summary>
        /// Gets the volume level, in the native float format.
        /// </summary>
        /// <returns>A <see cref="float"/> between <b>0.0</b> and <b>1.0</b>.</returns>
        float NativeVolume { get; }
        /// <summary>
        /// Gets the volume level.
        /// </summary>
        /// <returns>An <see cref="int"/> between <b>0</b> and <b>100</b>.</returns>
        int Volume { get; }
        /// <summary>
        /// Gets the mute state.
        /// </summary>
        bool Mute { get; }
    }
}