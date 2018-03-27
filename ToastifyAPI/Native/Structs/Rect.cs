using System.Runtime.InteropServices;

namespace ToastifyAPI.Native.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width { get { return this.right - this.left; } }

        public int Height { get { return this.bottom - this.top; } }

        public override string ToString()
        {
            return $"{{X={this.left},Y={this.top},Width={this.Width},Height={this.Height}}}";
        }
    }
}