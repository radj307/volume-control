namespace VolumeControl.SDK.Internal
{
    /// <summary>
    /// Defines events that are used by the program to perform various actions.
    /// </summary>
    /// <remarks>
    /// To trigger these events from an addon, use the methods found in <see cref="VCAPI"/>.
    /// </remarks>
    public static class VCEvents
    {
        /// <summary>
        /// Indicates that the SessionListNotification window should be shown.
        /// </summary>
        public static event EventHandler? ShowSessionListNotification;
        internal static void NotifyShowSessionListNotification(object? sender, EventArgs e)
            => ShowSessionListNotification?.Invoke(sender, e);
        /// <summary>
        /// Indicates that the DeviceListNotification window should be shown.
        /// </summary>
        public static event EventHandler? ShowDeviceListNotification;
        internal static void NotifyShowDeviceListNotification(object? sender, EventArgs e)
            => ShowDeviceListNotification?.Invoke(sender, e);
    }
}
