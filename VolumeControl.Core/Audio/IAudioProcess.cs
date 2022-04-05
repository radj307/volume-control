namespace VolumeControl.Core.Audio
{
    public interface IAudioProcess
    {
        bool Virtual { get; }
        string ProcessName { get; }
    }
}
