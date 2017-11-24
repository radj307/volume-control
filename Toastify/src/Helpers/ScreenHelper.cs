using System;
using System.Windows;
using System.Windows.Forms;
using log4net;
using log4net.Util;
using Toastify.View;

namespace Toastify.Helpers
{
    internal static class ScreenHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ScreenHelper));

        private const int SCREEN_RIGHT_MARGIN = 0;
        private const int SCREEN_TOP_MARGIN = 5;

        public static Point GetDPIRatios()
        {
            Point p = new Point(1.0, 1.0);
            try
            {
                if (ToastView.Current == null)
                    return p;

                var presentationSource = PresentationSource.FromVisual(ToastView.Current);

                if (presentationSource == null)
                    throw new ApplicationException("presentationSource is null");

                p = new Point(
                    presentationSource.CompositionTarget?.TransformToDevice.M11 ?? 1.0,
                    presentationSource.CompositionTarget?.TransformToDevice.M22 ?? 1.0);
            }
            catch (ApplicationException e)
            {
                logger.ErrorExt("Couldn't get PresentationSource, current ToastView has been disposed.", e);
            }
            return p;
        }

        public static Point GetDefaultToastPosition(double width, double height)
        {
            var screenRect = Screen.PrimaryScreen.WorkingArea;
            Point dpiRatio = GetDPIRatios();
            return new Point(screenRect.Width / dpiRatio.X - width - SCREEN_RIGHT_MARGIN,
                             screenRect.Height / dpiRatio.Y - height - SCREEN_TOP_MARGIN);
        }
    }
}