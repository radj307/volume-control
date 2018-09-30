using System;
using System.Collections.Generic;
using System.Linq;

namespace ToastifyAPI.Events
{
    public class SpotifyTrackChangedEventArgs : EventArgs
    {
        #region Public Properties

        public IReadOnlyList<string> Artists { get; }
        public string Album { get; }
        public string Title { get; }

        #endregion

        public SpotifyTrackChangedEventArgs(string album, string artist, string title) : this(album, new List<string>(1) { artist }, title)
        {
        }

        public SpotifyTrackChangedEventArgs(string album, IEnumerable<string> artists, string title)
        {
            this.Artists = artists.Where(artist => !string.IsNullOrWhiteSpace(artist)).ToList();
            this.Album = album;
            this.Title = title;
        }
    }
}