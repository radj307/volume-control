namespace VolumeControl.Audio.Events
{
    /// <summary>
    /// Generic read-only data wrapper for the <see cref="GenericReadOnlyEventHandler{T}"/> event type.
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class GenericReadOnlyEventArgs<T> : EventArgs
    {
        /// <summary>Constructs a new <see cref="GenericReadOnlyEventArgs{T}"/> instance.</summary>
        /// <param name="data">The event data</param>
        public GenericReadOnlyEventArgs(T data) => Data = data;
        /// <summary>Contains the event data.</summary>
        public T Data { get; }
    }
    /// <summary>
    /// Event handler for generic event types.
    /// </summary>
    /// <typeparam name="T">The type to use as the event argument data type.</typeparam>
    public delegate void GenericReadOnlyEventHandler<T>(object? sender, GenericReadOnlyEventArgs<T> e);
}
