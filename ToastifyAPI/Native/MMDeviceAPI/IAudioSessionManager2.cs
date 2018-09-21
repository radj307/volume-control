using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.MMDeviceAPI
{
    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionManager2
    {
        int NotImpl1();

        int NotImpl2();

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator sessionEnum);
    }
}