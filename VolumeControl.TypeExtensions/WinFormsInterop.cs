namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods for WinForms-WPF interoperation.
    /// </summary>
    public static class WinFormsInterop
    {
        /// <summary>
        /// Converts the <see cref="System.Windows.Media.Color"/> to the equivalent <see cref="System.Drawing.Color"/>.
        /// </summary>
        public static System.Drawing.Color ToFormsColor(this System.Windows.Media.Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        /// <summary>
        /// Converts the <see cref="System.Drawing.Color"/> to the equivalent <see cref="System.Windows.Media.Color"/>.
        /// </summary>
        public static System.Windows.Media.Color ToWpfColor(this System.Drawing.Color color) => System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}
