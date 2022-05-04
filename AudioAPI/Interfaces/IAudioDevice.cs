namespace AudioAPI.Interfaces
{
    public interface IAudioDevice
    {
        /// <summary>
        /// Gets whether or not this is a virtual (non-existent) device.
        /// </summary>
        bool Virtual { get; }
        /// <summary>
        /// Gets the device name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the device ID.
        /// </summary>
        string DeviceID { get; }
        /// <summary>
        /// Gets all current audio sessions on this device.
        /// </summary>
        /// <returns>A list of audio sessions.</returns>
        List<IAudioSession> GetAudioSessions();
    }
}
