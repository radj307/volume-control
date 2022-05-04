using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(EDataFlow dataFlow, EDeviceState stateMask, [Out] out IMMDeviceCollection deviceCollection);

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);


        /// <summary>
        /// The EnumAudioEndpoints method generates a collection of audio endpoint devices that meet the specified criteria.
        /// </summary>
        /// <param name="dataFlow">The data-flow direction for the endpoint devices in the collection. The caller should set this parameter to one of the following EDataFlow enumeration values:
        /// <list type="bullet"><item><description>eRender</description></item><item><description>eCapture</description></item><item><description>eAll</description></item></list>
        /// If the caller specifies eAll, the method includes both rendering and capture endpoints in the collection.
        /// </param>
        /// <param name="dwStateMask">The state or states of the endpoints that are to be included in the collection. The caller should set this parameter to the bitwise OR of one or more of the following DEVICE_STATE_XXX constants:
        /// <list type="bullet">
        /// <item><description>DEVICE_STATE_ACTIVE</description></item>
        /// <item><description>DEVICE_STATE_DISABLED</description></item>
        /// <item><description>DEVICE_STATE_NOTPRESENT</description></item>
        /// <item><description>DEVICE_STATE_UNPLUGGED</description></item>
        /// </list>
        /// For example, if the caller sets the dwStateMask parameter to DEVICE_STATE_ACTIVE | DEVICE_STATE_UNPLUGGED, the method includes endpoints that are either active or unplugged from their jacks, but excludes endpoints that are on audio adapters that have been disabled or are not present. To include all endpoints, regardless of state, set dwStateMask = DEVICE_STATEMASK_ALL.</param>
        /// <param name="ppDevices">Pointer to a pointer variable into which the method writes the address of the IMMDeviceCollection interface of the device-collection object. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the EnumAudioEndpoints call fails, *ppDevices is NULL.</param>
        /// <returns><list>
        /// <item>E_POINTER<term></term><description> Parameter ppDevices is NULL. </description></item>
        /// <item>E_INVALIDARG<term></term><description> Parameter dataFlow or dwStateMask is out of range. </description></item>
        /// <item>E_OUTOFMEMORY<term></term><description>  Out of memory. </description></item>
        /// </list></returns>
        [PreserveSig]
        int EnumAudioEndpoints(EDataFlow dataFlow, int dwStateMask, out IMMDeviceCollection ppDevices);


        /// <summary>
        /// The GetDevice method retrieves an audio endpoint device that is identified by an endpoint ID string.
        /// </summary>
        /// <remarks>If two programs are running in two different processes and both need to access the same audio endpoint device, one program cannot simply pass the device's IMMDevice interface to the other program. However, the programs can access the same device by following these steps:
        ///
        ///        The first program calls the IMMDevice::GetId method in the first process to obtain the endpoint ID string that identifies the device.
        ///        The first program passes the endpoint ID string across the process boundary to the second program.
        ///        To obtain a reference to the device's IMMDevice interface in the second process, the second program calls GetDevice with the endpoint ID string.
        ///
        ///
        ///    For more information about the GetDevice method, see the following topics:
        ///
        ///
        ///        Endpoint ID Strings
        ///        Audio Events for Legacy Audio Applications
        ///
        ///    For code examples that use the GetDevice method, see the following topics:
        ///
        ///
        ///        Device Properties
        ///
        ///        Device Events
        ///
        ///        Using the IKsControl Interface to Access Audio Properties
        ///</remarks>
        /// <param name="endpointID">Pointer to a string containing the endpoint ID. The caller typically obtains this string from the IMMDevice::GetId method or from one of the methods in the IMMNotificationClient interface.</param>
        /// <param name="ppDevice">Pointer to a pointer variable into which the method writes the address of the IMMDevice interface for the specified device. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the GetDevice call fails, *ppDevice is NULL.</param>
        /// <returns>
        /// <list type="number">
        /// <item><term>E_POINTER</term><description>Parameter pwstrId or ppDevice is NULL.</description></item>
        /// <item><term>E_NOTFOUND</term><description>The device ID does not identify an audio device that is in this system.</description></item>
        /// <item><term>E_OUTOFMEMORY</term><description>Out of memory.</description></item>
        /// </list>
        /// </returns>
        [PreserveSig]
        int GetDevice(string endpointID, out IMMDevice ppDevice);
    }
}