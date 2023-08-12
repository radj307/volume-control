namespace Audio.Interfaces
{
    /// <summary>
    /// Represents an audio instance with a read-only volume level and mute state.
    /// </summary>
    public interface IReadOnlyVolumeControl
    {
        /// <summary>
        /// Gets the volume level of the audio instance, in the native float format.
        /// </summary>
        /// <remarks>
        /// Range: <b>0.0</b> - <b>1.0</b>
        /// </remarks>
        float NativeVolume { get; }
        /// <summary>
        /// Gets the volume level of the audio instance.
        /// </summary>
        /// <remarks>
        /// Range: <b>0</b> - <b>100</b>
        /// </remarks>
        int Volume { get; }
        /// <summary>
        /// Gets the mute state of the audio instance.
        /// </summary>
        bool Mute { get; }
    }
}