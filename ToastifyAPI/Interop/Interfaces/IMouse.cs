using System.Windows.Input;

namespace ToastifyAPI.Interop.Interfaces
{
    /// <summary>
    ///     A wrapper around the <see cref="MouseDevice" /> class.
    /// </summary>
    public interface IMouse : IInputDevice
    {
        bool IsPressed(MouseButton mouseButton);

        bool IsReleased(MouseButton mouseButton);
    }
}