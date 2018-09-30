using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ToastifyAPI.Core
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpotifySubscriptionLevel
    {
        Unknown = -1,
        Free,
        Open = Free,
        Premium
    }
}