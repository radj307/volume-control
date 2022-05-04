using AudioAPI.Interfaces;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Interfaces;

namespace AudioAPI
{
    /// <summary>
    /// Represents a real audio device.
    /// </summary>
    public class AudioDevice : IAudioDevice
    {
        public AudioDevice(IMMDevice device)
        {
            _device = device;
            _device.GetId(out _devId);
            // get the device name
            if (_device.OpenPropertyStore(EStorageAccess.READ, out IPropertyStore store) == 0)
            {
                var nameKey = IMMDevice.PKEY_Device_FriendlyName;
                store.GetValue(ref nameKey, out IntPtr value);
                _name = value as object as string ?? string.Empty;
            }
            else _name = string.Empty;
        }

        private readonly IMMDevice _device;
        private readonly string _name;
        private readonly string _devId;

        public string Name => _name;
        public string DeviceID => _devId;

        public bool Virtual => false;

        public List<IAudioSession> GetAudioSessions() => Volume.GetAllSessions(_device).Cast<IAudioSession>().ToList();
    }
}
