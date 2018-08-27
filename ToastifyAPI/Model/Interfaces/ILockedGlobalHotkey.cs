using System.Windows.Forms;

namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     A global hotkey that can't be modified.
    /// </summary>
    public interface ILockedGlobalHotkey
    {
        #region Public Properties

        bool Enabled { get; }
        Keys KeyCode { get; }
        bool Ctrl { get; }
        bool Alt { get; }
        bool Shift { get; }
        bool WindowsKey { get; }

        #endregion
    }
}