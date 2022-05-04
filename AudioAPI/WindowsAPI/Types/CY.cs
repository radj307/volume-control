using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct CY
    {
        [FieldOffset(0)] public uint Lo;
        [FieldOffset(4)] public int Hi;
        [FieldOffset(8)] public long int64;
    }
}
