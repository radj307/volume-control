using System.Drawing;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native.Structs
{
    public struct WindowPlacement
    {
        public int length;
        public int flags;
        public ShowWindowCmd showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public Rectangle rcNormalPosition;

        public override string ToString()
        {
            return $"{this.length},{this.flags},{this.showCmd},{this.ptMinPosition},{this.ptMaxPosition},{this.rcNormalPosition}";
        }
    }
}