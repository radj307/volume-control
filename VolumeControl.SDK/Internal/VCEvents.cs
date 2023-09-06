namespace VolumeControl.SDK.Internal
{
    public static class VCEvents
    {
        public static event EventHandler? ShowSessionListNotification;
        internal static void NotifyShowSessionListNotification(object? sender, EventArgs e)
            => ShowSessionListNotification?.Invoke(sender, e);
    }
}
