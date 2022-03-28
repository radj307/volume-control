namespace HotkeyLib
{
    /// <summary>
    /// Enumerator that represents hotkey modifiers.
    /// </summary>
    [Flags]
    public enum Modifier : byte
    {
        /// <summary>
        /// No Modifier Keys.
        /// </summary>
        NONE = 0x0,
        /// <summary>
        /// Left or Right Alt Key.
        /// </summary>
        ALT = 0x1,
        /// <summary>
        /// Left or Right Ctrl Key.
        /// </summary>
        CTRL = 0x2,
        /// <summary>
        /// Left or Right Shift Key.
        /// </summary>
        SHIFT = 0x4,
        /// <summary>
        /// Left or Right Windows (Super) Key.
        /// </summary>
        WIN = 0x8,
    }
}
