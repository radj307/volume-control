using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using System.Diagnostics.CodeAnalysis;

namespace Toastify.Core
{
    public class Song
    {
        private static readonly AlbumArtSize[] albumArtSizes = { AlbumArtSize.Size640, AlbumArtSize.Size320, AlbumArtSize.Size160 };

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

        public override string ToString()
        {
            return this.Artist == null ? this.Track : $"{this.Artist} – {this.Track}";
        }

        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Artist) || !string.IsNullOrEmpty(this.Track);
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
            string artist = spotifyTrack.ArtistResource.Name;
            string title = spotifyTrack.TrackResource.Name;
            string album = spotifyTrack.AlbumResource.Name;
            string coverArtUrl = "";

            foreach (var size in albumArtSizes)
            {
                coverArtUrl = spotifyTrack.GetAlbumArtUrl(size);
                if (!string.IsNullOrWhiteSpace(coverArtUrl))
                    break;
            }

            return new Song(artist, title, album) { CoverArtUrl = coverArtUrl };
        }
    }
}