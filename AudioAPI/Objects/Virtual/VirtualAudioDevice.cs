using AudioAPI.Interfaces;

namespace AudioAPI.Objects.Virtual
{
    /// <summary>
    /// Represents a fake audio device.
    /// </summary>
    public class VirtualAudioDevice : IAudioDevice
    {
        /// <inheritdoc/>
        /// <remarks>This object is always virtual.</remarks>
        public bool Virtual => true;
        /// <inheritdoc/>
        public string Name => string.Empty;
        /// <inheritdoc/>
        public string DeviceID => string.Empty;
        /// <inheritdoc/>
        public List<IAudioSession> GetAudioSessions() => new();
    }
}
