using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;

namespace AudioAPI.Forms
{
    public interface IAudioProcess : IDisposable
    {
        Process Process { get; }

        string ProcessName { get; }
        string DisplayName { get; set; }

        IAudioSessionControl2 SessionControl { get; }

        ISimpleAudioVolume AudioControl { get; }

        int PID { get; }

        public float Volume { get; set; }
        public bool Muted { get; set; }
    }
}
