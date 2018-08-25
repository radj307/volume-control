using System.Windows.Input;

namespace ToastifyAPI.Model.Interfaces
{
    public interface IKeyboardHotkey : IHotkey
    {
        #region Public Properties

        Key? Key { get; set; }

        #endregion
    }
}