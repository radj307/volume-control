using System;

namespace Toastify.Events
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