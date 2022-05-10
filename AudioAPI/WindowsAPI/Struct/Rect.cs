using System.Runtime.InteropServices;

namespace AudioAPI.WindowsAPI.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width => right - left;

        public int Height => bottom - top;

        /// <inheritdoc/>
        public override string ToString() => $"{{X={left},Y={top},Width={Width},Height={Height}}}";
    }
}