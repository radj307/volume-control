using ToastifyAPI.Core;

namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     Defines a hotkey that uses mouse buttons.
    /// </summary>
    public interface IMouseHookHotkey : IHotkey
    {
        #region Public Properties

        MouseAction? MouseButton { get; set; }

        #endregion
    }
}