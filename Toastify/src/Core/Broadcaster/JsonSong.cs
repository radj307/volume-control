using System;
using System.Collections.Generic;
using System.Linq;
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

        [JsonProperty("artists")]
        public List<string> Artists { get; }

        [JsonProperty("album")]
        public string Album { get; }

        [JsonProperty("length")]
        public int Length { get; }

        #endregion

        public JsonSong(ISong song)
        {
            this.Title = song?.Title;
            this.Artists = song?.Artists.ToList();
            this.Album = song?.Album;
            this.Length = song?.Length ?? -1;
        }
    }
}