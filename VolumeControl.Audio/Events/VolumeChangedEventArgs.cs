namespace VolumeControl.Audio.Events
{
    /// <summary>
    /// Event arguments for volume change events.
    /// </summary>
    public class VolumeChangedEventArgs : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="VolumeChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="volume">The new volume level of the target.<br/><i>(See <see cref="Volume"/>)</i></param>
        /// <param name="muted">The new mute state of the target.<br/><i>(See <see cref="Muted"/>)</i></param>
        public VolumeChangedEventArgs(int volume, bool muted)
        {
            this.Volume = volume;
            this.Muted = muted;
        }
        #endregion Constructor

        #region Fields
        /// <inheritdoc/>
        public static new readonly VolumeChangedEventArgs Empty = new(0, false);
        #endregion Fields

        #region Properties
        /// <summary>
        /// The new volume level of the target.
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// The new mute state of the target.
        /// </summary>
        public bool Muted { get; set; }
        #endregion Properties
    }

    /// <summary>
    /// Event handler delegate that uses the <see cref="VolumeChangedEventArgs"/> event args type.
    /// </summary>
    /// <param name="sender">The object instance that triggered the event.</param>
    /// <param name="e">An instance of <see cref="VolumeChangedEventArgs"/> that contains the event data.</param>
    public delegate void VolumeChangedEventHandler(object? sender, VolumeChangedEventArgs e);
}
