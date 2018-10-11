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

        #region Static Members

        public static implicit operator ToastifyAPI.Events.SpotifyTrackChangedEventArgs(SpotifyTrackChangedEventArgs e)
        {
            return new ToastifyAPI.Events.SpotifyTrackChangedEventArgs(e.NewSong.Album, e.NewSong.Artists, e.NewSong.Title);
        }

        #endregion
    }
}