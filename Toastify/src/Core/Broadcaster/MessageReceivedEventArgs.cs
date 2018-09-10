using System;

namespace Toastify.Core.Broadcaster
{
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Public Properties

        public string Message { get; }

        #endregion

        public MessageReceivedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}