using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace VolumeControl.WPF.MessageHooks
{
    /// <summary>
    /// Provides a window message hook that adds support for horizontal scrolling with tiltable mouse wheels.
    /// </summary>
    /// <remarks>
    /// Adds an attached event to all UIElements that triggers when the user tilts the mouse scroll wheel.
    /// </remarks>
    public static class WpfTiltScrollHook
    {
        #region Properties
        private static readonly HashSet<IntPtr> _hookedHwnds = new();
        /// <summary>
        /// Gets the <see cref="HwndSourceHook"/> delegate that handles horizontal scroll messages.
        /// </summary>
        public static HwndSourceHook Hook { get; } = WndProcHook;
        #endregion Properties

        #region Methods

        #region (Private) WndProcHook
        private const int WM_MOUSEHWHEEL = 0x020E;
        private static IntPtr WndProcHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case WM_MOUSEHWHEEL:
                HandleMouseWheelHorizontal(GetHorizontalDelta(wParam));
                break;
            }
            return IntPtr.Zero;
        }
        #endregion (Private) WndProcHook

        #region (Private) GetHorizontalDelta
        private static int GetHorizontalDelta(IntPtr value)
        {
            return unchecked(-(short)((uint)(IntPtr.Size == 8 ? (int)value.ToInt64() : value.ToInt32()) >> 16));
        }
        #endregion (Private) GetHorizontalDelta

        #region (Private) HandleMouseWheelHorizontal
        private static void HandleMouseWheelHorizontal(int delta)
        {
            if (delta == 0) return;

            var element = Mouse.DirectlyOver;
            if (element == null) return;

            if (element is not UIElement)
            {
                if (VisualTreeHelpers.FindAncestorWithType<UIElement>((DependencyObject)element) is not UIElement ancestorElement)
                    return;
                element = ancestorElement;
            }

            // raise PreviewMouseWheelHorizontal event
            var eventArgs = new MouseWheelHorizontalEventArgs(Mouse.PrimaryDevice, Environment.TickCount, delta)
            {
                RoutedEvent = HorizontalScroll.PreviewMouseWheelHorizontalEvent
            };

            element.RaiseEvent(eventArgs);
            if (eventArgs.Handled) return; //< preview handled the event

            // raise MouseWheelHorizontal event
            eventArgs.RoutedEvent = HorizontalScroll.MouseWheelHorizontalEvent;
            element.RaiseEvent(eventArgs);
        }
        #endregion (Private) HandleMouseWheelHorizontal

        #region (Private) GetParentWindowHandle
        private static IntPtr GetWindowHandle(Window window) => new WindowInteropHelper(window).Handle;
        private static IntPtr? GetWindowHandle(DependencyObject d)
            => (PresentationSource.FromDependencyObject(d) as HwndSource)?.Handle;
        #endregion (Private) GetParentWindowHandle

        #region (Private) EnableSupport
        private static bool EnableSupport(IntPtr handle)
        {
            if (_hookedHwnds.Contains(handle) || handle == IntPtr.Zero)
                return true;

            var source = HwndSource.FromHwnd(handle);
            if (source == null)
                return false;

            _hookedHwnds.Add(handle);
            source.AddHook(WndProcHook);
            return true;
        }
        private static bool EnableSupport(IntPtr? handle) => handle.HasValue && EnableSupport(handle.Value);
        #endregion (Private) EnableSupport

        #region (Internal) EnableSupportFor
        internal static bool EnableSupportFor(UIElement uiElement)
        {
            if (uiElement is Window window)
            {
                if (window.IsLoaded)
                    return EnableSupport(GetWindowHandle(window));
                else
                { // enable support when loaded
                    window.Loaded += (s, e) => EnableSupport(GetWindowHandle((Window)s));
                    return true;
                }
            }
            else if (uiElement is Popup popup)
            {
                // enable support when a new popup window opens
                popup.Opened += (s, e) => EnableSupport(GetWindowHandle((Popup)s!));

                // enable support now if it's already open
                return !popup.IsOpen || EnableSupport(GetWindowHandle(popup));
            }
            else if (uiElement is ContextMenu contextMenu)
            {
                // enable support when a new contextmenu window opens
                contextMenu.Opened += (s, e) => EnableSupport(GetWindowHandle((ContextMenu)s));

                // enable support now if it's already open
                return !contextMenu.IsOpen || EnableSupport(GetWindowHandle(contextMenu));
            }
            else if (GetWindowHandle(uiElement) is IntPtr hWnd)
            {
                // add handler in case the parent window changes
                PresentationSource.AddSourceChangedHandler(uiElement, new((s, e) => EnableSupport((e.NewSource as HwndSource)?.Handle)));

                // enable support now
                return EnableSupport(hWnd);
            }
            else return false;
        }
        #endregion (Private) EnableSupportFor

        #endregion Methods
    }
}
