using AudioAPI.API;
using AudioAPI.Objects;
using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Interfaces;
using System.Runtime.InteropServices;

namespace AudioAPI.Implementations
{
    /// <summary>
    /// Wrapper object for the <see cref="IMMDevice"/> COM object that implements the <see cref="IDisposable"/> interface to call <see cref="Marshal.ReleaseComObject(object)"/>.
    /// </summary>
    /// <remarks>This object is designed for using statements, and should not be stored for longer than absolutely necessary.</remarks>
    public class MMDevice : IDisposable
    {
        public MMDevice(string endpointID) => Device = WrapperAPI.GetDevice(endpointID);

        private MMDevice() => Device = null!;
        public static MMDevice GetDefaultDevice()
        {
            MMDevice mmDev = new();
            mmDev.Device = WrapperAPI.GetDefaultDevice();
            return mmDev;
        }

        private IMMDevice Device;
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Marshal.ReleaseComObject(Device);
                disposedValue = true;
            }
        }

        ~MMDevice() => Dispose(disposing: false);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <returns>Raw <see cref="IAudioSessionManager2"/> COM object, or null if an error occurred.</returns>
        /// <inheritdoc cref="IMMDevice.Activate"/>
        internal IAudioSessionManager2? Activate(ref Guid targetTypeGuid, int dwClsCtx, IntPtr pActivationParams)
        {
            if (Device.Activate(ref targetTypeGuid, dwClsCtx, pActivationParams, out object ppInterface) == 0)
                return (IAudioSessionManager2)ppInterface;
            return null;
        }

        /// <returns>The device's <see cref="IPropertyStore"/>.</returns>
        /// <inheritdoc cref="IMMDevice.OpenPropertyStore"/>
        public IPropertyStore OpenPropertyStore(EStorageAccess accessMode)
        {
            _ = Device.OpenPropertyStore(accessMode, out IPropertyStore propertyStore);
            return propertyStore;
        }

        /// <returns>The Endpoint ID string.</returns>
        /// <inheritdoc cref="IMMDevice.GetId(out string)"/>
        public string GetId()
        {
            _ = Device.GetId(out string id);
            return id;
        }

        /// <returns><see cref="EDeviceState"/>, or <see cref="EDeviceState.AccessError"/> if an error occurred.</returns>
        /// <inheritdoc cref="IMMDevice.GetState"/>
        public EDeviceState GetState()
        {
            if (Device.GetState(out uint state) == 0)
                return (EDeviceState)state;
            return EDeviceState.AccessError;
        }

        public List<AudioSession> GetSessions() => WrapperAPI.GetAllSessions(Device);
    }
}
