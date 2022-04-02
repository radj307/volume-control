namespace VolumeControl.Core.Events
{
    /// <summary>
    /// Event arguments for target selection change events.
    /// </summary>
    public class TargetEventArgs : EventArgs
    {
        /// <summary>
        /// This determines whether the event was triggered because of user keyboard input using a text box, or because of another source such as target switching hotkeys.
        /// <list type="table">
        /// <item><term>true</term><description>The event was triggered by user input.</description></item>
        /// <item><term>false</term><description>The event was automatically triggered because of another event, such as a hotkey press.</description></item>
        /// </list>
        /// </summary>
        public bool UserOrigin;
        /// <summary>
        /// Empty event, sets UserOrigin to false.
        /// </summary>
        public new static readonly TargetEventArgs Empty = new() { UserOrigin = false };
    }
    /// <summary>
    /// Event handler for target selection change events.
    /// </summary>
    /// <param name="sender">The sender of this event.</param>
    /// <param name="e">Event arguments, including whether this event was triggered by the user or not.</param>
    public delegate void TargetEventHandler(object sender, TargetEventArgs e);
}
