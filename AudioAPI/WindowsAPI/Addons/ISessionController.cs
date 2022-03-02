using AudioAPI.WindowsAPI.Audio;

namespace AudioAPI.WindowsAPI.Addons
{
    public interface ISessionController : ISimpleAudioVolume, IAudioSessionControl2
    {
        bool IsVirtual { get; }
    }
}