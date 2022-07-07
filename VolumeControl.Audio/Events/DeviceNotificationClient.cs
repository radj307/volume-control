using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using VolumeControl.Audio.Collections;

namespace VolumeControl.Audio.Events
{
    /// <summary>Receives audio device events from the core audio api.<br/>These events relate to <b>all</b> audio devices.</summary>
    /// <remarks>This is used by the top-tier <see cref="AudioAPI"/> object, and is used for receiving Core Audio API events relating to audio devices, such as state change events, removal events, etc.<br/>For more information, see the <see cref="IMMNotificationClient"/> interface, or its MSDN documentation here: <see href="https://docs.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immnotificationclient"/></remarks>
    internal sealed class DeviceNotificationClient : IMMNotificationClient
    {
        public DeviceNotificationClient(params AudioDeviceCollection[] deviceCollections) => DeviceCollections = deviceCollections;

        private readonly AudioDeviceCollection[] DeviceCollections;
        private AudioDevice? FindWithDeviceID(string deviceID)
        {
            foreach (AudioDeviceCollection? collection in DeviceCollections)
            {
                if (collection.FirstOrDefault(d => d is not null && d.DeviceID.Equals(deviceID, StringComparison.Ordinal), null) is AudioDevice dev)
                    return dev;
            }

            return null;
        }

        #region DeviceEvents
        public event EventHandler<AudioDevice>? DefaultDeviceChanged;
        public event EventHandler<AudioDevice>? GlobalDeviceAdded;
        public event EventHandler<string>? GlobalDeviceRemoved;
        public event EventHandler<AudioDevice>? GlobalDeviceStateChanged;
        public event EventHandler<(AudioDevice device, PropertyKey property)>? GlobalDevicePropertyValueChanged;

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (!role.Equals(Role.Multimedia))
                return;
            foreach (AudioDeviceCollection? collection in DeviceCollections)
            {
                if (collection.DataFlow.HasFlag(flow))
                {
                    int index = collection.IndexOf(defaultDeviceId);
                    if (index != -1)
                    {
                        DefaultDeviceChanged?.Invoke(this, collection.Default = collection[index]);
                        return;
                    }
                }
            }
        }
        public void OnDeviceAdded(string pwstrDeviceId)
        {
            using var enumerator = new MMDeviceEnumerator();
            MMDevice? mmDevice = enumerator.GetDevice(pwstrDeviceId);
            enumerator.Dispose();
            DataFlow flow = mmDevice.DataFlow;
            AudioDevice? dev = null;
            foreach (AudioDeviceCollection? collection in DeviceCollections)
            { // find a collection that accepts devices with this dataflow type:
                if (collection.DataFlow.HasFlag(flow))
                {
                    dev = collection.CreateDeviceFromMMDevice(mmDevice);
                    break;
                }
            }
            if (dev != null)
                GlobalDeviceAdded?.Invoke(this, dev);
        }
        public void OnDeviceRemoved(string deviceId)
        {
            this.FindWithDeviceID(deviceId)?.ForwardRemoved(this, EventArgs.Empty);
            GlobalDeviceRemoved?.Invoke(this, deviceId);
        }
        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            if (this.FindWithDeviceID(deviceId) is AudioDevice device)
            {
                device.ForwardStateChanged(this, newState);
                GlobalDeviceStateChanged?.Invoke(this, device);
            }
            else // existing but previously ignored device was activated:
            {
                using var enumerator = new MMDeviceEnumerator();
                MMDevice? mmDevice = enumerator.GetDevice(deviceId);
                enumerator.Dispose();
                DataFlow flow = mmDevice.DataFlow;
                AudioDevice? dev = null;
                foreach (AudioDeviceCollection? collection in DeviceCollections)
                {
                    if (collection.DataFlow.HasFlag(flow))
                    {
                        dev = collection.CreateDeviceFromMMDevice(mmDevice);
                        break;
                    }
                }
                if (dev != null)
                    GlobalDeviceAdded?.Invoke(this, dev);
            }
        }
        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            if (this.FindWithDeviceID(pwstrDeviceId) is AudioDevice device)
            {
                device.ForwardPropertyStoreChanged(this, key);
                GlobalDevicePropertyValueChanged?.Invoke(this, (device, key));
            }
        }
        #endregion DeviceEvents
    }
}
