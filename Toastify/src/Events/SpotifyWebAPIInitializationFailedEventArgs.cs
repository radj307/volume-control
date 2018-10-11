using System;
using Toastify.Core;

namespace Toastify.Events
{
    public class SpotifyWebAPIInitializationFailedEventArgs : EventArgs
    {
        #region Public Properties

        public SpotifyWebAPIInitializationFailedReason Reason { get; }

        #endregion

        public SpotifyWebAPIInitializationFailedEventArgs(SpotifyWebAPIInitializationFailedReason reason)
        {
            this.Reason = reason;
        }
    }
}