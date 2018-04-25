using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;

namespace ToastifyAPI.Interop
{
    public class InputDevices : IInputDevices
    {
        public static Keyboard PrimaryKeyboard { get; } = new Keyboard(System.Windows.Input.Keyboard.PrimaryDevice);
        public static Mouse PrimaryMouse { get; } = new Mouse(System.Windows.Input.Mouse.PrimaryDevice);

        public InputDevices()
        {
            this.Keyboard = PrimaryKeyboard;
            this.Mouse = PrimaryMouse;
        }

        public InputDevices(IKeyboard keyboard, IMouse mouse)
        {
            this.Keyboard = keyboard;
            this.Mouse = mouse;
        }

        /// <inheritdoc />
        public IKeyboard Keyboard { get; set; }

        /// <inheritdoc />
        public IMouse Mouse { get; set; }

        /// <inheritdoc />
        public bool IsPressed(Key key)
        {
            return this.Keyboard?.IsKeyDown(key) ?? false;
        }

        /// <inheritdoc />
        public bool ArePressed(ModifierKeys modifiers)
        {
            bool alt = modifiers.HasFlag(ModifierKeys.Alt);
            bool ctrl = modifiers.HasFlag(ModifierKeys.Control);
            bool shift = modifiers.HasFlag(ModifierKeys.Shift);
            bool win = modifiers.HasFlag(ModifierKeys.Windows);

            return (!alt || this.IsPressed(Key.LeftAlt) || this.IsPressed(Key.RightAlt)) &&
                   (!ctrl || this.IsPressed(Key.LeftCtrl) || this.IsPressed(Key.RightCtrl)) &&
                   (!shift || this.IsPressed(Key.LeftShift) || this.IsPressed(Key.RightShift)) &&
                   (!win || this.IsPressed(Key.LWin) || this.IsPressed(Key.RWin));
        }

        /// <inheritdoc />
        public bool IsPressed(MouseButton mouseButton)
        {
            return this.Mouse?.IsPressed(mouseButton) ?? false;
        }
    }
}