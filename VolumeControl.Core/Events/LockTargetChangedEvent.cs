namespace VolumeControl.Core.Events
{
    public class LockTargetChangedEventArgs : EventArgs
    {
        public LockTargetChangedEventArgs(bool state) => State = state;
        public bool State { get; }
    }
    public delegate void LockTargetChangedEventHandler(object sender, LockTargetChangedEventArgs e);
}
