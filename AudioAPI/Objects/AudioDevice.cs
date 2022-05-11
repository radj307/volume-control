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
    public class AudioDevice : IAudioDevice, IDisposable
    {
        /// <summary>
        /// Audio Device Constructor.<br/>
        /// </summary>
        /// <param name="device">The <see cref="IMMDevice"/> that this audio device wraps.</param>
        public AudioDevice(IMMDevice device)
        {
            _device = device;
            _device.GetId(out _devId);
            // get the device name
            if (_device.OpenPropertyStore(EStorageAccess.READ, out IPropertyStore store) == 0)
            {
                var nameKey = IMMDevice.PKEY_Device_FriendlyName;
                if (store.GetValue(ref nameKey, out PROPVARIANT variant) == 0 && variant.Value is string s)
                    _name = s;
                else _name = string.Empty;
            }
            else _name = string.Empty;
        }

        private readonly IMMDevice _device;
        private readonly string _name;
        private readonly string _devId;
        private bool disposedValue;
        public IMMDevice Interface => _device;
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
            if (_device.GetState(out uint dwState) == 0)
                return (EDeviceState)dwState;
            return EDeviceState.AccessError;
        }


        /// <inheritdoc/>
        public List<AudioSession> GetAudioSessions() => WrapperAPI.GetAllSessions(_device).ToList();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.ReleaseComObject(_device);
                disposedValue = true;
            }
        }

        ~AudioDevice() => Dispose(disposing: false);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
