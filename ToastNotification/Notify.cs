using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetListForm
{
    public struct DefaultColor
    {
        public static readonly Color Foreground = Color.Black;
        public static readonly Color Background = Color.LightSlateGray;
    }

    public static class Notify
    {
        public static Color GetAltColor(Color color)
        {
            return Color.FromArgb(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        public static void ShowNotification(string message, int timeout, Image? img, Color background, Color foreground)
        {
            NotificationForm form = new();
            form.ShowNotification(message, timeout, img, background, foreground);
        }
        public static void ShowNotification(string message, int timeout, Image img, Color background) => ShowNotification(message, timeout, img, background, GetAltColor(background));
        public static void ShowNotification(string message, int timeout, Color background, Color foreground) => ShowNotification(message, timeout, null, background, foreground);

    }
}
