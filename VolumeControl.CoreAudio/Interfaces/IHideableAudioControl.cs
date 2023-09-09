namespace VolumeControl.CoreAudio.Interfaces
{
    /// <summary>
    /// Represents an audio instance that can be prevented from becoming selected.
    /// </summary>
    public interface IHideableAudioControl : IAudioControl
    {
        /// <summary>
        /// Gets or sets whether this audio instance is hidden.
        /// </summary>
        bool IsHidden { get; set; }
    }
}
