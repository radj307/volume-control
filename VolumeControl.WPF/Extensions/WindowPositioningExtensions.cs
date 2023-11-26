using System.ComponentModel;
using System.Windows;
using VolumeControl.TypeExtensions;

namespace VolumeControl.WPF.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Window"/> class and other helper functions for manipulating screen space coordinates and positioning windows.
    /// </summary>
    public static class WindowPositioningExtensions
    {
        #region Get/Set Pos
        /// <summary>
        /// Gets the position of the window.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>A <see cref="Point"/> containing the x/y coordinates of the window's origin point (top-left corner).</returns>
        public static Point GetPos(this Window wnd)
            => new(wnd.Left, wnd.Top);
        /// <summary>
        /// Sets the position of the window to a given point.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <param name="point">A <see cref="Point"/> specifying the x/y coordinates to position the window's origin point (top-left corner) at.</param>
        public static void SetPos(this Window wnd, Point point)
        {
            wnd.Left = point.X;
            wnd.Top = point.Y;
        }
        #endregion Get/Set Pos

        #region Get/Set PosAtCenterPoint
        /// <summary>
        /// Gets the centerpoint of the window.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>A <see cref="Point"/> containing the x/y coordinates of the window's centerpoint.</returns>
        public static Point GetPosAtCenterPoint(this Window wnd)
            => new(wnd.Left + wnd.Width / 2, wnd.Top + wnd.Height / 2);
        /// <summary>
        /// Sets the position of the window, using the window's center as an origin point.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <param name="point">The absolute x/y coordinates of the target position.</param>
        public static void SetPosAtCenterPoint(this Window wnd, Point point)
        {
            wnd.Left = point.X - wnd.Width / 2;
            wnd.Top = point.Y - wnd.Height / 2;
        }
        #endregion Get/Set CenterPoint

        #region Get/Set PosAtCorner
        /// <summary>
        /// Gets the position of the window at the specified <paramref name="corner"/>.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <param name="corner">The corner to check</param>
        /// <returns>The position of the window at the specified <paramref name="corner"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Invalid <see cref="EScreenCorner"/> enumeration.</exception>
        public static Point GetPosAtCorner(this Window wnd, EScreenCorner corner)
        {
            var compositionTarget = PresentationSource.FromVisual(wnd)?.CompositionTarget;
            return corner switch
            {
                EScreenCorner.TopLeft => compositionTarget?.TransformToDevice.Transform(new Point(wnd.Left, wnd.Top)) ?? new Point(wnd.Left, wnd.Top),
                EScreenCorner.TopRight => compositionTarget?.TransformToDevice.Transform(new Point(wnd.Left + wnd.ActualWidth, wnd.Top)) ?? new Point(wnd.Left + wnd.ActualWidth, wnd.Top),
                EScreenCorner.BottomLeft => compositionTarget?.TransformToDevice.Transform(new Point(wnd.Left, wnd.Top + wnd.ActualHeight)) ?? new Point(wnd.Left, wnd.Top + wnd.ActualHeight),
                EScreenCorner.BottomRight => compositionTarget?.TransformToDevice.Transform(new Point(wnd.Left + wnd.ActualWidth, wnd.Top + wnd.ActualHeight)) ?? new Point(wnd.Left + wnd.ActualWidth, wnd.Top + wnd.ActualHeight),
                _ => throw new InvalidEnumArgumentException(nameof(corner), (int)corner, typeof(EScreenCorner)),
            };
        }
        /// <summary>
        /// Sets the position of the window, using the specified <paramref name="corner"/> as an origin point.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <param name="corner">The corner of the window to use as an origin point when setting the window's position.</param>
        /// <param name="point">The absolute x/y coordinates of the target position.</param>
        public static void SetPosAtCorner(this Window wnd, EScreenCorner corner, Point point)
        {
            switch (corner)
            {
            case EScreenCorner.TopLeft:
                wnd.Left = point.X;
                wnd.Top = point.Y;
                break;
            case EScreenCorner.TopRight:
                wnd.Left = point.X - wnd.Width;
                wnd.Top = point.Y;
                break;
            case EScreenCorner.BottomLeft:
                wnd.Left = point.X;
                wnd.Top = point.Y - wnd.Height;
                break;
            case EScreenCorner.BottomRight:
                wnd.Left = point.X - wnd.Width;
                wnd.Top = point.Y - wnd.Height;
                break;
            }
        }
        #endregion Get/Set AtCorner

        #region GetPosAtCurrentCorner
        /// <summary>
        /// Gets the position of the corner of the window that is closest to the screen edge.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>A <see cref="Point"/> containing the x/y coordinates of the corner of the window that is closest to the screen edge.</returns>
        public static Point GetPosAtCurrentCorner(this Window wnd)
            => wnd.GetPosAtCorner(wnd.GetCurrentScreenCorner());
        #endregion GetPosAtCurrentCorner

        #region GetDefaultScreen
        /// <summary>
        /// Gets the primary screen.
        /// </summary>
        /// <returns></returns>
        public static System.Windows.Forms.Screen GetDefaultScreen() => System.Windows.Forms.Screen.PrimaryScreen;
        #endregion GetDefaultScreen

        #region GetCurrentScreen
        /// <summary>
        /// Gets the screen that contains the centerpoint of the window.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>The <see cref="System.Windows.Forms.Screen"/> instance that the window's centerpoint is found on.</returns>
        public static System.Windows.Forms.Screen GetCurrentScreen(this Window wnd)
            => GetClosestScreenFromPoint(wnd.GetPosAtCenterPoint());
        #endregion GetCurrentScreen

        #region GetCurrentScreenCenterPoint
        /// <summary>
        /// Gets the centerpoint of the screen where the window is currently located.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>A <see cref="Point"/> containing the x/y coordinates of the current screen's centerpoint.</returns>
        public static Point GetCurrentScreenCenterPoint(this Window wnd)
            => GetScreenCenterPoint(wnd.GetCurrentScreen());
        #endregion GetCurrentScreenCenterPoint

        #region GetCurrentScreenCorner
        /// <summary>
        /// Gets the closest <see cref="EScreenCorner"/> to the window's centerpoint.
        /// </summary>
        /// <param name="wnd">(Implicit) A <see cref="Window"/> instance.</param>
        /// <returns>The closest <see cref="EScreenCorner"/> to the window's centerpoint.</returns>
        public static EScreenCorner GetCurrentScreenCorner(this Window wnd)
            => GetClosestScreenCornerFromPoint(wnd.GetPosAtCenterPoint());
        #endregion GetCurrentScreenCorner

        #region GetClosestScreenFromPoint
        /// <summary>
        /// Gets the screen that contains the specified <paramref name="point"/>.
        /// </summary>
        /// <param name="point">A <see cref="Point"/> specifying the x/y coordinate of a point on the desktop.</param>
        /// <returns>The <see cref="System.Windows.Forms.Screen"/> instance that contains the specified <paramref name="point"/>, or the closest screen to the <paramref name="point"/> if none of the screens actually contain it.</returns>
        public static System.Windows.Forms.Screen GetClosestScreenFromPoint(Point point)
            => System.Windows.Forms.Screen.FromPoint(new((int)point.X, (int)point.Y));
        #endregion GetClosestScreenFromPoint

        #region GetScreenCenterPoint
        /// <summary>
        /// Gets the centerpoint of the specified <paramref name="screen"/>.
        /// </summary>
        /// <param name="screen">A <see cref="System.Windows.Forms.Screen"/> instance to get the centerpoint of.</param>
        /// <returns>A <see cref="Point"/> containing the absolute x/y coordinates of the specified <paramref name="screen"/>'s centerpoint.</returns>
        public static Point GetScreenCenterPoint(System.Windows.Forms.Screen screen)
            => new(screen.WorkingArea.Left + screen.WorkingArea.Width / 2, screen.WorkingArea.Top + screen.WorkingArea.Height / 2);
        #endregion GetScreenCenterPoint

        #region GetClosestScreenCornerFromPoint
        /// <summary>
        /// Gets the corner of the given <paramref name="screen"/> that is closest to the specified <paramref name="point"/>.
        /// </summary>
        /// <param name="screen">The <see cref="System.Windows.Forms.Screen"/> instance to use.</param>
        /// <param name="point">An x/y coordinate specifying a point on the <paramref name="screen"/>.</param>
        /// <returns>The <see cref="EScreenCorner"/> representing the corner of the <paramref name="screen"/> that is closest to the given <paramref name="point"/>.</returns>
        public static EScreenCorner GetClosestScreenCornerFromPoint(System.Windows.Forms.Screen screen, Point point)
        {
            // automatic corner selection is enabled:
            // get the centerpoint of this window
            (double cx, double cy) = GetScreenCenterPoint(screen);

            // figure out which corner is the closest & use that
            bool left = point.X < cx;
            bool top = point.Y < cy;

            if (left && top)
                return EScreenCorner.TopLeft;
            else if (!left && top)
                return EScreenCorner.TopRight;
            else if (left && !top)
                return EScreenCorner.BottomLeft;
            else if (!left && !top)
                return EScreenCorner.BottomRight;

            return EScreenCorner.TopLeft;
        }
        /// <inheritdoc cref="GetClosestScreenCornerFromPoint(System.Windows.Forms.Screen, Point)"/>
        /// <remarks>
        /// This calls <see cref="GetClosestScreenCornerFromPoint(System.Windows.Forms.Screen, Point)"/> internally by automatically determining the screen to use based on the given <paramref name="pos"/>.
        /// </remarks>
        public static EScreenCorner GetClosestScreenCornerFromPoint(Point pos)
            => GetClosestScreenCornerFromPoint(GetClosestScreenFromPoint(pos), pos);
        #endregion GetClosestScreenCornerFromPoint

        #region ContainsPoint
        /// <summary>
        /// Gets whether the <paramref name="screen"/> boundaries contain the specified <paramref name="point"/> or not.
        /// </summary>
        /// <param name="screen">The <see cref="System.Windows.Forms.Screen"/> instance to use.</param>
        /// <param name="point">An x/y coordinate specifying a point on the <paramref name="screen"/>.</param>
        /// <returns><see langword="true"/> when the <paramref name="screen"/> contains the <paramref name="point"/>; otherwise <see langword="false"/>.</returns>
        public static bool ContainsPoint(this System.Windows.Forms.Screen screen, Point point)
            => screen.Bounds.Contains(point.ToFormsPoint());
        #endregion ContainsPoint
    }
}
