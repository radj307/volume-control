using System.ComponentModel;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents an audio device.
    /// </summary>
    public interface IDevice : IAudioControllable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the device name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the device ID.
        /// </summary>
        string DeviceID { get; }
    }
}
