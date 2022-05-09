using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Types;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI
{

    /// <summary>
    /// The IMMNotificationClient interface provides notifications when an audio endpoint device is added or removed, when the state or properties of an endpoint device change, or when there is a change in the default role assigned to an endpoint device. Unlike the other interfaces in this section, which are implemented by the MMDevice API system component, an MMDevice API client implements the IMMNotificationClient interface. To receive notifications, the client passes a pointer to its IMMNotificationClient interface instance as a parameter to the IMMDeviceEnumerator::RegisterEndpointNotificationCallback method.<br/>
    /// After registering its IMMNotificationClient interface, the client receives event notifications in the form of callbacks through the methods of the interface.<br/>
    /// Each method in the IMMNotificationClient interface receives, as one of its input parameters, an endpoint ID string that identifies the audio endpoint device that is the subject of the notification.The string uniquely identifies the device with respect to all of the other audio endpoint devices in the system.The methods in the IMMNotificationClient interface implementation should treat this string as opaque.That is, none of the methods should attempt to parse the contents of the string to obtain information about the device.The reason is that the string format is undefined and might change from one implementation of the MMDevice API system module to the next.<br/>
    /// A client can use the endpoint ID string that it receives as an input parameter in a call to an IMMNotificationClient method in two ways:<br/>
    /// The client can create an instance of the device that the endpoint ID string identifies. The client does this by calling the IMMDeviceEnumerator::GetDevice method and supplying the endpoint ID string as an input parameter.<br/>
    /// The client can compare the endpoint ID string with the endpoint ID string of an existing device instance. To obtain the second endpoint ID string, the client calls the IMMDevice::GetId method of the device instance. If the two strings match, they identify the same device.<br/>
    /// In implementing the IMMNotificationClient interface, the client should observe these rules to avoid deadlocks and undefined behavior:<br/>
    /// The methods of the interface must be nonblocking.The client should never wait on a synchronization object during an event callback.<br/>
    /// To avoid dead locks, the client should never call IMMDeviceEnumerator::RegisterEndpointNotificationCallback or IMMDeviceEnumerator::UnregisterEndpointNotificationCallback in its implementation of IMMNotificationClient methods.<br/>
    /// The client should never release the final reference on an MMDevice API object during an event callback.<br/>
    /// For a code example that implements the IMMNotificationClient interface, see Device Events.
    /// </summary>
    [Guid("7991eec9-7e89-4d85-8390-6c703cec60c0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMNotificationClient
    {
        /// <summary>
        /// The OnDefaultDeviceChanged method notifies the client that the default audio endpoint device for a particular device role has changed.
        /// </summary>
        /// <remarks>
        /// The three input parameters specify the data-flow direction, device role, and endpoint ID string of the new default audio endpoint device.<br/>
        /// In Windows Vista, the MMDevice API supports device roles but the system-supplied user interface programs do not.The user interface in Windows Vista enables the user to select a default audio device for rendering and a default audio device for capture.When the user changes the default rendering or capture device, the system assigns all three device roles(eConsole, eMultimedia, and eCommunications) to the new device.Thus, when the user changes the default rendering or capture device, the system calls the client's OnDefaultDeviceChanged method three times—once for each of the three device roles.<br/>
        /// In a future version of Windows, the user interface might enable the user to assign individual roles to different devices.In that case, if the user changes the assignment of only one or two device roles to a new rendering or capture device, the system will call the client's OnDefaultDeviceChanged method only once or twice (that is, one call per changed role). Depending on how the OnDefaultDeviceChanged method responds to role changes, the behavior of an audio application developed to run in Windows Vista might change when run in a future version of Windows. For more information, see Device Roles in Windows Vista.
        /// For a code example that implements the OnDefaultDeviceChanged method, see Device Events.
        /// </remarks>
        /// <param name="flow">The data-flow direction of the endpoint device. This parameter is set to one of the following <see cref="EDataFlow"/> enumeration values: <see cref="EDataFlow.ERender"/> or <see cref="EDataFlow.ECapture"/>. The data-flow direction for a rendering device is eRender. The data-flow direction for a capture device is eCapture.</param>
        /// <param name="role">The device role of the audio endpoint device. This parameter is set to one of the following <see cref="ERole"/> enumeration values: <see cref="ERole.EConsole"/>, <see cref="ERole.EMultimedia"/>, or <see cref="ERole.ECommunications"/>.</param>
        /// <param name="pwstrDefaultDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call. If the user has removed or disabled the default device for a particular role, and no other device is available to assume that role, then pwstrDefaultDevice is NULL.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public int OnDefaultDeviceChanged(EDataFlow flow, ERole role, [MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);

        /// <summary>
        /// The OnDeviceAdded method indicates that a new audio endpoint device has been added.
        /// </summary>
        /// <param name="pwstrDefaultDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public int OnDeviceAdded([MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);

        /// <summary>
        /// The OnDeviceRemoved method indicates that an audio endpoint device has been removed.
        /// </summary>
        /// <remarks>For a code example that implements the OnDeviceRemoved method, see <see href="https://docs.microsoft.com/en-us/windows/desktop/CoreAudio/device-events">Device Events</see></remarks>
        /// <param name="pwstrDefaultDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public int OnDeviceRemoved([MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId);

        /// <summary>
        /// The OnDeviceStateChanged method indicates that the state of an audio endpoint device has changed.
        /// </summary>
        /// <param name="pwstrDefaultDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <param name="dwNewState">Specifies the new <see cref="EDeviceState"/> of the endpoint device.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public int OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId, uint dwNewState);


        /// <summary>
        /// The OnPropertyValueChanged method indicates that the value of a property belonging to an audio endpoint device has changed.
        /// </summary>
        /// <remarks>
        /// A call to the IPropertyStore::SetValue method that successfully changes the value of a property of an audio endpoint device generates a call to OnPropertyValueChanged. For more information about IPropertyStore::SetValue, see the Windows SDK documentation.<br/>
        /// A client can use the key parameter to retrieve the new property value.For a code example that uses a property key to retrieve a property value from the property store of an endpoint device, see Device Properties.<br/>
        /// For a code example that implements the OnPropertyValueChanged method, see <see href="https://docs.microsoft.com/en-us/windows/desktop/CoreAudio/device-events">Device Events</see>.
        /// </remarks>
        /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string that contains the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <param name="key">A <see cref="PROPERTYKEY"/> structure that specifies the property. The structure contains the property-set GUID and an index identifying a property within the set. The structure is passed by value. It remains valid for the duration of the call. For more information about PROPERTYKEY, see the Windows SDK documentation.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public int OnPropertyValueChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PROPERTYKEY key);

        /// <inheritdoc cref="OnDeviceStateChanged(string, uint)"/>
        public int OnDeviceStateChanged([MarshalAs(UnmanagedType.LPWStr)] string pwstrDefaultDeviceId, EDeviceState newState);
    }
}
