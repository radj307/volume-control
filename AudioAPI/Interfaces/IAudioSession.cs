using AudioAPI.Implementations;

namespace AudioAPI.Interfaces
{
    /// <summary>
    /// Represents an audio session that is currently running on a device.
    /// </summary>
    /// <remarks>Implements <see cref="IProcess"/>, and is equatable to itself or other implementations of <see cref="IProcess"/>.</remarks>
    public interface IAudioSession : IProcess, IEquatable<IAudioSession>, IEquatable<IProcess>
    {
        /// <summary>
        /// Gets or sets the volume of this process using the <see cref="int"/> type in the range <b>( 0 - 100 )</b>
        /// </summary>
        /// <remarks>Values are clamped before actually being applied, so out-of-bounds values are allowed.</remarks>
        /// <exception cref="InvalidOperationException">Cannot perform operations on a disposed-of object!</exception>
        int Volume { get; set; }
        /// <summary>
        /// Gets or sets the volume of this process using the native <see cref="float"/> type, in the range <b>( 0.0 <i>(0%)</i> - 1.0 <i>(100%)</i> )</b>
        /// </summary>
        /// <remarks>Values are clamped before actually being applied, so out-of-bounds values are allowed.</remarks>
        /// <exception cref="InvalidOperationException">Cannot perform operations on a disposed-of object!</exception>
        float NativeVolume { get; set; }
        /// <summary>
        /// Gets or sets whether the session is muted or not.
        /// </summary>
        bool Muted { get; set; }
    }
}
