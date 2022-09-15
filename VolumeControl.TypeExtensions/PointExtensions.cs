namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extends the <see cref="System.Drawing.Point"/> &amp; <see cref="System.Windows.Point"/> structs.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Deconstruct method for <see cref="System.Windows.Point"/>.
        /// </summary>
        public static void Deconstruct(this System.Windows.Point p, out double x, out double y)
        {
            x = p.X;
            y = p.Y;
        }
        /// <summary>
        /// Deconstruct method for <see cref="System.Drawing.Point"/>.
        /// </summary>
        public static void Deconstruct(this System.Drawing.Point p, out int x, out int y)
        {
            x = p.X;
            y = p.Y;
        }
        /// <summary>
        /// Get a <see cref="System.Drawing.Point"/> instance from a <see cref="System.Windows.Point"/> instance.
        /// </summary>
        public static System.Drawing.Point ToFormsPoint(this System.Windows.Point p) => new((int)p.X, (int)p.Y);
        /// <summary>
        /// Get a <see cref="System.Drawing.Point"/> instance from a <see cref="System.Windows.Point"/> instance.
        /// </summary>
        public static System.Windows.Point ToWpfPoint(this System.Drawing.Point p) => new((int)p.X, (int)p.Y);
    }
}
