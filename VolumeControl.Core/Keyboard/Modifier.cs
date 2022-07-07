namespace VolumeControl.Core.Keyboard
{
    /// <summary>
    /// Represents modifier keys that must be held down when pressing a hotkey's primary key for the hotkey press to be registered.
    /// </summary>
    [Flags]
    public enum Modifier : uint
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
    /// <summary>
    /// Extension methods for the <see cref="Modifier"/> <see langword="enum"/>.
    /// </summary>
    public static class ModifierExtensions
    {
        /// <summary>
        /// Sets the bit specified by <paramref name="flag"/> to <b>1</b> in <see langword="ref"/> <paramref name="m"/>.
        /// </summary>
        /// <param name="m">Modifier</param>
        /// <param name="flag">The modifier to add.</param>
        public static void SetFlag(this ref Modifier m, Modifier flag) => m |= flag;
        /// <summary>
        /// Sets the bit specified by <paramref name="flag"/> to <b>0</b> in <see langword="ref"/> <paramref name="m"/>.
        /// </summary>
        /// <param name="m">Modifier</param>
        /// <param name="flag">The modifier to remove.</param>
        public static void UnsetFlag(this ref Modifier m, Modifier flag) => m &= ~flag;
        /// <summary>
        /// Sets or unsets the bit specified by <paramref name="flag"/> in <see langword="ref"/> <paramref name="m"/>.
        /// </summary>
        /// <param name="m">Modifier</param>
        /// <param name="flag">The modifier to add/remove.</param>
        /// <param name="state">When <see langword="true"/>, <paramref name="flag"/> is set to <b>1</b>; when <see langword="false"/>, <paramref name="flag"/> is set to <b>0</b>.</param>
        /// <returns>The resulting <see cref="Modifier"/>.</returns>
        public static Modifier Set(this Modifier m, Modifier flag, bool state)
        {
            if (state)
                m.SetFlag(flag);
            else
                m.UnsetFlag(flag);
            return m;
        }
    }
}
