using System.Windows.Input;

namespace ToastifyAPI.Interop.Interfaces
{
    public interface IInputDevices
    {
        #region Public Properties

        IKeyboard Keyboard { get; set; }

        IMouse Mouse { get; set; }

        #endregion

        bool IsPressed(Key key);

        bool ArePressed(ModifierKeys modifiers);

        bool IsPressed(MouseButton mouseButton);
    }
}