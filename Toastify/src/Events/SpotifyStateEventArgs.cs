using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SpotifyStateEventArgs : EventArgs
    {
        #region Public Properties

        public Song CurrentSong { get; }
        public bool Playing { get; }
        public double TrackTime { get; }
        public double Volume { get; }

        #endregion

        public SpotifyStateEventArgs(Song currentSong, bool playing, double trackTime, double volume)
        {
            this.CurrentSong = currentSong;
            this.Playing = playing;
            this.TrackTime = trackTime;
            this.Volume = volume;
        }
    }
}