using System.ComponentModel;
using System.Windows.Input;
using VolumeControl.Core.Input.Enums;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="KeyboardDevice"/> class.
    /// </summary>
    public static class KeyboardDeviceExtensions
    {
        /// <summary>
        /// Checks if the specified <paramref name="modifierKey"/> is currently pressed on this keyboard device.
        /// </summary>
        /// <remarks>
        /// When <paramref name="modifierKey"/> is <see cref="EModifierKey.None"/>, returns <see langword="true"/> when no modifier keys are pressed.
        /// </remarks>
        /// <param name="keyboard">(implicit) <see cref="KeyboardDevice"/> to check.</param>
        /// <param name="modifierKey">The <see cref="EModifierKey"/> to check.</param>
        /// <returns><see langword="true"/> when the specified modifier key is pressed; otherwise <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="modifierKey"/> was <see cref="EModifierKey.NoRepeat"/>.</exception>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="modifierKey"/> was not a valid single <see cref="EModifierKey"/> value.</exception>
        public static bool IsModifierKeyDown(this KeyboardDevice keyboard, EModifierKey modifierKey)
        {
            if (!modifierKey.IsSingleValue())
                throw new InvalidEnumArgumentException(nameof(modifierKey), (int)modifierKey, typeof(EModifierKey));
            switch (modifierKey)
            {
            case EModifierKey.NoRepeat:
                throw new InvalidOperationException($"{nameof(EModifierKey.NoRepeat)} is not a physical key!");
            case EModifierKey.Alt:
                return keyboard.IsKeyDown(Key.LeftAlt) || keyboard.IsKeyDown(Key.RightAlt);
            case EModifierKey.Ctrl:
                return keyboard.IsKeyDown(Key.LeftCtrl) || keyboard.IsKeyDown(Key.RightCtrl);
            case EModifierKey.Shift:
                return keyboard.IsKeyDown(Key.LeftShift) || keyboard.IsKeyDown(Key.RightShift);
            case EModifierKey.Super:
                return keyboard.IsKeyDown(Key.LWin) || keyboard.IsKeyDown(Key.RWin);
            case EModifierKey.None:
                return !keyboard.IsKeyDown(Key.LeftAlt) && !keyboard.IsKeyDown(Key.RightAlt)
                    && !keyboard.IsKeyDown(Key.LeftCtrl) && !keyboard.IsKeyDown(Key.RightCtrl)
                    && !keyboard.IsKeyDown(Key.LeftShift) && !keyboard.IsKeyDown(Key.RightShift)
                    && !keyboard.IsKeyDown(Key.LWin) && !keyboard.IsKeyDown(Key.RWin);
            default:
                throw new InvalidEnumArgumentException(nameof(modifierKey), (int)modifierKey, typeof(EModifierKey));
            }
        }
        /// <summary>
        /// Checks if any modifier keys are pressed.
        /// </summary>
        /// <param name="keyboard">(implicit) <see cref="KeyboardDevice"/> to check.</param>
        /// <returns><see langword="true"/> when at least one modifier key is pressed; otherwise <see langword="false"/>.</returns>
        public static bool IsAnyModifierKeyDown(this KeyboardDevice keyboard)
            => !IsModifierKeyDown(keyboard, EModifierKey.None);
    }
}
