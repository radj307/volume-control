using System;

namespace ToastifyAPI.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        #region Public Properties

        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }

        #endregion

        public SpotifyTrackChangedEventArgs(string artist, string album, string title)
        {
            this.Artist = artist;
            this.Album = album;
            this.Title = title;
        }
    }
}