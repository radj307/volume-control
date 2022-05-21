namespace VolumeControl.Extensions
{
    public static class TimerExtensions
    {
        public static void Reset(this System.Windows.Forms.Timer timer)
        {
            if (timer.Enabled)
            {
                timer.Enabled = false;
                timer.Enabled = true;
            }
        }
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
