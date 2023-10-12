namespace VolumeControl.CoreAudio.Events
{
    /// <summary>
    /// Event arguments object for the <see cref="PreviewSessionIsSelectedEventHandler"/> event type.
    /// </summary>
    public sealed class PreviewSessionIsSelectedEventArgs : EventArgs
    {
        #region Constructor
        internal PreviewSessionIsSelectedEventArgs(AudioSession audioSession, bool isSelected)
        {
            AudioSession = audioSession;
            IsSelected = isSelected;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the AudioSession instance that is being operated on.
        /// </summary>
        public AudioSession AudioSession { get; }
        /// <summary>
        /// Gets or sets whether the AudioSession should be selected when initialized.
        /// </summary>
        public bool IsSelected { get; set; }
        #endregion Properties
    }
    /// <summary>
    /// Event handler type for the <see cref="AudioSessionMultiSelector.PreviewSessionIsSelected"/> event.
    /// </summary>
    /// <param name="sender">The <see cref="AudioSessionMultiSelector"/> instance that triggered this event.</param>
    /// <param name="e">Event arguments that allow setting whether a session is selected by default.</param>
    public delegate void PreviewSessionIsSelectedEventHandler(object sender, PreviewSessionIsSelectedEventArgs e);
}
