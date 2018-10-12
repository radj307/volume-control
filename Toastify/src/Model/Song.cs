using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using log4net;
using SpotifyAPI.Web.Models;
using Toastify.Core;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public sealed class Song : SpotifyTrack, ISong
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Song));

        private readonly FullTrack spotifyTrack;
        private ISongAlbumArt _albumArt;

        #region Public Properties

        public string Album { get; }

        public IReadOnlyList<string> Artists { get; }

        public ISongAlbumArt AlbumArt
        {
            get
            {
                // Lazy fetch of CoverArtUrl
                if (this._albumArt == null)
                {
                    var smallestImage = this.spotifyTrack?.Album?.Images?.Last();
                    this._albumArt = smallestImage != null ? new SongAlbumArt(smallestImage) : SongAlbumArt.Empty;
                }

                return this._albumArt;
            }
            set { this._albumArt = value; }
        }

        #endregion

        public Song(string album, string artist, string title, int length) : this(album, new List<string>(1) { artist }, title, length)
        {
        }

        public Song(string album, IEnumerable<string> artists, string title, int length) : base(SpotifyTrackType.Song, title, length)
        {
            this.Artists = artists?.Where(artist => !string.IsNullOrWhiteSpace(artist)).ToList() ?? new List<string>(0);
            this.Album = album;
        }

        // ReSharper disable ConstantConditionalAccessQualifier
        // ReSharper disable once ConstantNullCoalescingCondition
        public Song([NotNull] FullTrack track) : base(SpotifyTrackType.Song, track?.Name, (track?.DurationMs ?? 0) / 1000)
        {
            this.spotifyTrack = track ?? throw new ArgumentNullException(nameof(track));
            this.Album = track.Album?.Name ?? string.Empty;
            this.Artists = track.Artists?.Select(a => a.Name).ToList() ?? new List<string>(0);
        }
        // ReSharper restore ConstantConditionalAccessQualifier

        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrEmpty(this.Album) && this.Artists.Count > 0;
        }

        public override string GetClipboardText(string template)
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

            if (this.Artists.Count <= 0)
                return this.Title ?? string.Empty;

            string artists = string.Join(", ", this.Artists);
            return Settings.Current.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist
                ? $"\x201C{this.Title}\x201D by {artists}"
                : $"{artists}: \x201C{this.Title}\x201D";
        }

        #region Equals / GetHashCode

        /// <inheritdoc />
        public bool Equals(ISong other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return base.Equals(other) &&
                   string.Equals(this.Album, other.Album) &&
                   this.Artists.All(artist => other.Artists.Contains(artist)) &&
                   other.Artists.All(artist => this.Artists.Contains(artist));
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return base.Equals(obj) && this.Equals(obj as Song);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Album.GetHashCode();
                hashCode = this.Artists.Aggregate(hashCode, (acc, artist) => (acc * 397) ^ artist.GetHashCode());
                return hashCode;
            }
        }

        #endregion

        #region Static Members

        public static Song FromSpotifyWindowTitle([NotNull] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            Song song = null;
            string[] newTitleElements = title.Split('-');
            if (newTitleElements.Length < 2)
            {
                // TODO: Handle unexpected title format
                // This can be an episode of a podcast
                logger.Warn($"Unexpected title format: \"{title}\"");
            }
            else if (newTitleElements.Length > 2)
            {
                // Either or both the song title or/and the artist name contain a "-".
                // Either or both of them can contain compound words with hyphens: these hyphens should be ignored when separating the string!
                // Let's assume that only the song title contains hyphens surrounded by spaces!
                var match = Regex.Match(title, @"^((?:[^-]+)|(?:.*?\b-\b.*?)) - (.*)$", RegexOptions.Compiled);
                song = new Song("Unknown Album", match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim(), 0);
            }
            else
                song = new Song("Unknown Album", newTitleElements[0].Trim(), newTitleElements[1].Trim(), 0);

            return song;
        }

        public static bool Equal(ISong s1, ISong s2)
        {
            if (ReferenceEquals(s1, s2))
                return true;
            if (s1 == null || s2 == null)
                return false;

            return s1.Equals((object)s2);
        }

        public static implicit operator Song(FullTrack spotifyTrack)
        {
            return spotifyTrack == null ? null : new Song(spotifyTrack);
        }

        #endregion
    }
}