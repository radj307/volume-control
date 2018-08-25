using System;

namespace Toastify.Events
{
    public class SpotifyTrackTimeChangedEventArgs : EventArgs
    {
        #region Public Properties

        public double TrackTime { get; }

        #endregion

        public SpotifyTrackTimeChangedEventArgs(double trackTime)
        {
            this.TrackTime = trackTime;
        }
    }
}