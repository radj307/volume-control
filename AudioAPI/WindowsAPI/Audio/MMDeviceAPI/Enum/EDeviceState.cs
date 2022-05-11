namespace AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum
{
    [Flags]
    public enum EDeviceState : uint
    {
        /// <summary>
        /// This is a custom enumeration with the value 0 that indicates an error having occurred while accessing the device state.
        /// </summary>
        /// <remarks>If this is returned from a method, the device is likely invalid.</remarks>
        AccessError = 0x0,
        Active = 0x00000001,
        Disabled = 0x00000002,
        NotPresent = 0x00000004,
        UnPlugged = 0x00000008,
        All = UnPlugged | NotPresent | Disabled | Active // 0x0000000F
    }
}