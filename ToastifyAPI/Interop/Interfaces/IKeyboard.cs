using System.Windows.Input;

namespace ToastifyAPI.Interop.Interfaces
{
    /// <summary>
    ///     A wrapper around the <see cref="KeyboardDevice" /> class.
    /// </summary>
    public interface IKeyboard : IInputDevice
    {
        bool IsKeyDown(Key key);

        bool IsKeyUp(Key key);
    }
}