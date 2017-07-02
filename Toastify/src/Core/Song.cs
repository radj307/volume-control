using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System.Diagnostics.CodeAnalysis;

namespace Toastify.Core
{
    public class Song
    {
        private static readonly AlbumArtSize[] albumArtSizes = { AlbumArtSize.Size160, AlbumArtSize.Size320, AlbumArtSize.Size640 };

        public string Artist { get; }
        public string Track { get; set; }
        public string Album { get; }

        public string CoverArtUrl { get; set; }

        public Song(string artist, string title, string album = null)
        {
            this.Artist = artist;
            this.Track = title;
            this.Album = album;
        }

        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Artist) || !string.IsNullOrEmpty(this.Track);
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
            return this.Artist == null ? this.Track : $"{this.Artist} – {this.Track}";
        }

        public override bool Equals(object obj)
        {
            var target = obj as Song;
            if (target == null)
                return false;

            return target.Artist == this.Artist && target.Track == this.Track;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return this.Artist.GetHashCode() ^ this.Track.GetHashCode();
        }

        public static implicit operator Song(Track spotifyTrack)
        {
            if (spotifyTrack == null)
                return null;

            if (spotifyTrack.IsAd())
                return new Song("", "");

            string artist = spotifyTrack.ArtistResource?.Name;
            string title = spotifyTrack.TrackResource?.Name;
            string album = spotifyTrack.AlbumResource?.Name;
            string coverArtUrl = "";

            // Take the smallest image possible.
            foreach (var size in albumArtSizes)
            {
                coverArtUrl = spotifyTrack.GetAlbumArtUrl(size);
                if (!string.IsNullOrWhiteSpace(coverArtUrl))
                    break;
            }

            return new Song(artist, title, album) { CoverArtUrl = coverArtUrl };
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