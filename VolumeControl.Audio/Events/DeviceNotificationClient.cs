using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace VolumeControl.Audio.Events
{
    #region DeviceEvents
    /// <summary>Event arguments for events related to the addition or removal of audio devices.</summary>
    public class DeviceAddedRemovedEventArgs : EventArgs
    {
        /// <inheritdoc cref="DeviceAddedRemovedEventArgs"/>
        /// <param name="deviceID">The <see cref="AudioDevice.DeviceID"/> of the device that was added or removed.</param>
        public DeviceAddedRemovedEventArgs(string deviceID) => DeviceID = deviceID;
        /// <inheritdoc cref="AudioDevice.DeviceID"/>
        public string DeviceID { get; }
    }
    /// <summary>Event arguments for events related to device state change events.</summary>
    public class DeviceStateChangedEventArgs : DeviceAddedRemovedEventArgs
    {
        /// <param name="deviceID"><see cref="AudioDevice.DeviceID"/></param>
        /// <inheritdoc cref="DeviceStateChangedEventArgs"/>
        /// <param name="state"><see cref="AudioDevice.State"/></param>
        public DeviceStateChangedEventArgs(string deviceID, DeviceState state) : base(deviceID) => State = state;
        /// <inheritdoc cref="AudioDevice.State"/>
        public DeviceState State { get; }
    }
    /// <summary>Event arguments for events related to device state change events.</summary>
    public class DefaultDeviceChangedEventArgs : DeviceAddedRemovedEventArgs
    {
        /// <param name="deviceID"><see cref="AudioDevice.DeviceID"/></param>
        /// <inheritdoc cref="DeviceStateChangedEventArgs"/>
        public DefaultDeviceChangedEventArgs(string deviceID, DataFlow flow, Role role) : base(deviceID)
        {
            DataFlow = flow;
            Role = role;
        }
        /// <summary>
        /// The device's <see cref="DataFlow"/>.<br/>
        /// Determines whether the device is an input or output device.
        /// </summary>
        public DataFlow DataFlow { get; }
        /// <summary>
        /// The device's <see cref="Role"/>.<br/>
        /// Determines the type of device.
        /// </summary>
        public Role Role { get; }
    }
    /// <summary>Event arguments for events related to device property change events.</summary>
    public class DevicePropertyChangedEventArgs : DeviceAddedRemovedEventArgs
    {
        /// <param name="deviceID"><see cref="AudioDevice.DeviceID"/></param>
        /// <inheritdoc cref="DeviceStateChangedEventArgs"/>
        /// <param name="property">The new property value.</param>
        public DevicePropertyChangedEventArgs(string deviceID, PropertyKey property) : base(deviceID) => Property = property;
        /// <summary>
        /// The device property that was changed.<br/>
        /// This contains the new value.
        /// </summary>
        public PropertyKey Property { get; }
    }
    public delegate void DeviceAddedRemovedEventHandler(object? sender, DeviceAddedRemovedEventArgs e);
    public delegate void DefaultDeviceChangedEventHandler(object? sender, DefaultDeviceChangedEventArgs e);
    public delegate void DeviceStateChangedEventHandler(object? sender, DeviceStateChangedEventArgs e);
    public delegate void DevicePropertyValueChangedEventHandler(object? sender, DevicePropertyChangedEventArgs e);
    #endregion DeviceEvents

    /// <summary>Core audio event interface.</summary>
    public class DeviceNotificationClient : IMMNotificationClient
    {
        #region DeviceEvents
        /// <summary>
        /// Triggered when any default device is changed regardless of its <see cref="Role"/> or <see cref="DataFlow"/>.<br/>
        /// This event is not fired because of the <see cref="DefaultDevice"/> property, but it is used to update it; <b>not to notify clients of it being changed.</b>
        /// </summary>
        /// <remarks>This event is routed from the windows API.</remarks>
        public event DefaultDeviceChangedEventHandler? DefaultDeviceChanged;
        /// <summary>
        /// Triggered when an audio device is added to the windows API.
        /// </summary>
        /// <remarks>This event is routed from the windows API.</remarks>
        public event DeviceAddedRemovedEventHandler? DeviceAdded;
        /// <summary>
        /// Triggered when an audio device is removed from the windows API.
        /// </summary>
        /// <remarks>This event is routed from the windows API.</remarks>
        public event DeviceAddedRemovedEventHandler? DeviceRemoved;
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This event is routed from the windows API.</remarks>
        public event DeviceStateChangedEventHandler? DeviceStateChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This event is routed from the windows API.</remarks>
        public event DevicePropertyValueChangedEventHandler? DevicePropertyValueChanged;
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId) => DefaultDeviceChanged?.Invoke(this, new(defaultDeviceId, flow, role));
        public void OnDeviceAdded(string pwstrDeviceId) => DeviceAdded?.Invoke(this, new(pwstrDeviceId));
        public void OnDeviceRemoved(string deviceId) => DeviceRemoved?.Invoke(this, new(deviceId));
        public void OnDeviceStateChanged(string deviceId, DeviceState newState) => DeviceStateChanged?.Invoke(this, new(deviceId, newState));
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) => DevicePropertyValueChanged?.Invoke(this, new(pwstrDeviceId, key));
        #endregion DeviceEvents
    }
}
