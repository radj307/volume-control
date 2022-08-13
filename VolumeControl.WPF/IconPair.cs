using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Encapsulates a pair of differently-sized icons.
    /// </summary>
    public class IconPair
    {
        /// <summary>
        /// Default <see cref="BitmapSizeOptions"/> object used by the <see cref="CreateImageSourceFromIconHandle(IntPtr)"/> method.
        /// </summary>
        public static readonly BitmapSizeOptions DefaultSizeOptions = BitmapSizeOptions.FromEmptyOptions();
        /// <summary>Calls <see cref="Imaging.CreateBitmapSourceFromHIcon(IntPtr, Int32Rect, BitmapSizeOptions)"/> with <see cref="DefaultSizeOptions"/>.</summary>
        /// <inheritdoc cref="Imaging.CreateBitmapSourceFromHIcon(IntPtr, Int32Rect, BitmapSizeOptions)"/>
        public static ImageSource CreateImageSourceFromIconHandle(IntPtr icon) => Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, DefaultSizeOptions);
        /// <inheritdoc cref="CreateImageSourceFromIconHandle(IntPtr)"/>
        public static ImageSource CreateImageSourceFromIconHandle(IntPtr icon, BitmapSizeOptions sizeOptions) => Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, sizeOptions);

        /// <summary>
        /// Creates a new instance of <see cref="IconPair"/> with <see langword="null"/> values.
        /// </summary>
        public IconPair() { }
        /// <summary>
        /// Creates a new instance of <see cref="IconPair"/> with the given icons.
        /// </summary>
        /// <param name="smallIcon">Handle of a smaller-sized icon. See <see cref="SmallIcon"/></param>
        /// <param name="largeIcon">Handle of a larger-sized icon. See <see cref="LargeIcon"/></param>
        public IconPair(IntPtr? smallIcon, IntPtr? largeIcon)
        {
            if (smallIcon.HasValue) this.SmallIcon = CreateImageSourceFromIconHandle(smallIcon.Value);
            if (largeIcon.HasValue) this.LargeIcon = CreateImageSourceFromIconHandle(largeIcon.Value);
        }
        /// <summary>
        /// Creates a new instance of <see cref="IconPair"/> with the given icon sources.
        /// </summary>
        /// <param name="smallIconSource"><see cref="SmallIcon"/></param>
        /// <param name="largeIconSource"><see cref="LargeIcon"/></param>
        public IconPair(ImageSource? smallIconSource, ImageSource? largeIconSource)
        {
            this.SmallIcon = smallIconSource;
            this.LargeIcon = largeIconSource;
        }

        /// <summary>
        /// Smaller-sized icon.
        /// </summary>
        public ImageSource? SmallIcon { get; set; }
        /// <summary>
        /// Larger-sized icon.
        /// </summary>
        public ImageSource? LargeIcon { get; set; }
        /// <summary>
        /// Gets the first non-<see langword="null"/> icon, respecting the specified preference.
        /// </summary>
        /// <param name="preferLarge"></param>
        /// <returns><see cref="LargeIcon"/> or <see cref="SmallIcon"/>.<br/>When <paramref name="preferLarge"/> is <see langword="true"/>, <see cref="LargeIcon"/> is preferred over <see cref="SmallIcon"/>; otherwise <see cref="SmallIcon"/> is preferred.</returns>
        public ImageSource? GetBestFitIcon(bool preferLarge = true) => preferLarge ? (this.LargeIcon ?? this.SmallIcon) : (this.SmallIcon ?? this.LargeIcon);

        /// <summary>
        /// Check if this <see cref="IconPair"/> instance does not contain any valid icons.
        /// </summary>
        /// <returns><see langword="true"/> when both <see cref="SmallIcon"/> &amp; <see cref="LargeIcon"/> are <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool IsNull => this.SmallIcon is null && this.LargeIcon is null;

        /// <summary>Deconstructs the <see cref="IconPair"/> into a tuple.</summary>
        /// <param name="smallIcon"><see cref="SmallIcon"/></param>
        /// <param name="largeIcon"><see cref="LargeIcon"/></param>
        public void Deconstruct(out ImageSource? smallIcon, out ImageSource? largeIcon)
        {
            smallIcon = this.SmallIcon;
            largeIcon = this.LargeIcon;
        }
    }
}
