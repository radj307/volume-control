using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Core.Broadcaster
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonTrack
    {
        #region Public Properties

        [JsonProperty("type")]
        public SpotifyTrackType Type { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("length")]
        public int Length { get; }

        [JsonProperty("artists")]
        public List<string> Artists { get; }

        [JsonProperty("album")]
        public string Album { get; }

        #endregion

        public JsonTrack(ISpotifyTrack track)
        {
            this.Type = track?.Type ?? SpotifyTrackType.Unknown;
            this.Title = track?.Title;
            this.Length = track?.Length ?? 0;

            if (track?.Type == SpotifyTrackType.Song)
            {
                ISong song = track as ISong;
                this.Artists = song?.Artists.ToList();
                this.Album = song?.Album;
            }
        }
    }
}