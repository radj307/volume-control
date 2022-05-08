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
        public string Name => string.Empty;
        public string DeviceID => string.Empty;
        public List<IAudioSession> GetAudioSessions() => new();
    }
}
