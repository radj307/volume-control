namespace VolumeControl.Core.Events
{
    public class SwitchEventArgs<T> : EventArgs
    {
        public SwitchEventArgs(T newTarget)
        {
            Target = newTarget;
        }
        
        public T Target { get; }
    }
}
