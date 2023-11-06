using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Enums;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Defines extension methods for the <see cref="IHotkey"/> interface.
    /// </summary>
    public static class HotkeyExtensions
    {
        /// <summary>
        /// Returns a <see cref="string"/> that represents the current hotkey object's key combination.
        /// </summary>
        /// <param name="hotkey">(implicit) Hotkey instance to get the string representation of.</param>
        /// <returns>The <see cref="string"/> representation of the key combination of this instance.</returns>
        public static string GetStringRepresentation(this IHotkey hotkey)
        {
            var modifiersString = hotkey.Modifiers.GetStringRepresentation();
            return (modifiersString.Length > 0 ? modifiersString + '+' : string.Empty) + hotkey.Key.ToString();
        }
    }
}
