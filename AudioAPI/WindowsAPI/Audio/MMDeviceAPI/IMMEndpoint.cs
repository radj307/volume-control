using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI
{
    [Guid("1BE09788-6894-4089-8586-9A2A6C265AC5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMMEndpoint
    {
        [PreserveSig]
        public int GetDataFlow(out EDataFlow dataFlow);
    }
}