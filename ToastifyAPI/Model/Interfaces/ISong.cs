using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISong : ISpotifyTrack, IEquatable<ISong>
    {
        #region Public Properties

        [NotNull]
        string Album { get; }

        [NotNull]
        IReadOnlyList<string> Artists { get; }

        ISongAlbumArt AlbumArt { get; set; }

        #endregion
    }
}