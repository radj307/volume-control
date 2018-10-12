using System;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        #region Public Properties

        public ISpotifyTrack PreviousTrack { get; }
        public ISpotifyTrack NewTrack { get; }

        #endregion

        public SpotifyTrackChangedEventArgs(ISpotifyTrack previousTrack, ISpotifyTrack newTrack)
        {
            this.PreviousTrack = previousTrack;
            this.NewTrack = newTrack;
        }
    }
}