using SpotifyAPI.Web.Models;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class CurrentlyPlayingObject : ICurrentlyPlayingObject
    {
        #region Public Properties

        public int ProgressMs { get; }
        public bool IsPlaying { get; }
        public ISong Track { get; }
        public string Type { get; }

        #endregion

        public CurrentlyPlayingObject(int progressMs, bool isPlaying, ISong track, string type)
        {
            this.ProgressMs = progressMs;
            this.IsPlaying = isPlaying;
            this.Track = track;
            this.Type = type;
        }

        public CurrentlyPlayingObject(PlaybackContext playbackContext)
            : this(playbackContext.ProgressMs, playbackContext.IsPlaying, new Song(playbackContext.Item), null)
        {
            // TODO: Set "currently_playing_type" once SpotifyAPI-NET has added it
        }
    }
}