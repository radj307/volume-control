using System;
using System.Windows;
using System.Windows.Input;
using VolumeControl.WPF.MessageHooks;

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
    /// WPF attached event for receiving horizontal scroll events sent by tiltable mouse wheels.
    /// </summary>
    public static class HorizontalScroll
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
            typeof(WpfTiltScrollHook));
        /// <summary>
        /// Adds the specified <paramref name="handler"/> to the <see cref="PreviewMouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void AddPreviewMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
        {
            var inst = (UIElement)d;
            inst.AddHandler(PreviewMouseWheelHorizontalEvent, handler);
            WpfTiltScrollHook.EnableSupportFor(inst);
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
            typeof(WpfTiltScrollHook));
        /// <summary>
        /// Adds the specified <paramref name="handler"/> to the <see cref="MouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void AddMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
        {
            var inst = (UIElement)d;
            inst.AddHandler(MouseWheelHorizontalEvent, handler);
            WpfTiltScrollHook.EnableSupportFor(inst);
        }
        /// <summary>
        /// Removes the specified <paramref name="handler"/> from the <see cref="MouseWheelHorizontalEvent"/>.
        /// </summary>
        public static void RemoveMouseWheelHorizontalHandler(DependencyObject d, MouseWheelHorizontalEventHandler handler)
            => ((UIElement)d).RemoveHandler(MouseWheelHorizontalEvent, handler);
        #endregion MouseWheelHorizontalEvent

        #endregion Events
    }
}
