namespace VolumeControl.Core.Controls
{
    public static class CornerExtensions
    {
        public static Point? GetPosition(this Corner corner, Size screenSize, Size objSize, Size? edgePadding = null)
        {
            Size padding = edgePadding ?? new(0, 0);
            switch (corner)
            {
            case Corner.TopLeft:
                return new(padding.Width, padding.Height);
            case Corner.TopRight:
                return new(screenSize.Width - objSize.Width - padding.Width, padding.Height);
            case Corner.BottomLeft:
                return new(padding.Width, screenSize.Height - objSize.Height - padding.Height);
            case Corner.BottomRight:
                return new(screenSize.Width - objSize.Width - padding.Width, screenSize.Height - objSize.Height - padding.Height);
            default: break;
            }
            return null;
        }
    }
}
