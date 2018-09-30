using JetBrains.Annotations;
using SpotifyAPI.Web.Models;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    public class SongAlbumArt : ISongAlbumArt
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
    }
}