using System;
using JetBrains.Annotations;
using log4net;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using Toastify.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public sealed class Song : ISong
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Song));

        internal const string TITLE_SPOTIFY_AD = "Spotify Ad";
        internal const string TITLE_UNKNOWN = "[Unknown Track Type]";

        private static readonly AlbumArtSize[] albumArtSizes = { AlbumArtSize.Size160, AlbumArtSize.Size320, AlbumArtSize.Size640 };

        private readonly Track spotifyTrack;
        private string _coverArtUrl;

        #region Public properties

        public string Artist { get; }
        public string Track { get; }
        public string Album { get; }
        public int Length { get; }
        public string Type { get; }

        public string CoverArtUrl
        {
            get
            {
                // Lazy fetch of CoverArtUrl
                if (this._coverArtUrl == null)
                    this._coverArtUrl = this.GetSmallestCoverArtUrl();

                return this._coverArtUrl ?? string.Empty;
            }
            set { this._coverArtUrl = value; }
        }

        #endregion

        public Song(string artist, string title, int length, string type, string album)
        {
            this.Artist = artist;
            this.Track = title;
            this.Album = album;
            this.Length = length;
            this.Type = type;
        }

        public Song([NotNull] Track track)
        {
            this.spotifyTrack = track ?? throw new ArgumentNullException(nameof(track));

            if (track.IsAd())
            {
                this.Album = string.Empty;
                this.Artist = string.Empty;
                this.Track = TITLE_SPOTIFY_AD;
                this.Length = track.Length;
                this.Type = SpotifyTrackType.AD;
            }
            else if (track.IsOtherTrackType())
            {
                this.Album = string.Empty;
                this.Artist = string.Empty;
                this.Track = TITLE_UNKNOWN;
                this.Length = track.Length;
                this.Type = SpotifyTrackType.OTHER;
            }
            else
            {
                this.Album = track.AlbumResource?.Name ?? string.Empty;
                this.Artist = track.ArtistResource?.Name ?? string.Empty;
                this.Track = track.TrackResource?.Name ?? string.Empty;
                this.Length = track.Length;
                this.Type = track.TrackType;
            }
        }

        public bool IsAd()
        {
            return this.spotifyTrack?.IsAd() ?? this.Type == SpotifyTrackType.AD || this.Length == 0;
        }

        public bool IsOtherTrackType()
        {
            return this.spotifyTrack?.IsOtherTrackType() ?? this.Type == SpotifyTrackType.OTHER;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Type))
                return false;

            switch (this.Type)
            {
                case SpotifyTrackType.NORMAL:
                    return !string.IsNullOrEmpty(this.Album) &&
                           !string.IsNullOrEmpty(this.Artist) &&
                           !string.IsNullOrEmpty(this.Track) &&
                           this.Length > 0;

                case SpotifyTrackType.OTHER:
                    return this.Length > 0;

                case SpotifyTrackType.AD:
                    return this.Length >= 0;

                default:
                    logger.Warn($"Invalid Song type: \"{this.Type}\"");
                    return false;
            }
        }

        public string GetCoverArtUrl(AlbumArtSize size)
        {
            string url = string.Empty;
            try
            {
                if (this.spotifyTrack?.AlbumResource != null)
                    url = this.spotifyTrack.GetAlbumArtUrl(size, App.ProxyConfig.ProxyConfig);
            }
            catch (Exception e)
            {
                logger.Error("Error while getting album art url", e);
            }

            return url;
        }

        public string GetSmallestCoverArtUrl()
        {
            string url = string.Empty;
            foreach (AlbumArtSize size in albumArtSizes)
            {
                url = this.GetCoverArtUrl(size);
                if (!string.IsNullOrWhiteSpace(url))
                    break;
            }

            return url ?? string.Empty;
        }

        public string GetClipboardText(string template)
        {
            // TODO: GetClipboardText(string) doesn't belong here; move it somewhere else and change it to GetClipboardText(Song,string)!

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
            // TODO: De-couple this ToString() from Settings

            if (string.IsNullOrWhiteSpace(this.Artist))
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
                int hashCode = this.Album.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Artist.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Track.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Length;
                hashCode = (hashCode * 397) ^ this.Type.GetHashCode();
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

            var that = obj as Song;
            return this.Equals(that);
        }

        /// <inheritdoc />
        public bool Equals(ISong other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(this.Album, other.Album) &&
                   string.Equals(this.Artist, other.Artist) &&
                   string.Equals(this.Track, other.Track) &&
                   this.Length == other.Length &&
                   string.Equals(this.Type, other.Type);
        }

        public static bool Equal(Song s1, Song s2)
        {
            if (ReferenceEquals(s1, s2))
                return true;
            if (s1 == null || s2 == null)
                return false;

            return s1.Equals(s2);
        }

        public static implicit operator Song(Track spotifyTrack)
        {
            return spotifyTrack == null ? null : new Song(spotifyTrack);
        }
    }
}