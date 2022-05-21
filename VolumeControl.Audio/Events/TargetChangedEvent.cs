namespace VolumeControl.Audio.Events
{
    public class TargetChangedEventArgs : EventArgs
    {
        public TargetChangedEventArgs(string targetName) => TargetName = targetName;
        public string TargetName { get; private set; }
    }
    public delegate void TargetChangedEventHandler(object sender, TargetChangedEventArgs e);
}
