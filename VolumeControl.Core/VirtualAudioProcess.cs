using AudioAPI.Interfaces;

namespace VolumeControl.Core
{
    public class VirtualAudioProcess : IProcess
    {
        public bool Virtual => true;
        public string ProcessName => "";
        public int PID => -1;
        public bool Equals(IProcess? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
    }
}
