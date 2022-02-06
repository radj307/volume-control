using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI
{
    [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMDeviceCollection
    {
        [PreserveSig]
        int GetCount(out int deviceCount);

        [PreserveSig]
        int Item(int deviceIndex, [Out] out IMMDevice device);
    }
}