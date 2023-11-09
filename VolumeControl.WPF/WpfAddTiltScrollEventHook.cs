using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Event arguments for horizontal mouse wheel events.
    /// </summary>
    public class MouseWheelHorizontalEventArgs : MouseEventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="MouseWheelHorizontalEventArgs"/> instance with the specified parameters.
        /// </summary>
        /// <param name="mouse">The mouse device associated with this event.</param>
        /// <param name="timestamp">The time when the input occurred.</param>
        /// <param name="horizontalDelta">The amount the wheel has changed.</param>
        public MouseWheelHorizontalEventArgs(MouseDevice mouse, int timestamp, int horizontalDelta) : base(mouse, timestamp)
        {
            HorizontalDelta = horizontalDelta;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets a value that indicates the amount that the mouse wheel has changed.
        /// </summary>
        /// <returns>
        /// The amount the wheel has changed. This value is positive if the mouse wheel is 
        /// tilted to the left or negative if the mouse wheel is tilted to the right.
        /// </returns>
        public int HorizontalDelta { get; }
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
            => ((MouseWheelHorizontalEventHandler)genericHandler)(genericTarget, this);
        #endregion Methods
    }
    /// <summary>
    /// Event handler type for the MouseWheelHorizontal &amp; PreviewMouseWheelHorizontal events.
    /// </summary>
    /// <param name="sender">The control that the user horizontally scrolled over top of.</param>
    /// <param name="e">Event arguments containing the distance scrolled, where </param>
    public delegate void MouseWheelHorizontalEventHandler(object sender, MouseWheelHorizontalEventArgs e);
    /// <summary>
    /// Provides a window message hook that adds support for horizontal scrolling with tiltable mouse wheels.
    /// </summary>
    /// <remarks>
    /// Adds an attached event to all UIElements that triggers when the user tilts the mouse scroll wheel.
    /// </remarks>
    public static class WpfAddTiltScrollEventHook
    {
        #region Events

        #region PreviewMouseWheelHorizontalEvent
        /// <summary>
        /// Occurs when the user tilts the mouse wheel while the mouse pointer is over this element.
        /// </summary>
        public static readonly RoutedEvent PreviewMouseWheelHorizontalEvent = EventManager.RegisterRoutedEvent(
            "PreviewMouseWheelHorizontal",
            RoutingStrategy.Tunnel,
            typeof(MouseWheelHorizontalEventHandler),
            typeof(WpfAddTiltScrollEventHook));
        /// <summary>
        /// Adds the specified <paramref name="handler"/> to the <see cref="PreviewMouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void AddPreviewMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
        {
            var inst = (UIElement)d;
            inst.AddHandler(PreviewMouseWheelHorizontalEvent, handler);
            EnableSupportFor(inst);
        }
        /// <summary>
        /// Removes the specified <paramref name="handler"/> from the <see cref="PreviewMouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void RemovePreviewMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
            => ((UIElement)d).RemoveHandler(PreviewMouseWheelHorizontalEvent, handler);
        #endregion PreviewMouseWheelHorizontalEvent

        #region MouseWheelHorizontalEvent
        /// <summary>
        /// Occurs when the user tilts the mouse wheel while the mouse pointer is over this element.
        /// </summary>
        public static readonly RoutedEvent MouseWheelHorizontalEvent = EventManager.RegisterRoutedEvent(
            "MouseWheelHorizontal",
            RoutingStrategy.Bubble,
            typeof(MouseWheelHorizontalEventHandler),
            typeof(WpfAddTiltScrollEventHook));
        /// <summary>
        /// Adds the specified <paramref name="handler"/> to the <see cref="MouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void AddMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
        {
            var inst = (UIElement)d;
            inst.AddHandler(MouseWheelHorizontalEvent, handler);
            EnableSupportFor(inst);
        }
        /// <summary>
        /// Removes the specified <paramref name="handler"/> from the <see cref="MouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void RemoveMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
            => ((UIElement)d).RemoveHandler(MouseWheelHorizontalEvent, handler);
        #endregion MouseWheelHorizontalEvent

        #endregion Events

        #region Properties
        private static readonly HashSet<IntPtr> _hookedHwnds = new();
        #endregion Properties

        #region Methods

        #region GetHook
        /// <summary>
        /// Gets the <see cref="WpfAddTiltScrollEventHook"/> <see cref="HwndSourceHook"/> instance.
        /// </summary>
        public static HwndSourceHook GetHook() => WndProcHook;
        #endregion GetHook

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

            var eventArgs = new MouseWheelHorizontalEventArgs(Mouse.PrimaryDevice, Environment.TickCount, delta) { RoutedEvent = PreviewMouseWheelHorizontalEvent };

            element.RaiseEvent(eventArgs);
            if (eventArgs.Handled) return;

            eventArgs.RoutedEvent = MouseWheelHorizontalEvent;
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

        #region (Private) EnableSupportFor
        private static bool EnableSupportFor(UIElement uiElement)
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
