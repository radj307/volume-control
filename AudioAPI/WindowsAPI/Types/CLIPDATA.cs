using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct CLIPDATA
    {
        [FieldOffset(0)] public uint cbSize;
        [FieldOffset(4)] public int ulClipFmt;
        [FieldOffset(8)] public byte[] pClipData;
    }
}
