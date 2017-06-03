using SpotifyAPI.Local.Models;
using System;
using Toastify.Core;

namespace Toastify.Events
{
    internal class SpotifyStateEventArgs : EventArgs
    {
        public Song CurrentSong { get; }
        public bool Playing { get; }
        public double TrackTime { get; }
        public double Volume { get; }

        public SpotifyStateEventArgs(Song currentSong, bool playing, double trackTime, double volume)
        {
            this.CurrentSong = currentSong;
            this.Playing = playing;
            this.TrackTime = trackTime;
            this.Volume = volume;
        }

        public SpotifyStateEventArgs(StatusResponse spotifyStatus) : this(spotifyStatus.Track, spotifyStatus.Playing, spotifyStatus.PlayingPosition, spotifyStatus.Volume)
        {
        }
    }
}