using Toastify.Common;

namespace Toastify.Core
{
    public enum ToastTitlesOrder
    {
        [ComboBoxItem("Track by Artist", "Show the Artist name below the Track name.")]
        TrackByArtist,

        [ComboBoxItem("Artist: \x201CTrack\x201D", "Show The Track name below the Artist name, inside double quotes.")]
        ArtistOfTrack
    }
}