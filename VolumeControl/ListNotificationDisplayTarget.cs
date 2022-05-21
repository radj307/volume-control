namespace VolumeControl
{
    /// <summary>
    /// Defines the possible display modes for the <see cref="ListNotification"/> window.
    /// </summary>
    public enum ListNotificationDisplayTarget
    {
        /// <summary>
        /// Doesn't show anything in the list panel.
        /// </summary>
        None,
        /// <summary>
        /// Shows audio sessions in the list panel, and highlights the currently selected one.
        /// </summary>
        Sessions,
        /// <summary>
        /// Shows audio devices in the list panel, and highlights the currently selected one.
        /// </summary>
        Devices,
    }
}
