using JetBrains.Annotations;
using System;
using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;

namespace ToastifyAPI.Interop
{
    public class Mouse : IMouse
    {
        private readonly MouseDevice mouseDevice;

        public Mouse([NotNull] MouseDevice mouseDevice)
        {
            this.mouseDevice = mouseDevice;
        }

        /// <inheritdoc />
        public bool IsKeyboard() => false;

        /// <inheritdoc />
        public bool IsMouse() => true;

        /// <inheritdoc />
        public bool IsPressed(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                case MouseButton.Right:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                case MouseButton.Middle:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                case MouseButton.XButton1:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                case MouseButton.XButton2:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseButton), mouseButton, null);
            }
        }

        /// <inheritdoc />
        public bool IsReleased(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return this.mouseDevice.LeftButton == MouseButtonState.Released;

                case MouseButton.Right:
                    return this.mouseDevice.LeftButton == MouseButtonState.Released;

                case MouseButton.Middle:
                    return this.mouseDevice.LeftButton == MouseButtonState.Released;

                case MouseButton.XButton1:
                    return this.mouseDevice.LeftButton == MouseButtonState.Released;

                case MouseButton.XButton2:
                    return this.mouseDevice.LeftButton == MouseButtonState.Released;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseButton), mouseButton, null);
            }
        }
    }
}