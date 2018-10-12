using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ToastifyAPI.Core
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpotifyTrackType
    {
        Unknown = -1,
        Song,
        Episode,
        Ad
    }
}