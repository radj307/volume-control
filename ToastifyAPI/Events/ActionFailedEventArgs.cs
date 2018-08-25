using System;

namespace ToastifyAPI.Events
{
    public class ActionFailedEventArgs : EventArgs
    {
        #region Public Properties

        public string Message { get; }

        #endregion

        public ActionFailedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}