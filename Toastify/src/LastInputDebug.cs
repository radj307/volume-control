#if DEBUG

using System;
using System.Runtime.InteropServices;
using System.Threading;
// ReSharper disable BuiltInTypeReferenceStyle

namespace Toastify
{
    internal static class LastInputDebug
    {
        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct LASTINPUTINFO
        {
            private static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static void Start()
        {
            var t = new Thread(LastInputCheck);
            t.Start();
        }

        private static void LastInputCheck()
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            while (true)
            {
                GetLastInputInfo(ref lastInputInfo);

                var idleTime = (uint)Environment.TickCount - lastInputInfo.dwTime;

                System.Diagnostics.Debug.WriteLine("Idle Time: " + ((idleTime > 0) ? (idleTime / 1000) : 0) + "secs (dwTime: " + lastInputInfo.dwTime + ")");

                Thread.Sleep(10000);
            }
        }
    }
}

#else

namespace Toastify
{
    internal static class LastInputDebug
    {
        public static void Start() { }
    }
}

#endif