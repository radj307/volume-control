namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum
{
    [Flags]
    public enum EDeviceState
    {
        Active = 0x00000001,
        Disabled = 0x00000002,
        NotPresent = 0x00000004,
        UnPlugged = 0x00000008,
        All = UnPlugged | NotPresent | Disabled | Active // 0x0000000F
    }
}