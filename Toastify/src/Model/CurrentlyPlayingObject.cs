using System;
using JetBrains.Annotations;
using log4net;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using ToastifyAPI.Core;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class CurrentlyPlayingObject : ICurrentlyPlayingObject
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CurrentlyPlayingObject));

        #region Public Properties

        public int ProgressMs { get; }
        public bool IsPlaying { get; }
        public ISpotifyTrack Track { get; }
        public SpotifyTrackType Type { get; }

        #endregion

        public CurrentlyPlayingObject(int progressMs, bool isPlaying, ISpotifyTrack track, SpotifyTrackType type)
        {
            this.ProgressMs = progressMs;
            this.IsPlaying = isPlaying;
            this.Track = track;
            this.Type = type;
        }

        // ReSharper disable ConstantConditionalAccessQualifier
        // ReSharper disable once ConstantNullCoalescingCondition
        public CurrentlyPlayingObject([NotNull] PlaybackContext playbackContext)
            : this(playbackContext?.ProgressMs ?? 0, playbackContext?.IsPlaying ?? false, null, SpotifyTrackType.Unknown)
        {
            if (playbackContext == null)
                throw new ArgumentNullException(nameof(playbackContext));

            switch (playbackContext.CurrentlyPlayingType)
            {
                case TrackType.Track:
                    if (playbackContext.Item != null)
                        this.Track = new Song(playbackContext.Item);
                    break;

                case TrackType.Episode:
                    this.Track = new SpotifyTrack(SpotifyTrackType.Episode, playbackContext.Item?.Name, (playbackContext.Item?.DurationMs ?? 0) / 1000);
                    break;

                case TrackType.Ad:
                    this.Track = new SpotifyTrack(SpotifyTrackType.Ad);
                    break;

                case TrackType.Unknown:
                    this.Track = new SpotifyTrack(SpotifyTrackType.Unknown, playbackContext.Item?.Name, (playbackContext.Item?.DurationMs ?? 0) / 1000);
                    break;

                default:
                    logger.Error($"Unexpected CurrentlyPlayingType of current playback context: {playbackContext.CurrentlyPlayingType}");
                    throw new ArgumentOutOfRangeException();
            }

            this.Type = this.Track?.Type ?? SpotifyTrackType.Unknown;
        }
        // ReSharper restore ConstantConditionalAccessQualifier
    }
}