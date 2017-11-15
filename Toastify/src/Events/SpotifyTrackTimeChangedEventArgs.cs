using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SpotifyTrackTimeChangedEventArgs : EventArgs
    {
        public double TrackTime { get; }
        public Song CurrentSong { get; }
        public bool Playing { get; }

        public SpotifyTrackTimeChangedEventArgs(double trackTime, Song currentSong, bool playing)
        {
            this.TrackTime = trackTime;
            this.CurrentSong = currentSong;
            this.Playing = playing;
        }
    }
}