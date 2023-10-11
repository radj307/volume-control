namespace VolumeControl.CoreAudio.Interfaces
{
    /// <summary>
    /// Represents a list item multi-selector.
    /// </summary>
    public interface IAudioMultiSelector
    {
        /// <summary>
        /// Gets or sets the selection state of each AudioSession instance in the Sessions list.
        /// </summary>
        /// <remarks>
        /// This list is always the same length and order as the <see cref="AudioSessionManager.Sessions"/> list.
        /// </remarks>
        IReadOnlyList<bool> SelectionStates { get; set; }
        /// <summary>
        /// Gets or sets the current index of the selector in the Sessions list.
        /// </summary>
        int CurrentIndex { get; set; }
        /// <summary>
        /// Gets or sets whether the CurrentIndex can be changed.
        /// </summary>
        /// <returns><see langword="true"/> when the CurrentIndex can be changed; otherwise <see langword="false"/>.</returns>
        bool LockCurrentIndex { get; set; }
        /// <summary>
        /// Gets or sets whether the selected items can be changed.
        /// </summary>
        /// <returns><see langword="true"/> when the selected items can be changed; otherwise <see langword="false"/>.</returns>
        bool LockSelection { get; set; }
    }
}
