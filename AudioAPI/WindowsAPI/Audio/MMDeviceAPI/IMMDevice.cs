using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Interfaces;
using AudioAPI.WindowsAPI.Struct;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI
{
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDevice
    {
        /// <summary>
        /// The Activate method creates a COM object with the specified interface.
        /// </summary>
        /// <param name="iid">The interface identifier. This parameter is a reference to a GUID that identifies the interface that the caller requests be activated. The caller will use this interface to communicate with the COM object. Set this parameter to one of the following interface identifiers: [ IID_IAudioClient, IID_IAudioEndpointVolume, IID_IAudioMeterInformation, IID_IAudioSessionManager, IID_IAudioSessionManager2, IID_IBaseFilter, IID_IDeviceTopology, IID_IDirectSound, IID_IDirectSound8, IID_IDirectSoundCapture, IID_IDirectSoundCapture8, IID_IMFTrustedOutput, IID_ISpatialAudioClient, IID_ISpatialAudioMetadataClient ]. For more information, see Remarks.</param>
        /// <param name="dwClsCtx">The execution context in which the code that manages the newly created object will run. The caller can restrict the context by setting this parameter to the bitwise OR of one or more CLSCTX enumeration values. Alternatively, the client can avoid imposing any context restrictions by specifying CLSCTX_ALL. For more information about CLSCTX, see the Windows SDK documentation.</param>
        /// <param name="pActivationParams">Set to NULL to activate an IAudioClient, IAudioEndpointVolume, IAudioMeterInformation, IAudioSessionManager, or IDeviceTopology interface on an audio endpoint device. When activating an IBaseFilter, IDirectSound, IDirectSound8, IDirectSoundCapture, or IDirectSoundCapture8 interface on the device, the caller can specify a pointer to a PROPVARIANT structure that contains stream-initialization information. For more information, see Remarks.</param>
        /// <param name="ppInterface">Pointer to a pointer variable into which the method writes the address of the interface specified by parameter iid. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the Activate call fails, *ppInterface is NULL.</param>
        /// <returns>If the method succeeds, it returns 0.</returns>
        [PreserveSig]
        public int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        /// <summary>
        /// The OpenPropertyStore method retrieves an interface to the device's property store.
        /// </summary>
        /// <param name="stgmAccess">The storage-access mode. This parameter specifies whether to open the property store in read mode, write mode, or read/write mode.</param>
        /// <param name="properties">The <see cref="IPropertyStore"/> containing this device's properties.</param>
        /// <returns>If the method succeeds, it returns 0.</returns>
        [PreserveSig]
        public int OpenPropertyStore(EStorageAccess stgmAccess, out IPropertyStore properties);
        /// <summary>
        /// The GetId method retrieves an endpoint ID string that identifies the audio endpoint device.
        /// </summary>
        /// <param name="id">Output string that the endpoint ID string will be written to.</param>
        /// <returns>If the method succeeds, it returns 0.</returns>
        [PreserveSig]
        public int GetId([MarshalAs(UnmanagedType.LPWStr)] out string id);
        /// <summary>
        /// The GetState method retrieves the current device state.
        /// </summary>
        /// <param name="state">Output variable containing the <see cref="EDeviceState"/> as an unsigned integer (DWORD).</param>
        /// <returns>If the method succeeds, it returns 0.</returns>
        [PreserveSig]
        public int GetState(out uint state);

        /// <summary>
        /// The friendly name of the audio adapter to which the endpoint device is attached (for example, "XYZ Audio Adapter"). PROPVARIANT member vt is set to VT_LPWSTR and member pwszVal points to a null-terminated, wide-character string that contains the friendly name.
        /// </summary>
        /// <remarks>
        /// All audio devices have this property.<br/>
        /// This is a <see cref="PROPERTYKEY"/> for use with the <see cref="IPropertyStore"/> interface returned by <see cref="OpenPropertyStore(EStorageAccess, out IPropertyStore)"/> method.
        /// </remarks>
        public static readonly PROPERTYKEY PKEY_DeviceInterface_FriendlyName = new() { formatId = new Guid("026e516e-b814-414b-83cd-856d6fef4822"), propertyId = 2, };
        /// <summary>
        /// The device description of the endpoint device (for example, "Speakers"). PROPVARIANT member vt is set to VT_LPWSTR and member pwszVal points to a null-terminated, wide-character string that contains the device description.
        /// </summary>
        /// <remarks>
        /// All audio devices have this property.<br/>
        /// This is a <see cref="PROPERTYKEY"/> for use with the <see cref="IPropertyStore"/> interface returned by <see cref="OpenPropertyStore(EStorageAccess, out IPropertyStore)"/> method.
        /// </remarks>
        public static readonly PROPERTYKEY PKEY_Device_DeviceDesc = new() { formatId = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), propertyId = 2, };
        /// <summary>
        /// The friendly name of the endpoint device (for example, "Speakers (XYZ Audio Adapter)"). PROPVARIANT member vt is set to VT_LPWSTR and member pwszVal points to a null-terminated, wide-character string that contains the friendly name.
        /// </summary>
        /// <remarks>
        /// All audio devices have this property.<br/>
        /// This is a <see cref="PROPERTYKEY"/> for use with the <see cref="IPropertyStore"/> interface returned by <see cref="OpenPropertyStore(EStorageAccess, out IPropertyStore)"/> method.
        /// </remarks>
        public static readonly PROPERTYKEY PKEY_Device_FriendlyName = new() { formatId = new Guid("a45c254e-df1c-4efd-8020-67d146a850e0"), propertyId = 14 };
    }
}