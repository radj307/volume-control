using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using VolumeControl.Audio.Collections;
using VolumeControl.Audio.Interfaces;

namespace VolumeControl.Audio.Events
{
    /// <summary>Receives audio device events from the core audio api.<br/>These events relate to <b>all</b> audio devices.</summary>
    /// <remarks>This is used by the top-tier <see cref="AudioAPI"/> object, and is used for receiving Core Audio API events relating to audio devices, such as state change events, removal events, etc.<br/>For more information, see the <see cref="IMMNotificationClient"/> interface, or its MSDN documentation here: <see href="https://docs.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immnotificationclient"/></remarks>
    internal sealed class DeviceNotificationClient : IDeviceNotificationClient, IMMNotificationClient
    {
        #region Constructors
        public DeviceNotificationClient(AudioDeviceCollection deviceManager)
        {
            this.DataFlow = deviceManager.DataFlow;
            this.MMDeviceEnumerator = deviceManager.MMDeviceEnumerator;
        }
        #endregion Constructors

        #region Properties
        private readonly MMDeviceEnumerator MMDeviceEnumerator;
        public DataFlow DataFlow { get; }
        #endregion Properties

        #region Events
        public event EventHandler<(MMDevice, Role)>? DefaultDeviceChanged;
        public event EventHandler<MMDevice>? DeviceAdded;
        public event EventHandler<string>? DeviceRemoved;
        public event EventHandler<MMDevice>? DeviceStateChanged;
        public event EventHandler<(string, PropertyKey)>? PropertyValueChanged;
        #endregion Events

        #region Interface Methods
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            var mmDevice = MMDeviceEnumerator.GetDevice(defaultDeviceId);
            DefaultDeviceChanged?.Invoke(this, (mmDevice, role));
        }
        public void OnDeviceAdded(string pwstrDeviceId)
        {
            var mmDevice = MMDeviceEnumerator.GetDevice(pwstrDeviceId);
            DeviceAdded?.Invoke(this, mmDevice);
        }
        public void OnDeviceRemoved(string deviceId) => DeviceRemoved?.Invoke(this, deviceId);
        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            var mmDevice = MMDeviceEnumerator.GetDevice(deviceId);
            DeviceStateChanged?.Invoke(this, mmDevice);
        }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            PropertyValueChanged?.Invoke(this, (pwstrDeviceId, key));
        }
        #endregion Interface Methods
    }
}
