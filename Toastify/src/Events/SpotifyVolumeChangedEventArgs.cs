using System;

namespace Toastify.Events
{
    public class SpotifyVolumeChangedEventArgs : EventArgs
    {
        public double PreviousVolume { get; }
        public double NewVolume { get; }

        public SpotifyVolumeChangedEventArgs(double previousVolume, double newVolume)
        {
            this.PreviousVolume = previousVolume;
            this.NewVolume = newVolume;
        }
    }
}