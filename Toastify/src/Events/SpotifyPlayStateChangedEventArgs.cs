using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SpotifyPlayStateChangedEventArgs : EventArgs
    {
        public bool Playing { get; }
        public Song CurrentSong { get; }

        public SpotifyPlayStateChangedEventArgs(bool playing, Song currentSong)
        {
            this.Playing = playing;
            this.CurrentSong = currentSong;
        }
    }
}