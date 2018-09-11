using System;
using Newtonsoft.Json;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Core.Broadcaster
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonSong
    {
        #region Public Properties

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("artist")]
        public string Artist { get; }

        [JsonProperty("album")]
        public string Album { get; }

        [JsonProperty("length")]
        public int Length { get; }

        #endregion

        public JsonSong(ISong song)
        {
            this.Title = song?.Track;
            this.Artist = song?.Artist;
            this.Album = song?.Album;
            this.Length = song?.Length ?? -1;
        }
    }
}