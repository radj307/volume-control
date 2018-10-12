using System;
using Newtonsoft.Json;

namespace Toastify.Core.Broadcaster
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonGreetingsObject
    {
        #region Public Properties

        [JsonProperty("track")]
        public JsonTrack Track { get; set; }

        [JsonProperty("playing")]
        public bool Playing { get; set; }

        #endregion
    }
}