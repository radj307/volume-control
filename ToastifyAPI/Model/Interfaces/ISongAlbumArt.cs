using System;

namespace ToastifyAPI.Model.Interfaces
{
    public interface ISongAlbumArt : IEquatable<ISongAlbumArt>
    {
        #region Public Properties

        int Height { get; }
        int Width { get; }
        string Url { get; }

        #endregion
    }
}