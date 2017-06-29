using System;

namespace ToastifyAPI.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }

        public SpotifyTrackChangedEventArgs(string artist, string album, string title)
        {
            this.Artist = artist;
            this.Album = album;
            this.Title = title;
        }
    }
}