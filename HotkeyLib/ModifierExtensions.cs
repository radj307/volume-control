namespace HotkeyLib
{
    /// <summary>
    /// Provides extension methods for the Modifier enumerator type.
    /// </summary>
    public static class ModifierExtensions
    {
        /// <summary>
        /// Check if this modifier contains a given modifier type.
        /// </summary>
        /// <param name="mod">The modifier instance that this function is called from.</param>
        /// <param name="compare">Another modifier type to compare.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>This modifier contains the modifier specified by (compare).</description></item>
        /// <item><term>false</term><description>This modifier does not contain the modifier specified by (compare).</description></item>
        /// </list></returns>
        public static bool Contains(this Modifier mod, Modifier compare)
        {
            return (mod & compare) != 0;
        }
        public static Modifier Set(this ref Modifier mod, Modifier add)
        {
            return mod |= add;
        }
        public static Modifier Unset(this ref Modifier mod, Modifier remove)
        {
            return mod &= ~remove;
        }
        public static Modifier Toggle(this ref Modifier mod, Modifier toggle)
        {
            return mod ^= toggle;
        }
        public static Modifier Apply(this ref Modifier mod, Modifier incoming, bool state)
        {
            return state ? mod.Set(incoming) : mod.Unset(incoming);
        }
        public static bool Empty(this Modifier mod)
        {
            return mod == Modifier.NONE;
        }
        /// <summary>
        /// Check if this modifier has a specific bit set to 1.
        /// </summary>
        /// <param name="mod">The modifier instance that this function is called from.</param>
        /// <param name="bitIndex">The index of the bit to check.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>The specified bit is set to 1.</description></item>
        /// <item><term>false</term><description>The specified bit is set to 0.</description></item>
        /// </list></returns>
        public static bool ContainsBit(this Modifier mod, byte bitIndex)
        {
            return (mod & (Modifier)(1 << bitIndex)) != 0;
        }
        public static Modifier SetBit(this ref Modifier mod, byte bitIndex)
        {
            return mod |= (Modifier)(1 << bitIndex);
        }
        public static Modifier UnsetBit(this ref Modifier mod, byte bitIndex)
        {
            return mod &= ~(Modifier)(1 << bitIndex);
        }
        public static Modifier ToggleBit(this ref Modifier mod, byte bitIndex)
        {
            return mod ^= (Modifier)(1 << bitIndex);
        }
        public static Modifier ApplyBit(this ref Modifier mod, byte bitIndex, bool state)
        {
            return state ? mod.SetBit(bitIndex) : mod.UnsetBit(bitIndex);
        }
        public static uint ToWindowsModifier(this Modifier mod)
            => (uint)mod;
        public static string? Stringify(this Modifier mod)
            => !mod.Empty() ? $"{(mod.Contains(Modifier.ALT) ? "Alt" : "")}{(mod.Contains(Modifier.CTRL) ? "Ctrl" : "")}{(mod.Contains(Modifier.SHIFT) ? "Shift" : "")}{(mod.Contains(Modifier.WIN) ? "Win" : "")}" : null;
    }
}
