using System.Runtime.InteropServices;

namespace VolumeControl.WPF
{
    [StructLayout(LayoutKind.Sequential)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}