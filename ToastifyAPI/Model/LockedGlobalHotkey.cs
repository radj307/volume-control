using System.Windows.Forms;
using ToastifyAPI.Model.Interfaces;

namespace ToastifyAPI.Model
{
    /// <summary>
    ///     A read-only global hotkey.
    /// </summary>
    public class LockedGlobalHotkey : ILockedGlobalHotkey
    {
        #region Public Properties

        public bool Enabled { get; }

        public Keys KeyCode { get; }

        public bool Ctrl { get; }

        public bool Alt { get; }

        public bool Shift { get; }

        public bool WindowsKey { get; }

        #endregion

        public LockedGlobalHotkey(IGlobalHotkey globalHotkey)
        {
            this.Enabled = globalHotkey?.Enabled ?? false;
            this.KeyCode = globalHotkey?.KeyCode ?? Keys.None;
            this.Ctrl = globalHotkey?.Ctrl ?? false;
            this.Alt = globalHotkey?.Alt ?? false;
            this.Shift = globalHotkey?.Shift ?? false;
            this.WindowsKey = globalHotkey?.WindowsKey ?? false;
        }
    }
}