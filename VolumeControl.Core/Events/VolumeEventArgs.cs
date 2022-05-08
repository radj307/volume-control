namespace VolumeControl.Core.Events
{
    public class VolumeEventArgs : EventArgs
    {
        public VolumeEventArgs(int volume, bool muted)
        {
            Volume = volume;
            Muted = muted;
        }
        public int Volume { get; }
        public bool Muted { get; }
    }
}
