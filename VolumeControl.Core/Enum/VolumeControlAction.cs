namespace VolumeControl.Core.Enum
{
    public enum VolumeControlAction : byte
    {
        /// <summary>
        /// Null action. Does nothing.
        /// </summary>
        NULL = 0,
        /// <summary>
        /// Performs an increment action on the associated subject.
        /// <list type="bullet">
        /// <item><term>VOLUME</term><description>Increase Target Application Volume.</description></item>
        /// <item><term>MEDIA</term><description>Next Track.</description></item>
        /// <item><term>TARGET</term><description>Next Target.</description></item>
        /// </list>
        /// </summary>
        INCREMENT = 1,
        /// <summary>
        /// Performs an increment action on the associated subject.
        /// <list type="bullet">
        /// <item><term>VOLUME</term><description>Increase Target Application Volume.</description></item>
        /// <item><term>MEDIA</term><description>Next Track.</description></item>
        /// <item><term>TARGET</term><description>Next Target.</description></item>
        /// </list>
        /// </summary>
        NEXT = INCREMENT,
        /// <summary>
        /// Performs a decrement action on the associated subject.
        /// <list type="bullet">
        /// <item><term>VOLUME</term><description>Decrease Target Application Volume.</description></item>
        /// <item><term>MEDIA</term><description>Previous Track.</description></item>
        /// <item><term>TARGET</term><description>Previous Target.</description></item>
        /// </list>
        /// </summary>
        DECREMENT = 2,
        /// <summary>
        /// Performs a decrement action on the associated subject.
        /// <list type="bullet">
        /// <item><term>VOLUME</term><description>Decrease Target Application Volume.</description></item>
        /// <item><term>MEDIA</term><description>Previous Track.</description></item>
        /// <item><term>TARGET</term><description>Previous Target.</description></item>
        /// </list>
        /// </summary>
        PREV = DECREMENT,
        /// <summary>
        /// Performs a toggle action on the associated subject.
        /// <list type="bullet">
        /// <item><term>VOLUME</term><description>Mutes or unmutes the currently selected target.</description></item>
        /// <item><term>MEDIA</term><description>Toggles media playback.</description></item>
        /// <item><term>TARGET</term><description>Locks or unlocks the current target selection.</description></item>
        /// </list>
        /// </summary>
        TOGGLE = 3,
    }
}
