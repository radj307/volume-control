using VolumeControl.Core.Controls.Enum;

namespace VolumeControl.Core.Controls.Extensions
{
    public static class CornerExtensions
    {
        public static Point? GetPosition(this Corner corner, Size screenSize, Size objSize, Size? edgePadding = null, Size? offset = null)
        {
            Size padding = edgePadding ?? new(0, 0);
            Point? result = null;
            switch (corner)
            {
            case Corner.TopLeft:
                result = new(padding.Width, padding.Height);
                break;
            case Corner.TopRight:
                result = new(screenSize.Width - objSize.Width - padding.Width, padding.Height);
                break;
            case Corner.BottomLeft:
                result = new(padding.Width, screenSize.Height - objSize.Height - padding.Height);
                break;
            case Corner.BottomRight:
                result = new(screenSize.Width - objSize.Width - padding.Width, screenSize.Height - objSize.Height - padding.Height);
                break;
            default: break;
            }
            if (result != null && offset != null)
                result = new(result.Value.X + offset.Value.Width, result.Value.Y + offset.Value.Height);
            return result;
        }
    }
}
