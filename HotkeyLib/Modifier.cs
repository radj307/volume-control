namespace HotkeyLib
{
    /// <summary>
    /// Enumerator that represents hotkey modifiers.
    /// </summary>
    /// <remarks>These correspond to the fsModifiers parameter of the <see cref="HotkeyAPI.RegisterHotkey(IntPtr, int, uint, System.Windows.Forms.Keys)"/> function.</remarks>
    [Flags]
    public enum Modifier : uint
    {
        /// <summary>
        /// No Modifier Keys.
        /// </summary>
        NONE = 0x0000,
        /// <summary>
        /// Either ALT key must be held down. 
        /// </summary>
        ALT = 0x0001,
        /// <summary>
        /// Either CTRL key must be held down.
        /// </summary>
        CTRL = 0x0002,
        /// <summary>
        /// Either SHIFT key must be held down.
        /// </summary>
        SHIFT = 0x0004,
        /// <summary>
        /// Either WINDOWS key was held down. These keys are labeled with the Windows logo. Keyboard shortcuts that involve the WINDOWS key are reserved for use by the operating system.
        /// </summary>
        WIN = 0x0008,
        /// <summary>
        /// Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications.
        /// Windows Vista:  This flag is not supported.
        /// </summary>
        NOREPEAT = 0x4000,
    }
}
