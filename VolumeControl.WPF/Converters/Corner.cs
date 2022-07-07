using System;

namespace VolumeControl.WPF.Converters
{
    /// <summary>Specifies the corners of a 2D square.</summary>
    [Flags]
    public enum Corner
    {
        /// <summary>Nothing.</summary>
        None = 0,
        /// <summary>The top-left corner.</summary>
        TopLeft = 1,
        /// <summary>The top-right corner.</summary>
        TopRight = 2,
        /// <summary>The bottom-left corner.</summary>
        BottomLeft = 4,
        /// <summary>The bottom-right corner.</summary>
        BottomRight = 8,
        /// <summary>The top left and top right corners.</summary>
        Top = TopLeft | TopRight,
        /// <summary>The bottom left and bottom right corners.</summary>
        Bottom = BottomLeft | BottomRight,
        /// <summary>The top left and bottom left corners.</summary>
        Left = TopLeft | BottomLeft,
        /// <summary>The top right and bottom right corners.</summary>
        Right = TopRight | BottomRight,
        /// <summary>All corners.</summary>
        All = Top | Bottom,
    }
}
