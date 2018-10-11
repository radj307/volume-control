using System;
using Toastify.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Events
{
    public class SpotifyStateEventArgs : EventArgs
    {
        #region Public Properties

        public ISong CurrentSong { get; }
        public bool Playing { get; }
        public double TrackTime { get; }
        public double Volume { get; }

        #endregion

        public SpotifyStateEventArgs(ISong currentSong, bool playing, double trackTime, double volume)
        {
            this.CurrentSong = currentSong;
            this.Playing = playing;
            this.TrackTime = trackTime;
            this.Volume = volume;
        }
    }
}