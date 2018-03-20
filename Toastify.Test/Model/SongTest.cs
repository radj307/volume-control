using NUnit.Framework;
using SpotifyAPI.Local.Models;
using Toastify.Core;
using Toastify.Model;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(Song))]
    public class SongTest
    {
        [Test(Author = "aleab")]
        [TestCase(SpotifyTrackType.NORMAL), TestCase(SpotifyTrackType.AD), TestCase(SpotifyTrackType.OTHER)]
        public void TrackToSongImplicitOperatorTest(string type)
        {
            Track track = GetTestTrack(type);
            Song song = track;

            Assert.Multiple(() =>
            {
                switch (type)
                {
                    case "ad":
                        Assert.That(song.Album, Is.EqualTo(string.Empty));
                        Assert.That(song.Artist, Is.EqualTo(string.Empty));
                        Assert.That(song.Track, Is.EqualTo(Song.TITLE_SPOTIFY_AD));
                        Assert.That(song.Type, Is.EqualTo(track.TrackType));
                        Assert.That(song.Length, Is.EqualTo(track.Length));
                        break;

                    case "other":
                        Assert.That(song.Album, Is.EqualTo(string.Empty));
                        Assert.That(song.Artist, Is.EqualTo(string.Empty));
                        Assert.That(song.Track, Is.EqualTo(Song.TITLE_UNKNOWN));
                        Assert.That(song.Type, Is.EqualTo(track.TrackType));
                        Assert.That(song.Length, Is.EqualTo(track.Length));
                        break;

                    default:
                        Assert.That(song.Album, Is.EqualTo(track.AlbumResource.Name));
                        Assert.That(song.Artist, Is.EqualTo(track.ArtistResource.Name));
                        Assert.That(song.Track, Is.EqualTo(track.TrackResource.Name));
                        Assert.That(song.Type, Is.EqualTo(track.TrackType));
                        Assert.That(song.Length, Is.EqualTo(track.Length));
                        break;
                }
            });
        }

        private static Track GetTestTrack(string type)
        {
            return new Track
            {
                AlbumResource = new SpotifyResource
                {
                    Location = new TrackResourceLocation { Og = string.Empty },
                    Name = "Test Album",
                    Uri = "http://test.album"
                },
                ArtistResource = new SpotifyResource
                {
                    Location = new TrackResourceLocation { Og = string.Empty },
                    Name = "Test Artist",
                    Uri = "http://test.artist"
                },
                TrackResource = new SpotifyResource
                {
                    Location = new TrackResourceLocation { Og = string.Empty },
                    Name = "Test Track",
                    Uri = "http://test.track"
                },
                TrackType = type,
                Length = 60
            };
        }
    }
}