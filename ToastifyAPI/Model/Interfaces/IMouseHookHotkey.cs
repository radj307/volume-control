using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IMouseHookHotkey : IHotkey
    {
        MouseAction? MouseButton { get; set; }
    }
}