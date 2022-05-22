namespace VolumeControl.Audio.Events
{
    /// <summary>
    /// Event arguments for the <see cref="TargetChangedEventHandler"/> event type.<br/>
    /// This simply includes the <see cref="TargetName"/> property.
    /// </summary>
    public class TargetChangedEventArgs : EventArgs
    {
        /// <inheritdoc cref="TargetChangedEventArgs"/>
        /// <param name="targetName">The name of the target after being changed.</param>
        public TargetChangedEventArgs(string targetName) => TargetName = targetName;
        /// <summary>
        /// This is the name of the new target.<br/>
        /// When this event is fired, the actual target string has already changed to this value.
        /// </summary>
        public string TargetName { get; private set; }
    }
    /// <summary>
    /// Event handler triggered after a target has been changed.
    /// </summary>
    /// <param name="sender">The object that fired this event.</param>
    /// <param name="e">The arguments associated with this event.</param>
    public delegate void TargetChangedEventHandler(object sender, TargetChangedEventArgs e);
}
