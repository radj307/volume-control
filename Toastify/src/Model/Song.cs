using log4net;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System;
using Toastify.Core;

namespace Toastify.Model
{
    public sealed class Song : IEquatable<Song>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Song));

        private static readonly AlbumArtSize[] albumArtSizes = { AlbumArtSize.Size160, AlbumArtSize.Size320, AlbumArtSize.Size640 };

        internal const string TITLE_SPOTIFY_AD = "Spotify Ad";
        internal const string TITLE_UNKNOWN = "[Unknown Track Type]";

        public string Artist { get; }
        public string Track { get; }
        public string Album { get; }
        public int Length { get; }
        public string Type { get; }

        public string CoverArtUrl { get; set; }

        private Song(string title, int length, string type) : this(string.Empty, title, length, type, string.Empty)
        {
        }

        public Song(string artist, string title, int length, string type, string album)
        {
            this.Artist = artist;
            this.Track = title;
            this.Album = album;
            this.Length = length;
            this.Type = type;
        }

        public bool IsAd()
        {
            return this.Type == SpotifyTrackType.AD || this.Length == 0;
        }

        public bool IsOtherTrackType()
        {
            return this.Type == SpotifyTrackType.OTHER;
        }

        internal bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Type))
                return false;

            switch (this.Type)
            {
                case SpotifyTrackType.NORMAL:
                    return !string.IsNullOrEmpty(this.Artist) && !string.IsNullOrEmpty(this.Track);

                case SpotifyTrackType.OTHER:
                    return this.Length > 0;

                default:
                    return this.Length >= 0;
            }
        }

        public string GetClipboardText(string template)
        {
            string trackBeforeAction = this.ToString();

            // if the string is empty we set it to {0}
            if (string.IsNullOrWhiteSpace(template))
                template = "{0}";

            // add the song name to the end of the template if the user forgot to put in the
            // replacement marker
            if (!template.Contains("{0}"))
                template += " {0}";

            return string.Format(template, trackBeforeAction);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this.Artist == null)
                return this.Track;

            return Settings.Current.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist
                ? $"\x201C{this.Track}\x201D by {this.Artist}"
                : $"{this.Artist}: \x201C{this.Track}\x201D";
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (this.Artist != null ? this.Artist.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Track != null ? this.Track.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Album != null ? this.Album.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Length;
                hashCode = (hashCode * 397) ^ (this.Type != null ? this.Type.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            Song that = obj as Song;
            return this.Equals(that);
        }

        /// <inheritdoc />
        public bool Equals(Song other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(this.Artist, other.Artist) &&
                   string.Equals(this.Track, other.Track) &&
                   string.Equals(this.Album, other.Album) &&
                   this.Length == other.Length &&
                   string.Equals(this.Type, other.Type);
        }

        public static implicit operator Song(Track spotifyTrack)
        {
            if (spotifyTrack == null)
                return null;

            if (spotifyTrack.IsAd())
                return new Song(TITLE_SPOTIFY_AD, spotifyTrack.Length, SpotifyTrackType.AD);

            if (spotifyTrack.IsOtherTrackType())
                return new Song(TITLE_UNKNOWN, spotifyTrack.Length, SpotifyTrackType.OTHER);

            string artist = spotifyTrack.ArtistResource?.Name;
            string title = spotifyTrack.TrackResource?.Name;
            string album = spotifyTrack.AlbumResource?.Name;
            int length = spotifyTrack.Length;
            string type = spotifyTrack.TrackType;
            string coverArtUrl = null;

            if (spotifyTrack.AlbumResource != null)
            {
                // Take the smallest image possible.
                foreach (var size in albumArtSizes)
                {
                    try
                    {
                        coverArtUrl = spotifyTrack.GetAlbumArtUrl(size, App.ProxyConfig.ProxyConfig);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error while getting album art url", e);
                    }
                    if (!string.IsNullOrWhiteSpace(coverArtUrl))
                        break;
                }
            }

            return new Song(artist, title, length, type, album) { CoverArtUrl = coverArtUrl };
        }

        public static bool Equal(Song s1, Song s2)
        {
            if (ReferenceEquals(s1, s2))
                return true;
            if (s1 == null || s2 == null)
                return false;

            return s1.Equals(s2);
        }
    }
}