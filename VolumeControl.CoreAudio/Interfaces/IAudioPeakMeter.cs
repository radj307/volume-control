namespace Audio.Interfaces
{
    /// <summary>
    /// Represents some kind of object that has an audio peak meter.
    /// </summary>
    public interface IAudioPeakMeter
    {
        /// <summary>
        /// The current peak meter value.
        /// </summary>
        float PeakMeterValue { get; }
    }
}