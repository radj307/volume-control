using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using VolumeControl.Core.Input.Enums;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Extensions
{
    /// <summary>
    /// Defines extension methods for various types of Key/ModifierKey enumerations.
    /// </summary>
    public static class KeyExtensions
    {
        #region ModifierKeys
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
        #endregion ModifierKeys

        #region EModifierKey
        /// <summary>
        /// Sets or unsets the bit specified by <paramref name="flag"/> in <see langword="ref"/> <paramref name="m"/>.
        /// </summary>
        /// <param name="m"><see cref="EModifierKey"/></param>
        /// <param name="flag">The modifier to add/remove.</param>
        /// <param name="state">When <see langword="true"/>, <paramref name="flag"/> is set to <b>1</b>; when <see langword="false"/>, <paramref name="flag"/> is set to <b>0</b>.</param>
        /// <returns>The resulting <see cref="EModifierKey"/>.</returns>
        public static EModifierKey SetFlagState(this EModifierKey m, EModifierKey flag, bool state)
            => state
            ? m | flag
            : m & ~flag;
        /// <summary>
        /// Gets the string representation of the <see cref="EModifierKey"/> specified by <paramref name="m"/>.
        /// </summary>
        /// <param name="m"><see cref="EModifierKey"/></param>
        /// <param name="insertSpaces">When <see langword="true"/>, inserts spaces between the enum names &amp; the separator characters.</param>
        /// <param name="separator">The separator character to use between enum names.</param>
        /// <param name="includeNone">When the modifier key is <see cref="EModifierKey.None"/> and this is <see langword="true"/>, the returned string is "None"; otherwise, returns a blank string.</param>
        /// <returns>The <see cref="string"/> representation of <paramref name="m"/> in the form "Shift+Ctrl+Alt".</returns>
        public static string GetStringRepresentation(this EModifierKey m, bool insertSpaces = false, char separator = '+', bool includeNone = false)
        {
            var values = m.GetSingleValues();

            if (values.Length == 1)
            {
                if (!includeNone && values[0] == EModifierKey.None)
                    return string.Empty;
                return string.Format("{0:G}", values[0]);
            }

            var sb = new StringBuilder();
            
            for (int i = 0, i_max = values.Length; i < i_max; ++i)
            {
                var value = values[i];
                switch (value)
                {
                case EModifierKey.Super:
                    sb.Append("Win");
                    break;
                default:
                    sb.AppendFormat("{0:G}", value);
                    break;
                }
                if (i != i_max - 1)
                {
                    if (insertSpaces) sb.Append(' ');
                    sb.Append(separator);
                    if (insertSpaces) sb.Append(' ');
                }
            }

            return sb.ToString();
        }
        #endregion EModifierKey
    }
}
