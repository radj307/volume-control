using System;

namespace VolumeControl.WPF.CustomMessageBox
{
    /// <summary>
    /// Specifies special display options for a <see cref="CustomMessageBox"/>.
    /// </summary>
    [Flags]
    public enum CustomMessageBoxOptions
    {
        /// <summary>
        /// No options are set.
        /// </summary>
        None = 0,
        /// <summary>
        /// The message box text and title bar caption are right-aligned.
        /// </summary>
        RightAlign = 524288,
        /// <summary>
        /// All text, buttons, icons, and title bars are displayed right-to-left.
        /// </summary>
        RtlReading = 1048576,
    }
}
