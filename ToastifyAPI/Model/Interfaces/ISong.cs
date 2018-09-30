using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISong : IEquatable<ISong>
    {
        #region Public Properties

        [NotNull]
        string Album { get; }

        [NotNull]
        IReadOnlyList<string> Artists { get; }

        [NotNull]
        string Title { get; }

        int Length { get; }

        ISongAlbumArt AlbumArt { get; }

        #endregion
    }
}