using System;

namespace Toastify.Core
{
    public class SpotifyPlayStateChangedEventArgs : EventArgs
    {
        public bool Playing { get; }

        public SpotifyPlayStateChangedEventArgs(bool playing)
        {
            this.Playing = playing;
        }
    }
}