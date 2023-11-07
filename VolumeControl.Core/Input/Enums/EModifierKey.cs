namespace VolumeControl.Core.Input.Enums
{
    /// <summary>
    /// Represents modifier keys that must be held down when pressing a hotkey's primary key for the hotkey press to be registered.
    /// </summary>
    [Flags]
    public enum EModifierKey : uint
    {
        /// <summary>
        /// No modifier keys.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// Left or right ALT key.
        /// </summary>
        Alt = 0x0001,
        /// <summary>
        /// Left or right CTRL key.
        /// </summary>
        Ctrl = 0x0002,
        /// <summary>
        /// Left or right SHIFT key.
        /// </summary>
        Shift = 0x0004,
        /// <summary>
        /// Left or right SUPER key.<br/>
        /// <b>Super is commonly known as the <i>Windows key</i>.</b>
        /// </summary>
        Super = 0x0008,
        /// <summary>
        /// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.<br/>
        /// This flag is not supported on Windows Vista.
        /// </summary>
        NoRepeat = 0x4000,
    }
}
