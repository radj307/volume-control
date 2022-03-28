namespace Core.Enum
{
    /// <summary>
    /// The list of possible targets for any given VolumeControlAction.
    /// </summary>
    public enum VolumeControlSubject : byte
    {
        /// <summary>
        /// Null target. Does nothing.
        /// </summary>
        NULL = 0,
        /// <summary>
        /// Control the volume of the currently selected target.
        /// </summary>
        VOLUME = 1,
        /// <summary>
        /// Controls media playback.
        /// </summary>
        MEDIA = 2,
        /// <summary>
        /// Control the current target selection.
        /// </summary>
        TARGET = 3,
    }
}
