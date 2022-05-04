using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;

namespace AudioAPI.Interfaces
{
    /// <summary>
    /// Represents an audio session that is currently running on a device.
    /// </summary>
    public interface IAudioSession : IProcess, IEquatable<IAudioSession>, IEquatable<IProcess>
    {
        /// <summary>
        /// Gets or sets the <see cref="VolumeObject"/> for this session.
        /// </summary>
        VolumeObject VolumeObject { get; set; }
        /// <summary>
        /// Gets or sets the volume of this session.
        /// </summary>
        decimal Volume { get; set; }
        /// <summary>
        /// Gets or sets whether the session is muted or not.
        /// </summary>
        bool Muted { get; set; }

        AudioSessionControl2 SessionControl { get; }
        SimpleAudioVolume SessionVolume { get; }
    }
}
