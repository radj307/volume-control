using System.ComponentModel;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents an audio device.
    /// </summary>
    public interface IDevice : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the device name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the device ID.
        /// </summary>
        string DeviceID { get; }
        /// <summary>
        /// Gets or sets the volume level of the device, using the native <see cref="float"/> range 0.0 - 1.0
        /// </summary>
        float NativeEndpointVolume { get; set; }
        /// <summary>
        /// Gets or sets the volume level of the device, using the <see cref="int"/> range 0 - 100
        /// </summary>
        int EndpointVolume { get; set; }
        /// <summary>
        /// Gets or sets the mute state of the device endpoint.
        /// </summary>
        bool EndpointMuted { get; set; }
    }
}
