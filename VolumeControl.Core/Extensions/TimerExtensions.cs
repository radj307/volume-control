namespace VolumeControl.Core.Extensions
{
    public static class TimerExtensions
    {
        /// <summary>
        /// Restarts the timer without triggering a <see cref="System.Windows.Forms.Timer.Tick"/> event.
        /// </summary>
        /// <param name="t">Timer instance.</param>
        public static void Restart(this System.Windows.Forms.Timer t) => t.Enabled = !(t.Enabled = !t.Enabled);
    }
}
