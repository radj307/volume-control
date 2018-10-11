using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using SpotifyAPI.Web.Models;
using Toastify.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(Song))]
    public class SongTest
    {
        #region Static Members

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.AlbumArtTestCases))]
        public static void AlbumArtTest(Song song, Action<Song> test)
        {
            test?.Invoke(song);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.IsValidTestCases))]
        public static bool IsValidTest(Song song)
        {
            return song.IsValid();
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.FromSpotifyWindowTitleTestCases))]
        public static void TestFromSpotifyWindowTitle(string title, Action<Song> test)
        {
            Song song = Song.FromSpotifyWindowTitle(title);
            test?.Invoke(song);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.TrackTestCases))]
        public static void ImplicitCastToSong_Track(FullTrack track)
        {
            Song song = track;

            if (track == null)
                Assert.That(song, Is.Null);
            else
            {
                ISong mockedSong = A.Fake<ISong>(options => options.Wrapping(song));
                A.CallTo(() => mockedSong.AlbumArt).Returns(SongAlbumArt.Empty);

                Assert.Multiple(() =>
                {
                    var artistStrings = track.Artists.Select(a => a.Name).ToList();
                    Assert.That(mockedSong.Album, Is.EqualTo(track.Album.Name));
                    Assert.That(mockedSong.Artists, Is.SubsetOf(artistStrings));
                    Assert.That(artistStrings, Is.SubsetOf(mockedSong.Artists));
                    Assert.That(mockedSong.Title, Is.EqualTo(track.Name));
                    Assert.That(mockedSong.Length, Is.EqualTo(track.DurationMs / 1000));
                });
            }
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.EqualsTestCases))]
        public static bool EqualsTest(Song s1, Song s2)
        {
            if (s1 != null)
                return s1.Equals(s2);
            return s2 == null || s2.Equals(null);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.ObjectEqualsTestCases))]
        public static bool EqualsTest_Object(Song s, object obj)
        {
            if (s != null)
                return s.Equals(obj);
            return obj == null || obj.Equals(null);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.StaticEqualTestCases))]
        public static bool EqualTest_Static(Song s1, Song s2)
        {
            return Song.Equal(s1, s2);
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(SongAndTrackData), nameof(SongAndTrackData.GetHashCodeTestCases))]
        public static bool GetHashCodeTest(Song s1, Song s2)
        {
            return s1?.GetHashCode() == s2?.GetHashCode();
        }

        #endregion

        #region Test Cases

        public static class SongAndTrackData
        {
            #region Static Fields and Properties

            public static IEnumerable<TestCaseData> TrackTestCases
            {
                get
                {
                    // Normal track
                    yield return new TestCaseData(TestFullTrack).SetName("NORMAL Track");

                    // Null track
                    yield return new TestCaseData(null).SetName("null Track");
                }
            }

            public static IEnumerable<TestCaseData> AlbumArtTestCases
            {
                get
                {
                    Song regularSong = new Song("album", "artist", "title", 1);
                    Song trackSong = new Song(TestFullTrack);
                    Song songWithAlbumArt = new Song(TestFullTrack)
                    {
                        AlbumArt = new SongAlbumArt(10, 10, "http://fake.album.art")
                    };

                    yield return new TestCaseData(regularSong, new Action<Song>(song =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(song.AlbumArt, Is.Not.Null);
                            Assert.That(song.AlbumArt.Url, Is.Empty);
                            Assert.That(song.AlbumArt.Height, Is.EqualTo(0));
                            Assert.That(song.AlbumArt.Width, Is.EqualTo(0));
                        });
                    })).SetName("no album art set, no underlying track");

                    yield return new TestCaseData(trackSong, new Action<Song>(song =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(song.AlbumArt, Is.Not.Null);
                            Assert.That(song.AlbumArt.Url, Is.EqualTo(TestFullTrack.Album?.Images?.LastOrDefault()?.Url));
                        });
                    })).SetName("no album art set, with underlying track");

                    yield return new TestCaseData(songWithAlbumArt, new Action<Song>(song =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(song.AlbumArt, Is.Not.Null);
                            Assert.That(song.AlbumArt.Url, Is.Not.EqualTo(TestFullTrack.Album?.Images?.LastOrDefault()?.Url));
                            Assert.That(song.AlbumArt.Url, Is.EqualTo("http://fake.album.art"));
                        });
                    })).SetName("album art set, with underlying track");
                }
            }

            public static IEnumerable<TestCaseData> IsValidTestCases
            {
                get
                {
                    // NORMAL songs must have a non-null & non-empty Album, Artist and Track, the NORMAL type and a positive length.
                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, TestSimpleArtist.Name, TestSimpleTrack.Name, 60)).Returns(true).SetName("valid");

                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, string.Empty, TestSimpleTrack.Name, 60)).Returns(false).SetName("artist");
                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, string.Empty, string.Empty, 60)).Returns(false).SetName("artist,title");
                    yield return new TestCaseData(new Song(string.Empty, string.Empty, TestSimpleTrack.Name, 60)).Returns(false).SetName("artist,album");
                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, TestSimpleArtist.Name, string.Empty, 60)).Returns(false).SetName("title");
                    yield return new TestCaseData(new Song(string.Empty, TestSimpleArtist.Name, string.Empty, 60)).Returns(false).SetName("title,album");
                    yield return new TestCaseData(new Song(string.Empty, TestSimpleArtist.Name, TestSimpleTrack.Name, 60)).Returns(false).SetName("album");

                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, TestSimpleArtist.Name, TestSimpleTrack.Name, -1)).Returns(false).SetName("length <0");
                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, TestSimpleArtist.Name, TestSimpleTrack.Name, 0)).Returns(true).SetName("length =0");
                    yield return new TestCaseData(new Song(TestSimpleAlbum.Name, TestSimpleArtist.Name, TestSimpleTrack.Name, 1)).Returns(true).SetName("length >0");

                    // Songs from valid Tracks
                    yield return new TestCaseData(new Song(TestFullTrack)).Returns(true).SetName("from valid Track");
                }
            }

            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    Song song = new Song(TestFullTrack);

                    // with null
                    yield return new TestCaseData(song, null).Returns(false).SetName("with null");

                    // same
                    yield return new TestCaseData(song, song).Returns(true).SetName("same");

                    // equal
                    yield return new TestCaseData(song, new Song(TestFullTrack)).Returns(true).SetName("equal");

                    // not equal
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("different album", "artist", "title", 60)).Returns(false).SetName("not equal: different album");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "different artist", "title", 60)).Returns(false).SetName("not equal: different artist");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "artist", "different title", 60)).Returns(false).SetName("not equal: different title");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "artist", "title", 30)).Returns(false).SetName("not equal: different length");
                }
            }

            public static IEnumerable<TestCaseData> StaticEqualTestCases
            {
                get
                {
                    Song song = new Song(TestFullTrack);

                    foreach (var testCase in EqualsTestCases)
                    {
                        yield return testCase;
                    }

                    // with null
                    yield return new TestCaseData(null, null).Returns(true).SetName("with null: both null");

                    yield return new TestCaseData(song, null).Returns(false).SetName("with null: song,null");

                    yield return new TestCaseData(null, song).Returns(false).SetName("with null: null,song");

                    yield return new TestCaseData(new Song(null, (string)null, null, 0), null).Returns(false).SetName("with null: empty track,null");
                    yield return new TestCaseData(null, new Song(null, (string)null, null, 0)).Returns(false).SetName("with null: null,empty track");
                }
            }

            public static IEnumerable<TestCaseData> ObjectEqualsTestCases
            {
                get
                {
                    Song normal = new Song(TestFullTrack);

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
                    Song song = new Song(TestFullTrack);

                    // same
                    yield return new TestCaseData(song, song).Returns(true).SetName("same");

                    // equal
                    yield return new TestCaseData(song, new Song(TestFullTrack)).Returns(true).SetName("equal");

                    // not equal
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("different album", "artist", "title", 60)).Returns(false).SetName("not equal: different album");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "different artist", "title", 60)).Returns(false).SetName("not equal: different artist");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "artist", "different title", 60)).Returns(false).SetName("not equal: different title");
                    yield return new TestCaseData(
                        new Song("album", "artist", "title", 60),
                        new Song("album", "artist", "title", 30)).Returns(false).SetName("not equal: different length");
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
                        Assert.That(song.Artists.FirstOrDefault(), Is.EqualTo(parts[0]));
                        Assert.That(song.Title, Is.EqualTo(parts[1]));
                    }
                    
                    yield return new TestCaseData(null, new Action<Song>(song => Assert.That(song, Is.Null))).SetName("null title => null song");
                    yield return new TestCaseData(string.Empty, new Action<Song>(song => Assert.That(song, Is.Null))).SetName("empty title => null song");
                    yield return new TestCaseData(" ", new Action<Song>(song => Assert.That(song, Is.Null))).SetName("whitespace title => null song");

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

            internal static SimpleAlbum TestSimpleAlbum { get; } = new SimpleAlbum
            {
                Name = "Test Album",
                Uri = "http://test.album",
                Images = new List<Image>
                {
                    new Image { Height = 256, Width = 256, Url = "http://test.album/256/cover.png" },
                    new Image { Height = 64, Width = 64, Url = "http://test.album/64/cover.png" }
                }
            };

            internal static SimpleArtist TestSimpleArtist { get; } = new SimpleArtist
            {
                Name = "Test Artist",
                Uri = "http://test.artist"
            };

            internal static SimpleTrack TestSimpleTrack { get; } = new SimpleTrack
            {
                Name = "Test Track",
                Uri = "http://test.track"
            };

            internal static FullTrack TestFullTrack { get; } = new FullTrack
            {
                Album = TestSimpleAlbum,
                Artists = new List<SimpleArtist>(1) { TestSimpleArtist },
                Name = TestSimpleTrack.Name,
                DurationMs = 60000,
                Uri = TestSimpleTrack.Uri
            };

            #endregion
        }

        #endregion
    }
}