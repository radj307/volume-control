using log4net;
using log4net.Util;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using Toastify.Core;

namespace Toastify.Model
{
    public class Song
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Song));

        private static readonly AlbumArtSize[] albumArtSizes = { AlbumArtSize.Size160, AlbumArtSize.Size320, AlbumArtSize.Size640 };

        public string Artist { get; }
        public string Track { get; set; }
        public string Album { get; }
        public int Length { get; }
        public string Type { get; }

        public string CoverArtUrl { get; set; }

        internal Song() : this(string.Empty, string.Empty, -1, null)
        {
        }

        public Song(string artist, string title, int length, string type, string album = null)
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

        public override string ToString()
        {
            if (this.Artist == null)
                return this.Track;

            return Settings.Current.ToastTitlesOrder == ToastTitlesOrder.TrackByArtist ?
                $"\x201C{this.Track}\x201D by {this.Artist}" :
                $"{this.Artist}: \x201C{this.Track}\x201D";
        }

        public override bool Equals(object obj)
        {
            var target = obj as Song;
            if (target == null)
                return false;

            return target.Type == this.Type &&
                target.Artist == this.Artist &&
                target.Track == this.Track &&
                target.Album == this.Album;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            int result = 17;
            result += result * 31 + this.Type?.GetHashCode() ?? 0;
            result += result * 31 + this.Artist?.GetHashCode() ?? 0;
            result += result * 31 + this.Track?.GetHashCode() ?? 0;
            result += result * 31 + this.Album?.GetHashCode() ?? 0;
            return result;
        }

        public static implicit operator Song(Track spotifyTrack)
        {
            if (spotifyTrack == null)
                return null;

            if (spotifyTrack.IsAd())
                return new Song();

            string artist = spotifyTrack.ArtistResource?.Name;
            string title = spotifyTrack.TrackResource?.Name;
            string album = spotifyTrack.AlbumResource?.Name;
            int length = spotifyTrack.Length;
            string type = spotifyTrack.TrackType;
            string coverArtUrl = string.Empty;

            if (spotifyTrack.AlbumResource != null)
            {
                // Take the smallest image possible.
                foreach (var size in albumArtSizes)
                {
                    try
                    {
                        coverArtUrl = spotifyTrack.GetAlbumArtUrl(size);
                    }
                    catch (Exception e)
                    {
                        logger.ErrorExt("Error while getting album art url (GetAlbumArtUrl).", e);
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