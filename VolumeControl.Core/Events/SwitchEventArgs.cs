namespace VolumeControl.Core.Events
{
    public class SwitchEventArgs<T> : EventArgs
    {
        public SwitchEventArgs(T prevTarget, T newTarget)
        {
            OldTarget = prevTarget;
            NewTarget = newTarget;
        }
        
        public T OldTarget { get; }
        public T NewTarget { get; }
    }
}
