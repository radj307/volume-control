namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extensions for the <see cref="System.Windows.Forms.Timer"/> timer object.
    /// </summary>
    public static class TimerExtensions
    {
        /// <summary>
        /// Resets the timer back to default if it was already started; otherwise does nothing.
        /// </summary>
        /// <param name="timer">The timer instance this method is called on.</param>
        public static void Reset(this System.Windows.Forms.Timer timer)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
                timer.Enabled = true;
            }
        }
        /// <summary>
        /// Resets the timer back to default if it was already started; otherwise the timer is started.
        /// </summary>
        /// <param name="timer">The timer instance this method is called on.</param>
        public static void StartOrReset(this System.Windows.Forms.Timer timer)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
                timer.Enabled = true;
            }
            else
            {
                timer.Enabled = true;
            }
        }
    }
}
