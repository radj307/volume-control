using System;
using Toastify.Model;

namespace Toastify.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        #region Public Properties

        public Song PreviousSong { get; }
        public Song NewSong { get; }

        #endregion

        public SpotifyTrackChangedEventArgs(Song previousSong, Song newSong)
        {
            this.PreviousSong = previousSong;
            this.NewSong = newSong;
        }

        #region Static Members

        public static implicit operator ToastifyAPI.Events.SpotifyTrackChangedEventArgs(SpotifyTrackChangedEventArgs e)
        {
            return new ToastifyAPI.Events.SpotifyTrackChangedEventArgs(e.NewSong.Artist, e.NewSong.Album, e.NewSong.Track);
        }

        #endregion
    }
}