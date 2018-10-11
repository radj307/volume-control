using System;
using JetBrains.Annotations;
using SpotifyAPI.Web.Models;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class SongAlbumArt : ISongAlbumArt, IEquatable<SongAlbumArt>
    {
        #region Static Fields and Properties

        public static readonly SongAlbumArt Empty = new SongAlbumArt(0, 0, string.Empty);

        #endregion

        #region Public Properties

        public int Height { get; }
        public int Width { get; }
        public string Url { get; }

        #endregion

        public SongAlbumArt(int height, int width, string url)
        {
            this.Height = height;
            this.Width = width;
            this.Url = url;
        }

        public SongAlbumArt([NotNull] Image image) : this(image.Height, image.Width, image.Url)
        {
        }

        public bool IsEmpty()
        {
            return ReferenceEquals(this, Empty) || this.Equals(Empty);
        }

        #region Equals / GetHashCode

        public bool Equals(SongAlbumArt other)
        {
            return this.Equals((ISongAlbumArt)other);
        }

        public bool Equals(ISongAlbumArt other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return this.Height == other.Height &&
                   this.Width == other.Width &&
                   string.Equals(this.Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() &&
                   this.Equals((SongAlbumArt)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Height;
                hashCode = (hashCode * 397) ^ this.Width;
                hashCode = (hashCode * 397) ^ (this.Url != null ? this.Url.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}