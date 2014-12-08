using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Toastify
{
    public static class ScreenHelper
    {

        private const int SCREEN_RIGHT_MARGIN = 5;
        private const int SCREEN_TOP_MARGIN = 5;

        public static Point GetDPIRatios()
        {
            var presentationSource = PresentationSource.FromVisual(Toast.Current);

            // if we hit this then Settings were loaded before the Toast window was
            System.Diagnostics.Debug.Assert(presentationSource != null);

            if (presentationSource == null)
            {
                // return a default dpi ratio of 1 (96dpi)
                return new Point(1, 1);
            }

            return new Point(
                presentationSource.CompositionTarget.TransformToDevice.M11,
                presentationSource.CompositionTarget.TransformToDevice.M22
            );
        }

        public static System.Windows.Point GetDefaultToastPosition(double width, double height)
        {
            var screenRect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            var dpiRatio = GetDPIRatios();

            return new System.Windows.Point
                (
                    (screenRect.Width  / dpiRatio.X) - width  - SCREEN_RIGHT_MARGIN,
                    (screenRect.Height / dpiRatio.Y) - height - SCREEN_TOP_MARGIN
                );
        }
    }
}
