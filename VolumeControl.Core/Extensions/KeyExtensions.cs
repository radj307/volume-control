using System.ComponentModel;
using System.Windows.Input;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Extensions
{
    /// <summary>
    /// Defines extension methods for various types of Key/ModifierKey enumerations.
    /// </summary>
    public static class KeyExtensions
    {
        /// <summary>
        /// Gets the <see cref="Key"/> associated with this <see cref="ModifierKeys"/> value.
        /// </summary>
        /// <remarks>
        /// The modifier key value <b>must contain exactly one flag value</b> or an <see cref="InvalidOperationException"/> will be thrown.
        /// </remarks>
        /// <param name="modifierKey">(implicit) The enum value to operate on.</param>
        /// <param name="useLeftSideKey">When <see langword="true"/>, the left-side variants of the key are returned; otherwise, the right-side variants are returned.</param>
        /// <returns>The <see cref="Key"/> value representing the <see cref="ModifierKeys"/> value.</returns>
        /// <exception cref="InvalidEnumArgumentException">The specified <paramref name="modifierKey"/> was not a valid value for the <see cref="ModifierKeys"/> enumeration.</exception>
        /// <exception cref="InvalidOperationException">The specified <paramref name="modifierKey"/> was not a single flag value from the <see cref="ModifierKeys"/> enumeration.</exception>
        public static Key ToKey(this ModifierKeys modifierKey, bool useLeftSideKey = true)
        {
            if (!modifierKey.IsSingleValue())
                throw new InvalidOperationException($"Expected a single value from the {typeof(ModifierKeys)} enumeration, received {modifierKey:G}!");

#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (modifierKey) //< this is faster
#pragma warning restore IDE0066 // Convert switch statement to expression
            {
            case ModifierKeys.None:
                return Key.None;
            case ModifierKeys.Alt:
                return useLeftSideKey ? Key.LeftAlt : Key.RightAlt;
            case ModifierKeys.Control:
                return useLeftSideKey ? Key.LeftCtrl : Key.RightCtrl;
            case ModifierKeys.Shift:
                return useLeftSideKey ? Key.LeftShift : Key.RightShift;
            case ModifierKeys.Windows:
                return useLeftSideKey ? Key.LWin : Key.RWin;
            default:
                throw new InvalidEnumArgumentException(nameof(modifierKey), (int)modifierKey, typeof(ModifierKeys));
            }
        }
        /// <summary>
        /// Gets the <see cref="Key"/> values associated with this <see cref="ModifierKeys"/> value.
        /// </summary>
        /// <remarks>
        /// Unlike the ToKey method, this can accept <see cref="ModifierKeys"/> values that have more than one flag set.
        /// </remarks>
        /// <param name="modifierKeys">(implicit) The enum value to operate on.</param>
        /// <param name="useLeftSideKey">When <see langword="true"/>, the left-side variants of the key are returned; otherwise, the right-side variants are returned.</param>
        /// <returns>The <see cref="Key"/> values representing all of the flags that were set in the <see cref="ModifierKeys"/> value.</returns>
        public static IEnumerable<Key> ToKeys(this ModifierKeys modifierKeys, bool useLeftSideKey = true)
            => modifierKeys.GetSingleValues().Select(mk => mk.ToKey(useLeftSideKey));
    }
}
