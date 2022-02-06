using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Audio
{
    [Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioSessionControl2
    {
        [PreserveSig]
        int NotImpl0();
        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        [PreserveSig]
        int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        [PreserveSig]
        int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        [PreserveSig]
        int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string value, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        [PreserveSig]
        int GetGroupingParam(out Guid pRetVal);
        [PreserveSig]
        int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
        [PreserveSig]
        int NotImpl1();
        [PreserveSig]
        int NotImpl2();
        [PreserveSig]
        int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        [PreserveSig]
        int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);
        [PreserveSig]
        int GetProcessId(out int pRetVal);
        [PreserveSig]
        int IsSystemSoundsSession();
        [PreserveSig]
        int SetDuckingPreference(bool optOut);
    }
}