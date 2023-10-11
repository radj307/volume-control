namespace VolumeControl.CoreAudio.Events
{
    /// <summary>
    /// Event arguments for the preview session name event, which allows session display names to be changed when they are added by the <see cref="AudioSessionManager"/>.
    /// </summary>
    public sealed class PreviewSessionNameEventArgs : EventArgs
    {
        #region Initializer
        internal PreviewSessionNameEventArgs(AudioSession audioSession, string sessionName)
        {
            AudioSession = audioSession;
            SessionName = sessionName;
        }
        #endregion Initializer

        #region Properties
        /// <summary>
        /// Gets the AudioSession instance that this event was triggered for.
        /// </summary>
        public AudioSession AudioSession { get; }
        /// <summary>
        /// Gets or sets the initial name that will be used for the new audio session instance.
        /// </summary>
        public string SessionName { get; set; }
        #endregion Properties
    }
    /// <summary>
    /// Handler delegate type for the <see cref="AudioSessionManager.PreviewSessionName"/> event.
    /// </summary>
    /// <param name="sender">The <see cref="AudioSessionManager"/> instance that invoked the event.</param>
    /// <param name="e">Event arguments that allow changing an audio session's display name by modifying the <see cref="PreviewSessionNameEventArgs.SessionName"/> property.</param>
    public delegate void PreviewSessionNameEventHandler(object sender, PreviewSessionNameEventArgs e);
}