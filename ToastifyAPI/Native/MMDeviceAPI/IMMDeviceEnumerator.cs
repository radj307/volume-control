using System.Runtime.InteropServices;
using ToastifyAPI.Native.MMDeviceAPI.Enums;

namespace ToastifyAPI.Native.MMDeviceAPI
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceEnumerator
    {
        [PreserveSig]
        int EnumAudioEndpoints(EDataFlow dataFlow, EDeviceState stateMask, [Out] out IMMDeviceCollection deviceCollection);

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
    }
}