namespace AudioAPI.WindowsAPI.Enum
{
    [Flags]
    public enum EMenuFlags : long
    {
        MF_BYCOMMAND = 0x0000L,
        MF_ENABLED = 0x0000L,
        MF_GRAYED = 0x0001L,
        MF_DISABLED = 0x0002L,
        MF_BYPOSITION = 0x0400L
    }
}