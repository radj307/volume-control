using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class HotkeyActionCallbackFailedEventArgs : EventArgs
    {
        public Hotkey Hotkey { get; private set; }

        public Exception Exception { get; private set; }

        public HotkeyActionCallbackFailedEventArgs(Hotkey hotkey, Exception exception)
        {
            this.Hotkey = hotkey;
            this.Exception = exception;
        }
    }
}