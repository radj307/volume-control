namespace VolumeControl.CoreAudio.Events
{
    /// <summary>
    /// Event arguments for the <see cref="PreviewSessionIsHiddenEventHandler"/> event type.
    /// </summary>
    public sealed class PreviewSessionIsHiddenEventArgs : EventArgs
    {
        #region Constructor
        internal PreviewSessionIsHiddenEventArgs(AudioSession audioSession, bool isHidden)
        {
            AudioSession = audioSession;
            SessionIsHidden = isHidden;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the AudioSession instance that this event was triggered for.
        /// </summary>
        public AudioSession AudioSession { get; }
        /// <summary>
        /// Gets or sets whether the AudioSession is initially hidden or not.
        /// </summary>
        public bool SessionIsHidden { get; set; }
        #endregion Properties
    }
    /// <summary>
    /// Event handler type for the <see cref="AudioSessionManager.PreviewSessionIsHidden"/> event.
    /// </summary>
    /// <param name="sender">The <see cref="AudioSessionManager"/> instance that triggered this event.</param>
    /// <param name="e">The <see cref="PreviewSessionIsHiddenEventArgs"/> instance for this event.</param>
    public delegate void PreviewSessionIsHiddenEventHandler(object sender, PreviewSessionIsHiddenEventArgs e);
}
