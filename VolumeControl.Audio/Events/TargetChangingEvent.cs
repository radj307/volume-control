namespace VolumeControl.Audio.Events
{
    /// <summary>Event arguments for the <see cref="TargetChangingEventHandler"/> event handler type.</summary>
    public class TargetChangingEventArgs : EventArgs
    {
        /// <inheritdoc cref="TargetChangingEventArgs"/>
        /// <param name="outgoing">The current target name.</param>
        /// <param name="incoming">The incoming target name.</param>
        /// <param name="defaultCancel">Determines the default value of the <see cref="Cancel"/> property.</param>
        public TargetChangingEventArgs(string outgoing, string incoming, bool defaultCancel = false)
        {
            Outgoing = outgoing;
            Incoming = incoming;
            Cancel = defaultCancel;
        }

        /// <summary>This is the outgoing target, which will be replaced by the <see cref="Incoming"/> target, unless <see cref="Cancel"/> is set to true.</summary>
        /// <remarks>This is read-only to event handlers.</remarks>
        public string Outgoing { get; private set; }
        /// <summary>This is the incoming target, which will replace the <see cref="Outgoing"/> target, unless <see cref="Cancel"/> is set to true.</summary>
        /// <remarks>This can be modified in event handlers, which allows you to validate target names.</remarks>
        public string Incoming { get; set; }
        /// <summary>When true, the incoming changes are rejected, and the outgoing changes are used exclusively instead.</summary>
        public bool Cancel { get; set; }
    }
    /// <summary>Event handler used by <see cref="AudioAPI"/> when the <see cref="AudioAPI.Target"/> property is changed.</summary>
    /// <remarks>This event allows you to intercept and modify the final value of <see cref="AudioAPI.Target"/> before it is set.</remarks>
    /// <param name="sender">The object who triggered this event. In most cases, this is <see cref="AudioAPI"/>.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void TargetChangingEventHandler(object sender, TargetChangingEventArgs e);
}
