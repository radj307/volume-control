using System;
using Newtonsoft.Json;

namespace Toastify.Core.Broadcaster
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonGreetingsObject
    {
        #region Public Properties

        [JsonProperty("song")]
        public JsonSong Song { get; set; }

        [JsonProperty("playing")]
        public bool Playing { get; set; }

        #endregion
    }
}