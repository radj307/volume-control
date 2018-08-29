using System;
using System.Windows.Forms;

namespace ToastifyAPI.Model.Interfaces
{
    /// <summary>
    ///     A global hotkey.
    /// </summary>
    public interface IGlobalHotkey
    {
        #region Public Properties

        bool Enabled { get; set; }
        Keys KeyCode { get; set; }
        bool Ctrl { get; set; }
        bool Alt { get; set; }
        bool Shift { get; set; }
        bool WindowsKey { get; set; }

        #endregion

        #region Events

        event EventHandler HotkeyPressed;

        #endregion
    }
}