using System.Windows.Input;

namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     Defines a hotkey that uses keyboard keys.
    /// </summary>
    public interface IKeyboardHotkey : IHotkey
    {
        #region Public Properties

        Key? Key { get; set; }

        #endregion
    }
}