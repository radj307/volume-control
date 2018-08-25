using System;

namespace Toastify.Events
{
    public class SpotifyVolumeChangedEventArgs : EventArgs
    {
        #region Public Properties

        public double PreviousVolume { get; }
        public double NewVolume { get; }

        #endregion

        public SpotifyVolumeChangedEventArgs(double previousVolume, double newVolume)
        {
            this.PreviousVolume = previousVolume;
            this.NewVolume = newVolume;
        }
    }
}