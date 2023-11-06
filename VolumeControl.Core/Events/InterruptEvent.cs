namespace VolumeControl.Core.Events
{
    /// <summary>
    /// Event arguments that include an incoming and outgoing value, as well as a settable Handled property to interrupt the sender.
    /// </summary>
    /// <typeparam name="T">The type of value for this event.</typeparam>
    public class InterruptEventArgs<T> : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="InterruptEventArgs{T}"/> instance with the specified <paramref name="outgoingValue"/> and <paramref name="incomingValue"/>.
        /// </summary>
        /// <param name="outgoingValue">The current value that will be replaced by the <paramref name="incomingValue"/> when Handled is still <see langword="false"/> after all handlers have finished with this event.</param>
        /// <param name="incomingValue">The new value that will potentially be replacing the <paramref name="outgoingValue"/>.</param>
        public InterruptEventArgs(T outgoingValue, T incomingValue)
        {
            OutgoingValue = outgoingValue;
            IncomingValue = incomingValue;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets whether this event has been completely handled or whether the sender should continue its own processing.
        /// </summary>
        /// <returns><see langword="true"/> when the sender should be interrupted; otherwise, <see langword="false"/>.</returns>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the current (outgoing) value that will be replaced by the IncomingValue when Handled is <see langword="false"/>.
        /// </summary>
        public virtual T OutgoingValue { get; }
        /// <summary>
        /// Gets the incoming value.
        /// </summary>
        public virtual T IncomingValue { get; }
        #endregion Properties
    }
    /// <summary>
    /// Event with an incoming and outgoing value that allows handlers to interrupt it by setting <see cref="InterruptEventArgs{T}.Handled"/> to <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// This is intended for use in cases where the sender wants to allow handlers to prevent a change from occurring.
    /// </remarks>
    /// <typeparam name="T">The type of value for this event.</typeparam>
    /// <param name="sender">The object that sent this event.</param>
    /// <param name="e">The event arguments for this event.</param>
    public delegate void InterruptEventHandler<T>(object sender, InterruptEventArgs<T> e);
}
