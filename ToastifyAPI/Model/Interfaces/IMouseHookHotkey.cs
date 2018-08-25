using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IMouseHookHotkey : IHotkey
    {
        #region Public Properties

        MouseAction? MouseButton { get; set; }

        #endregion
    }
}