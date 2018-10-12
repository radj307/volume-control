using System;
using Toastify.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Events
{
    public class SpotifyStateEventArgs : EventArgs
    {
        #region Public Properties

        public ISpotifyTrack CurrentTrack { get; }
        public bool Playing { get; }
        public double TrackTime { get; }
        public double Volume { get; }

        #endregion

        public SpotifyStateEventArgs(ISpotifyTrack currentTrack, bool playing, double trackTime, double volume)
        {
            this.CurrentTrack = currentTrack;
            this.Playing = playing;
            this.TrackTime = trackTime;
            this.Volume = volume;
        }
    }
}