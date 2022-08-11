using NAudio.CoreAudioApi;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>Base interface type for <see cref="IDevice"/> and <see cref="IProcess"/>/<see cref="ISession"/>.</summary>
    /// <remarks>This is an empty interface.</remarks>
    public interface IAudioControllable
    {
        /// <summary>
        /// Interface used for displaying audio peaking meters in a GUI.
        /// </summary>
        AudioMeterInformation AudioMeterInformation { get; }
        /// <summary>
        /// Gets the current peak meter level.
        /// </summary>
        float PeakMeterValue { get; }
        /// <summary>
        /// Gets or sets the volume of this <see cref="IAudioControllable"/> instance as a <see cref="float"/> in the range <b>( 0.0 <i>(0%)</i> - 1.0 <i>(100%)</i> )</b><br/>
        /// This is the native volume range of this instance, into which the <see cref="Volume"/> property's value is scaled.
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        float NativeVolume { get; set; }
        /// <summary>
        /// Gets or sets the volume of this <see cref="IAudioControllable"/> instance as an <see cref="int"/> in the range <b>( 0 - 100 )</b>
        /// </summary>
        /// <remarks>Input values are clamped <b>before</b> being applied, so out-of-bounds values are allowed.</remarks>
        int Volume { get; set; }
        /// <summary>
        /// Gets or sets this <see cref="IAudioControllable"/> instance's current mute-state.
        /// </summary>
        /// <returns><see langword="true"/> when this instance is muted; otherwise <see langword="false"/>.</returns>
        bool Muted { get; set; }
    }
}
