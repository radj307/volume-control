using System;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        #region Public Properties

        public ISong PreviousSong { get; }
        public ISong NewSong { get; }

        #endregion

        public SpotifyTrackChangedEventArgs(ISong previousSong, ISong newSong)
        {
            this.PreviousSong = previousSong;
            this.NewSong = newSong;
        }
    }
}