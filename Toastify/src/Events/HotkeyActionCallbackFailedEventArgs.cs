using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class HotkeyActionCallbackFailedEventArgs : EventArgs
    {
        #region Public Properties

        public Hotkey Hotkey { get; }

        public Exception Exception { get; }

        #endregion

        public HotkeyActionCallbackFailedEventArgs(Hotkey hotkey, Exception exception)
        {
            this.Hotkey = hotkey;
            this.Exception = exception;
        }
    }
}