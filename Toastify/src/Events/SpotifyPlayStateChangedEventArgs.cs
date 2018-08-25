using System;

namespace Toastify.Events
{
    public class SpotifyPlayStateChangedEventArgs : EventArgs
    {
        #region Public Properties

        public bool Playing { get; }

        #endregion

        public SpotifyPlayStateChangedEventArgs(bool playing)
        {
            this.Playing = playing;
        }
    }
}