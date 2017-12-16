using System;
using Toastify.Model;

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

        public static implicit operator ToastifyAPI.Events.SpotifyTrackChangedEventArgs(SpotifyTrackChangedEventArgs e)
        {
            return new ToastifyAPI.Events.SpotifyTrackChangedEventArgs(e.NewSong.Artist, e.NewSong.Album, e.NewSong.Track);
        }
    }
}