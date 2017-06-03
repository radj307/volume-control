using System;

namespace Toastify.Core
{
    internal class SpotifyTrackChangedEventArgs : EventArgs
    {
        public Song PreviousSong { get; }
        public Song NewSong { get; }

        public SpotifyTrackChangedEventArgs(Song previousSong, Song newSong)
        {
            this.PreviousSong = previousSong;
            this.NewSong = newSong;
        }

        public static implicit operator Plugin.SpotifyTrackChangedEventArgs(SpotifyTrackChangedEventArgs e)
        {
            return new Plugin.SpotifyTrackChangedEventArgs(e.NewSong.Artist, e.NewSong.Album, e.NewSong.Track);
        }
    }
}