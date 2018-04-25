using System;

namespace ToastifyAPI.Events
{
    public class ActionFailedEventArgs : EventArgs
    {
        public string Message { get; }

        public ActionFailedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}