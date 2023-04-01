using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace VolumeControl.Audio.Interfaces
{
    /// <summary>
    /// Represents an accessible <see cref="IMMNotificationClient"/> wrapper that exposes events.
    /// </summary>
    public interface IDeviceNotificationClient
    {
        #region Events
        /// <summary>
        /// Triggered when a default audio device was changed.
        /// </summary>
        event EventHandler<(MMDevice, Role)>? DefaultDeviceChanged;
        /// <summary>
        /// Triggered when a new audio device was detected.
        /// </summary>
        event EventHandler<MMDevice>? DeviceAdded;
        /// <summary>
        /// Triggered when an audio device was removed.
        /// </summary>
        event EventHandler<string>? DeviceRemoved;
        /// <summary>
        /// Triggered when an audio device changed states.
        /// </summary>
        event EventHandler<MMDevice>? DeviceStateChanged;
        /// <summary>
        /// Triggered when an audio device's properties were changed.
        /// </summary>
        event EventHandler<(string, PropertyKey)>? PropertyValueChanged;
        #endregion Events
    }
}
