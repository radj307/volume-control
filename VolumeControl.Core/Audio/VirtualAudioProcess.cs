namespace VolumeControl.Core.Audio
{
    public class VirtualAudioProcess : IAudioProcess
    {
        public VirtualAudioProcess(string name)
        {
            ProcessName = name;
        }
        public bool Virtual => true;
        public string ProcessName { get; }
        public int PID => -1;
    }
}
