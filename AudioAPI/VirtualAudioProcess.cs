using AudioAPI.Interfaces;

namespace AudioAPI
{
    public class VirtualAudioProcess : IProcess
    {
        public bool Virtual => true;
        public string ProcessName => "";
        public int PID => -1;
        public bool Equals(IProcess? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
    }
}
