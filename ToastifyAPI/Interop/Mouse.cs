using System;
using System.Windows.Input;
using JetBrains.Annotations;
using ToastifyAPI.Interop.Interfaces;

namespace ToastifyAPI.Interop
{
    /// <summary>
    ///     Represents a mouse.
    /// </summary>
    public class Mouse : IMouse
    {
        private readonly MouseDevice mouseDevice;

        public Mouse([NotNull] MouseDevice mouseDevice)
        {
            this.mouseDevice = mouseDevice;
        }

        /// <inheritdoc />
        public bool IsKeyboard()
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsMouse()
        {
            return true;
        }

        /// <inheritdoc />
        public bool IsPressed(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return this.mouseDevice.LeftButton == MouseButtonState.Pressed;

                case MouseButton.Right:
                    return this.mouseDevice.RightButton == MouseButtonState.Pressed;

                case MouseButton.Middle:
                    return this.mouseDevice.MiddleButton == MouseButtonState.Pressed;

                case MouseButton.XButton1:
                    return this.mouseDevice.XButton1 == MouseButtonState.Pressed;

                case MouseButton.XButton2:
                    return this.mouseDevice.XButton2 == MouseButtonState.Pressed;

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
                    return this.mouseDevice.RightButton == MouseButtonState.Released;

                case MouseButton.Middle:
                    return this.mouseDevice.MiddleButton == MouseButtonState.Released;

                case MouseButton.XButton1:
                    return this.mouseDevice.XButton1 == MouseButtonState.Released;

                case MouseButton.XButton2:
                    return this.mouseDevice.XButton2 == MouseButtonState.Released;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mouseButton), mouseButton, null);
            }
        }
    }
}