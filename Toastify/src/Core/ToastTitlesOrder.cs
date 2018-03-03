using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Toastify.Common;

namespace Toastify.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ToastTitlesOrder
    {
        [ComboBoxItem("Track by Artist", "Show the Artist name below the Track name.")]
        TrackByArtist,

        [ComboBoxItem("Artist: \x201CTrack\x201D", "Show The Track name below the Artist name, inside double quotes.")]
        ArtistOfTrack
    }
}