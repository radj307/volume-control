using AudioAPI.API;
using AudioAPI.Interfaces;
using AudioAPI.Objects.Virtual;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Interfaces;
using AudioAPI.WindowsAPI.Struct;
using System.Runtime.InteropServices;

namespace AudioAPI.Objects
{
    /// <summary>
    /// Represents a real audio device, as opposed to a <see cref="VirtualAudioDevice"/>.<br/>
    /// An audio device is anything that Windows' CoreAudio APIs recognize as a device using the <see cref="IMMDevice"/> interface.
    /// </summary>
    /// <remarks>This is a wrapper for the <see cref="IMMDevice"/> interface, and implements <see cref="IAudioDevice"/> and <see cref="IDisposable"/>.</remarks>
    public class AudioDevice : IAudioDevice
    {
        /// <summary>
        /// Audio Device Constructor.<br/>
        /// </summary>
        /// <param name="device">The <see cref="IMMDevice"/> that this audio device wraps.</param>
        public AudioDevice(IMMDevice device)
        {
            device.GetId(out _devId);
            // get the device name
            if (device.OpenPropertyStore(EStorageAccess.READ, out IPropertyStore store) == 0)
            {
                var nameKey = IMMDevice.PKEY_Device_FriendlyName;
                if (store.GetValue(ref nameKey, out PROPVARIANT variant) == 0 && variant.Value is string s)
                    _name = s;
                else _name = string.Empty;
            }
            else _name = string.Empty;
        }

        private IMMDevice Device => WrapperAPI.GetDevice(DeviceID);
        private readonly string _name;
        private readonly string _devId;

        /// <inheritdoc/>
        public string Name => _name;
        /// <inheritdoc/>
        public string DeviceID => _devId;

        /// <inheritdoc/>
        /// <remarks>This object is never virtual.</remarks>
        public bool Virtual => false;

        /// <summary>
        /// Gets the current state of the device.
        /// </summary>
        /// <returns><see cref="EDeviceState"/> with the current device state, or <see cref="EDeviceState.AccessError"/> if an error occurred.</returns>
        public EDeviceState GetState()
        {
            if (Device.GetState(out var dwState) == 0)
                return (EDeviceState)dwState;
            return EDeviceState.AccessError;
        }


        /// <inheritdoc/>
        public List<AudioSession> GetAudioSessions() => WrapperAPI.GetAllSessions(Device).ToList();
    }
}
