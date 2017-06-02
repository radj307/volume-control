using System;

namespace Toastify.Core
{
    public class SpotifyTrackTimeChangedEventArgs : EventArgs
    {
        public double TrackTime { get; }

        public SpotifyTrackTimeChangedEventArgs(double trackTime)
        {
            this.TrackTime = trackTime;
        }
    }
}