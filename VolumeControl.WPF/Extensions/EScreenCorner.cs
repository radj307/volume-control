namespace VolumeControl.WPF.Extensions
{
    /// <summary>
    /// Defines the corners of a screen. (Rectangle)
    /// </summary>
    public enum EScreenCorner : byte
    {
        /// <summary>(⬉) The upper-left corner of the screen.</summary>
        TopLeft = 0,
        /// <summary>(⬈) The upper-right corner of the screen.</summary>
        TopRight = 1,
        /// <summary>(⬋) The lower-left corner of the screen.</summary>
        BottomLeft = 2,
        /// <summary>(⬊) The lower-right corner of the screen.</summary>
        BottomRight = 3,
    }
}
