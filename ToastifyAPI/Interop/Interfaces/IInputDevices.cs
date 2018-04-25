using System.Windows.Input;

namespace ToastifyAPI.Interop.Interfaces
{
    public interface IInputDevices
    {
        IKeyboard Keyboard { get; set; }

        IMouse Mouse { get; set; }

        bool IsPressed(Key key);

        bool ArePressed(ModifierKeys modifiers);

        bool IsPressed(MouseButton mouseButton);
    }
}