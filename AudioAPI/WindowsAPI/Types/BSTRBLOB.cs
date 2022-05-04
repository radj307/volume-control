using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Types
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BSTRBLOB
    {
        [FieldOffset(0)] public uint cbSize;
        [FieldOffset(4)] public IntPtr pData;
    }
}
