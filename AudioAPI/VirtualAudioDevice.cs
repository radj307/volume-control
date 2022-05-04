using AudioAPI.Interfaces;

namespace AudioAPI
{
    /// <summary>
    /// Represents a fake audio device.
    /// </summary>
    public class VirtualAudioDevice : IAudioDevice
    {
        public bool Virtual => true;
        public string Name => string.Empty;
        public string DeviceID => string.Empty;
        public List<IAudioSession> GetAudioSessions() => new();
    }
}
