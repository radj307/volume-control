namespace VolumeControl.Core.Controls.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Scale(this decimal n, decimal nMin, decimal nMax, decimal outMin, decimal outMax)
        {
            return nMin + (n - nMin) * (outMax - outMin) / (nMax - nMin);
        }
    }
}
