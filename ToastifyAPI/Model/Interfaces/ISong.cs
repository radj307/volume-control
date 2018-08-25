using System;
using JetBrains.Annotations;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISong : IEquatable<ISong>
    {
        #region Public Properties

        [NotNull]
        string Album { get; }

        [NotNull]
        string Artist { get; }

        [NotNull]
        string Track { get; }

        int Length { get; }

        [NotNull]
        string Type { get; }

        [NotNull]
        string CoverArtUrl { get; }

        #endregion

        bool IsAd();

        bool IsOtherTrackType();

        [NotNull]
        string GetSmallestCoverArtUrl();
    }
}