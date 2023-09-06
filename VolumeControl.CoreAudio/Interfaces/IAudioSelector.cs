namespace VolumeControl.CoreAudio.Interfaces
{
    /// <summary>
    /// Represents a list item selector.
    /// </summary>
    public interface IAudioSelector
    {
        /// <summary>
        /// Gets or sets the currently selected item by its index in the list.
        /// </summary>
        /// <remarks>
        /// Cannot be changed if <see cref="LockSelection"/> == <see langword="true"/>.
        /// </remarks>
        int SelectedIndex { get; set; }
        /// <summary>
        /// Gets or sets whether the selected item can be changed or not.
        /// </summary>
        /// <returns><see langword="true"/> when the selection cannot be changed; otherwise <see langword="false"/>.</returns>
        bool LockSelection { get; set; }
    }
}
