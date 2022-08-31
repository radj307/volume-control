using NAudio.CoreAudioApi;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>Base interface type for <see cref="IDevice"/> and <see cref="IProcess"/>/<see cref="ISession"/>.</summary>
    /// <remarks>This is an empty interface.</remarks>
    public interface IAudioControl : IVolumeControl
    {
        /// <summary>
        /// Interface used for displaying audio peaking meters in a GUI.
        /// </summary>
        AudioMeterInformation AudioMeterInformation { get; }
    }
}
