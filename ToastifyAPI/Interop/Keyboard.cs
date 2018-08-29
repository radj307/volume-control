using System.Windows.Input;
using JetBrains.Annotations;
using ToastifyAPI.Interop.Interfaces;

namespace ToastifyAPI.Interop
{
    /// <summary>
    ///     Represents a keyboard.
    /// </summary>
    public class Keyboard : IKeyboard
    {
        private readonly KeyboardDevice keyboardDevice;

        public Keyboard([NotNull] KeyboardDevice keyboardDevice)
        {
            this.keyboardDevice = keyboardDevice;
        }

        /// <inheritdoc />
        public bool IsKeyboard()
        {
            return true;
        }

        /// <inheritdoc />
        public bool IsMouse()
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsKeyDown(Key key)
        {
            return this.keyboardDevice.IsKeyDown(key);
        }

        /// <inheritdoc />
        public bool IsKeyUp(Key key)
        {
            return this.keyboardDevice.IsKeyUp(key);
        }
    }
}