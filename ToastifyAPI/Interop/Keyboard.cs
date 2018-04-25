using JetBrains.Annotations;
using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;

namespace ToastifyAPI.Interop
{
    public class Keyboard : IKeyboard
    {
        private readonly KeyboardDevice keyboardDevice;

        public Keyboard([NotNull] KeyboardDevice keyboardDevice)
        {
            this.keyboardDevice = keyboardDevice;
        }

        /// <inheritdoc />
        public bool IsKeyboard() => true;

        /// <inheritdoc />
        public bool IsMouse() => false;

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