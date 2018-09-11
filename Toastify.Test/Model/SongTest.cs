using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using SpotifyAPI.Local.Models;
using Toastify.Core;
using Toastify.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(Song))]
    public class SongTest
    {
        // TODO: Unit test GetAlbumArtUrl
        //       This will be quite tricky to unit test, since it has two external, non-mockable dependencies:
        //       Track and WebClient

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.IsAdTestCases))]
        public bool IsAdTest(Song song)
        {
            return song.IsAd();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.IsOtherTrackTypeTestCases))]
        public bool IsOtherTrackTypeTest(Song song)
        {
            return song.IsOtherTrackType();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.IsValidTestCases))]
        public bool IsValidTest(Song song)
        {
            return song.IsValid();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.FromSpotifyWindowTitleTestCases))]
        public void TestFromSpotifyWindowTitle(string title, Action<Song> test)
        {
            Song song = Song.FromSpotifyWindowTitle(title);
            test?.Invoke(song);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.TrackTestCases))]
        public void ImplicitCastToSong_Track(Track track)
        {
            Song song = track;

            if (track == null)
                Assert.That(song, Is.Null);
            else
            {
                ISong mockedSong = A.Fake<ISong>(options => options.Wrapping(song));
                A.CallTo(() => mockedSong.CoverArtUrl).Returns(string.Empty);

                Assert.Multiple(() =>
                {
                    switch (track.TrackType)
                    {
                        case SpotifyTrackType.AD:
                            Assert.That(mockedSong.Album, Is.EqualTo(string.Empty).Or.EqualTo(null));
                            Assert.That(mockedSong.Artist, Is.EqualTo(string.Empty).Or.EqualTo(null));
                            Assert.That(mockedSong.Track, Is.EqualTo(Song.TITLE_SPOTIFY_AD));
                            Assert.That(mockedSong.Type, Is.EqualTo(track.TrackType));
                            Assert.That(mockedSong.Length, Is.EqualTo(track.Length));
                            break;

                        case SpotifyTrackType.OTHER:
                            Assert.That(mockedSong.Album, Is.EqualTo(string.Empty).Or.EqualTo(null));
                            Assert.That(mockedSong.Artist, Is.EqualTo(string.Empty).Or.EqualTo(null));
                            Assert.That(mockedSong.Track, Is.EqualTo(Song.TITLE_UNKNOWN));
                            Assert.That(mockedSong.Type, Is.EqualTo(track.TrackType));
                            Assert.That(mockedSong.Length, Is.EqualTo(track.Length));
                            break;

                        default:
                            Assert.That(mockedSong.Album, Is.EqualTo(track.AlbumResource.Name));
                            Assert.That(mockedSong.Artist, Is.EqualTo(track.ArtistResource.Name));
                            Assert.That(mockedSong.Track, Is.EqualTo(track.TrackResource.Name));
                            Assert.That(mockedSong.Type, Is.EqualTo(track.TrackType));
                            Assert.That(mockedSong.Length, Is.EqualTo(track.Length));
                            break;
                    }
                });
            }
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.EqualsTestCases))]
        public bool EqualsTest(Song s1, Song s2)
        {
            if (s1 != null)
                return s1.Equals(s2);
            return s2 == null || s2.Equals(null);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.ObjectEqualsTestCases))]
        public bool EqualsTest_Object(Song s, object obj)
        {
            if (s != null)
                return s.Equals(obj);
            return obj == null || obj.Equals(null);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.StaticEqualTestCases))]
        public bool EqualTest_Static(Song s1, Song s2)
        {
            return Song.Equal(s1, s2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.GetHashCodeTestCases))]
        public bool GetHashCodeTest(Song s1, Song s2)
        {
            return s1?.GetHashCode() == s2?.GetHashCode();
        }

        #region Test Cases

        public class SongAndTrackData
        {
            #region Static Fields and Properties

            public static IEnumerable<TestCaseData> TrackTestCases
            {
                get
                {
                    // Normal track
                    yield return new TestCaseData(NormalTrack).SetName("NORMAL Track");

                    // Ad tracks
                    yield return new TestCaseData(AdPositiveLengthTrack).SetName("AD Track: >");
                    yield return new TestCaseData(AdZeroLengthTrack).SetName("AD Track: 0");

                    // Other track
                    yield return new TestCaseData(OtherTrack).SetName("OTHER Track");

                    // Null track
                    yield return new TestCaseData(null).SetName("null Track");
                }
            }

            public static IEnumerable<TestCaseData> IsAdTestCases
            {
                get
                {
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.AD, string.Empty)).Returns(true).SetName("AD");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.OTHER, string.Empty)).Returns(false).SetName("OTHER");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, string.Empty, string.Empty)).Returns(false).SetName("empty type");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, "invalid-type", string.Empty)).Returns(false).SetName("invalid type");

                    // A zero-length track is considered an AD regardless of its declared type
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, SpotifyTrackType.NORMAL, string.Empty)).Returns(true).SetName("NORMAL: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, SpotifyTrackType.AD, string.Empty)).Returns(true).SetName("AD: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, SpotifyTrackType.OTHER, string.Empty)).Returns(true).SetName("OTHER: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, string.Empty, string.Empty)).Returns(true).SetName("empty type: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, "invalid-type", string.Empty)).Returns(true).SetName("invalid type: 0");

                    // Songs from Tracks
                    yield return new TestCaseData(new Song(NormalTrack)).Returns(false).SetName("NORMAL Track");
                    yield return new TestCaseData(new Song(AdPositiveLengthTrack)).Returns(true).SetName("AD Track: >");
                    yield return new TestCaseData(new Song(AdZeroLengthTrack)).Returns(true).SetName("AD Track: 0");
                    yield return new TestCaseData(new Song(OtherTrack)).Returns(false).SetName("OTHER Track");
                }
            }

            public static IEnumerable<TestCaseData> IsOtherTrackTypeTestCases
            {
                get
                {
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.AD, string.Empty)).Returns(false).SetName("AD");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.OTHER, string.Empty)).Returns(true).SetName("OTHER");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, string.Empty, string.Empty)).Returns(false).SetName("empty type");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, "invalid-type", string.Empty)).Returns(false).SetName("invalid type");

                    // Songs from Tracks
                    yield return new TestCaseData(new Song(NormalTrack)).Returns(false).SetName("NORMAL Track");
                    yield return new TestCaseData(new Song(AdPositiveLengthTrack)).Returns(false).SetName("AD Track: >");
                    yield return new TestCaseData(new Song(AdZeroLengthTrack)).Returns(false).SetName("AD Track: 0");
                    yield return new TestCaseData(new Song(OtherTrack)).Returns(true).SetName("OTHER Track");
                }
            }

            public static IEnumerable<TestCaseData> IsValidTestCases
            {
                get
                {
                    // NORMAL songs must have a non-null & non-empty Album, Artist and Track, the NORMAL type and a positive length.
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(true).SetName("NORMAL: valid");

                    yield return new TestCaseData(new Song(string.Empty, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL: artist");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL: artist,title");
                    yield return new TestCaseData(new Song(string.Empty, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, string.Empty)).Returns(false).SetName("NORMAL: artist,album");
                    yield return new TestCaseData(new Song(TestArtistResource.Name, string.Empty, 60, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL: title");
                    yield return new TestCaseData(new Song(TestArtistResource.Name, string.Empty, 60, SpotifyTrackType.NORMAL, string.Empty)).Returns(false).SetName("NORMAL: title,album");
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 60, SpotifyTrackType.NORMAL, string.Empty)).Returns(false).SetName("NORMAL: album");

                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, -1, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL: <");
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 0, SpotifyTrackType.NORMAL, TestAlbumResource.Name)).Returns(false).SetName("NORMAL: 0");

                    // AD tracks must have the AD type and have a non-negative length
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, -1, SpotifyTrackType.AD, string.Empty)).Returns(false).SetName("AD: <");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, SpotifyTrackType.AD, string.Empty)).Returns(true).SetName("AD: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.AD, string.Empty)).Returns(true).SetName("AD: >");

                    // OTHER tracks must have the OTHER type and a positive length
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, -1, SpotifyTrackType.OTHER, string.Empty)).Returns(false).SetName("OTHER: <");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 0, SpotifyTrackType.OTHER, string.Empty)).Returns(false).SetName("OTHER: 0");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, 60, SpotifyTrackType.OTHER, string.Empty)).Returns(true).SetName("OTHER: >");

                    // If the Song has an unknown type, then it's invalid
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 0, string.Empty, TestAlbumResource.Name)).Returns(false).SetName("empty type");
                    yield return new TestCaseData(new Song(TestArtistResource.Name, TestTrackResource.Name, 0, "invalid-type", TestAlbumResource.Name)).Returns(false).SetName("invalid type");

                    // Songs from valid Tracks
                    yield return new TestCaseData(new Song(NormalTrack)).Returns(true).SetName("NORMAL Track");
                    yield return new TestCaseData(new Song(AdPositiveLengthTrack)).Returns(true).SetName("AD Track: >");
                    yield return new TestCaseData(new Song(AdZeroLengthTrack)).Returns(true).SetName("AD Track: 0");
                    yield return new TestCaseData(new Song(OtherTrack)).Returns(true).SetName("OTHER Track");
                }
            }

            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    Song normal = new Song(NormalTrack);
                    Song ad = new Song(AdPositiveLengthTrack);
                    Song other1 = new Song(OtherTrack);
                    Song other2 = new Song(other1.Artist, other1.Track, other1.Length * 2, other1.Type, other1.Album);

                    // with null
                    yield return new TestCaseData(normal, null).Returns(false).SetName("with null");

                    // same
                    yield return new TestCaseData(normal, normal).Returns(true).SetName("same");

                    // equal
                    yield return new TestCaseData(normal, new Song(NormalTrack)).Returns(true).SetName("equal");
                    yield return new TestCaseData(other1, new Song(OtherTrack)).Returns(true).SetName("equal: OTHER tracks with same length");

                    // not equal
                    yield return new TestCaseData(normal, ad).Returns(false).SetName("not equal");
                    yield return new TestCaseData(other1, other2).Returns(false).SetName("not equal: OTHER tracks with different length");

                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("different artist", "title", 60, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different artist");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "different title", 60, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different title");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "different album")).Returns(false).SetName("not equal: different album");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 30, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different length");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 60, SpotifyTrackType.AD, "album")).Returns(false).SetName("not equal: different type");
                }
            }

            public static IEnumerable<TestCaseData> StaticEqualTestCases
            {
                get
                {
                    Song normal = new Song(NormalTrack);
                    Song ad = new Song(AdPositiveLengthTrack);
                    Song other = new Song(OtherTrack);

                    foreach (var testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    // with null
                    yield return new TestCaseData(null, null).Returns(true).SetName("with null: both null");

                    yield return new TestCaseData(normal, null).Returns(false).SetName("with null: normal,null");
                    yield return new TestCaseData(ad, null).Returns(false).SetName("with null: ad,null");
                    yield return new TestCaseData(other, null).Returns(false).SetName("with null: other,null");

                    yield return new TestCaseData(null, normal).Returns(false).SetName("with null: null,normal");
                    yield return new TestCaseData(null, ad).Returns(false).SetName("with null: null,ad");
                    yield return new TestCaseData(null, other).Returns(false).SetName("with null: null,other");

                    yield return new TestCaseData(new Song(null, null, 0, null, null), null).Returns(false).SetName("with null: empty track,null");
                    yield return new TestCaseData(null, new Song(null, null, 0, null, null)).Returns(false).SetName("with null: null,empty track");
                }
            }

            public static IEnumerable<TestCaseData> ObjectEqualsTestCases
            {
                get
                {
                    Song normal = new Song(NormalTrack);

                    foreach (var testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    // different type
                    yield return new TestCaseData(normal, "song").Returns(false).SetName("different object type");
                }
            }

            public static IEnumerable<TestCaseData> GetHashCodeTestCases
            {
                get
                {
                    Song normal = new Song(NormalTrack);
                    Song ad = new Song(AdPositiveLengthTrack);
                    Song other1 = new Song(OtherTrack);
                    Song other2 = new Song(other1.Artist, other1.Track, other1.Length * 2, other1.Type, other1.Album);

                    // same
                    yield return new TestCaseData(normal, normal).Returns(true).SetName("same");

                    // equal
                    yield return new TestCaseData(normal, new Song(NormalTrack)).Returns(true).SetName("equal");
                    yield return new TestCaseData(other1, new Song(OtherTrack)).Returns(true).SetName("equal: OTHER tracks with same length");

                    // not equal
                    yield return new TestCaseData(normal, ad).Returns(false).SetName("not equal");
                    yield return new TestCaseData(other1, other2).Returns(false).SetName("not equal: OTHER tracks with different length");

                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("different artist", "title", 60, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different artist");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "different title", 60, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different title");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "different album")).Returns(false).SetName("not equal: different album");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 30, SpotifyTrackType.NORMAL, "album")).Returns(false).SetName("not equal: different length");
                    yield return new TestCaseData(
                        new Song("artist", "title", 60, SpotifyTrackType.NORMAL, "album"),
                        new Song("artist", "title", 60, SpotifyTrackType.AD, "album")).Returns(false).SetName("not equal: different type");
                }
            }

            public static IEnumerable<TestCaseData> FromSpotifyWindowTitleTestCases
            {
                get
                {
                    string GetTitle(IReadOnlyList<string> parts) => $"{parts[0]} - {parts[1]}";

                    string GetTestName(IReadOnlyList<string> parts) => $"Artist: «{parts[0]}», Title: «{parts[1]}»";

                    void Test(ISong song, IReadOnlyList<string> parts)
                    {
                        Assert.That(song.Artist, Is.EqualTo(parts[0]));
                        Assert.That(song.Track, Is.EqualTo(parts[1]));
                    }

                    string[] t1 = { "Artist", "Title" };
                    yield return new TestCaseData(GetTitle(t1), new Action<Song>(song => Test(song, t1))).SetName(GetTestName(t1));

                    string[] t2 = { "Artist", "A Compound-Title" };
                    yield return new TestCaseData(GetTitle(t2), new Action<Song>(song => Test(song, t2))).SetName(GetTestName(t2));

                    string[] t3 = { "Artist", "Title - With - a Bunch of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t3), new Action<Song>(song => Test(song, t3))).SetName(GetTestName(t3));

                    string[] t4 = { "Artist", "A Compound-Title - With - a-Bunch-of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t4), new Action<Song>(song => Test(song, t4))).SetName(GetTestName(t4));

                    string[] t5 = { "A Compound-Artist Name", "Title" };
                    yield return new TestCaseData(GetTitle(t5), new Action<Song>(song => Test(song, t5))).SetName(GetTestName(t5));

                    string[] t6 = { "A Compound-Artist Name", "A Compound-Title" };
                    yield return new TestCaseData(GetTitle(t6), new Action<Song>(song => Test(song, t6))).SetName(GetTestName(t6));

                    string[] t7 = { "A Compound-Artist Name", "Title - With - a Bunch of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t7), new Action<Song>(song => Test(song, t7))).SetName(GetTestName(t7));

                    string[] t8 = { "A Compound-Artist Name", "A Compound-Title - With - a-Bunch-of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t8), new Action<Song>(song => Test(song, t8))).SetName(GetTestName(t8));

                    string[] t9 = { "A Multi-Compounded Artist-Name", "Title" };
                    yield return new TestCaseData(GetTitle(t9), new Action<Song>(song => Test(song, t9))).SetName(GetTestName(t9));

                    string[] t10 = { "A Multi-Compounded Artist-Name", "A Compound-Title" };
                    yield return new TestCaseData(GetTitle(t10), new Action<Song>(song => Test(song, t10))).SetName(GetTestName(t10));

                    string[] t11 = { "A Multi-Compounded Artist-Name", "Title - With - a Bunch of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t11), new Action<Song>(song => Test(song, t11))).SetName(GetTestName(t11));

                    string[] t12 = { "A Multi-Compounded Artist-Name", "A Compound-Title - With - a-Bunch-of - Hyphens" };
                    yield return new TestCaseData(GetTitle(t12), new Action<Song>(song => Test(song, t12))).SetName(GetTestName(t12));

                    // NOTE: Artist names with hyphens surrounded by spaces are not supported: it has been chosen to assume that only song titles have them!
                }
            }

            internal static SpotifyResource TestAlbumResource { get; } = new SpotifyResource
            {
                Location = new TrackResourceLocation { Og = string.Empty },
                Name = "Test Album",
                Uri = "http://test.album"
            };

            internal static SpotifyResource TestArtistResource { get; } = new SpotifyResource
            {
                Location = new TrackResourceLocation { Og = string.Empty },
                Name = "Test Artist",
                Uri = "http://test.artist"
            };

            internal static SpotifyResource TestTrackResource { get; } = new SpotifyResource
            {
                Location = new TrackResourceLocation { Og = string.Empty },
                Name = "Test Track",
                Uri = "http://test.track"
            };

            internal static Track AdPositiveLengthTrack { get; } = new Track
            {
                AlbumResource = TestAlbumResource,
                ArtistResource = TestArtistResource,
                TrackResource = TestTrackResource,
                Length = 60,
                TrackType = SpotifyTrackType.AD
            };

            internal static Track AdZeroLengthTrack { get; } = new Track
            {
                AlbumResource = TestAlbumResource,
                ArtistResource = TestArtistResource,
                TrackResource = TestTrackResource,
                Length = 0,
                TrackType = SpotifyTrackType.AD
            };

            internal static Track OtherTrack { get; } = new Track
            {
                AlbumResource = null,
                ArtistResource = null,
                TrackResource = null,
                Length = 60,
                TrackType = SpotifyTrackType.OTHER
            };

            internal static Track NormalTrack { get; } = new Track
            {
                AlbumResource = TestAlbumResource,
                ArtistResource = TestArtistResource,
                TrackResource = TestTrackResource,
                Length = 60,
                TrackType = SpotifyTrackType.NORMAL
            };

            #endregion
        }

        #endregion
    }
}