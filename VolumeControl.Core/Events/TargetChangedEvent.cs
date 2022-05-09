namespace VolumeControl.Core.Events
{
    public class TargetChangedEventArgs : EventArgs
    {
        public TargetChangedEventArgs(string target) => Target = target;
        public string Target { get; }
    }
    public delegate void TargetChangedEventHandler(object sender, TargetChangedEventArgs e);
}
