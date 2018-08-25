using System;
using SpotifyAPI.Local.Models;
using Toastify.Model;

namespace Toastify.Events
{
    public class SpotifyStateEventArgs : EventArgs
    {
        #region Public Properties

        public Song CurrentSong { get; }
        public bool Playing { get; }
        public double TrackTime { get; }
        public double Volume { get; }

        #endregion

        public SpotifyStateEventArgs(Song currentSong, bool playing, double trackTime, double volume)
        {
            this.CurrentSong = currentSong;
            this.Playing = playing;
            this.TrackTime = trackTime;
            this.Volume = volume;
        }

        public SpotifyStateEventArgs(StatusResponse spotifyStatus) : this(spotifyStatus?.Track, spotifyStatus?.Playing ?? false, spotifyStatus?.PlayingPosition ?? -1, spotifyStatus?.Volume ?? -1)
        {
        }
    }
}