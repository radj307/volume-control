using ManagedWinapi;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace VolumeControl
{
    public static class Windows
    {

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                User32.EnumChildWindows(
                    parent,
                    (hWnd, lParam) =>
                    {
                        GCHandle gch = GCHandle.FromIntPtr(lParam);
                        if (!(gch.Target is List<IntPtr> list))
                            throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
                        list.Add(hWnd);
                        return true;
                    },
                    GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        public static List<IntPtr> GetProcessWindows(uint processId)
        {
            return GetProcessWindows(processId, null);
        }

        public static List<IntPtr> GetProcessWindows(uint processId, string lpClassName)
        {
            List<IntPtr> result = new List<IntPtr>();
            User32.EnumWindows((hWnd, lParam) =>
            {
                User32.GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == processId)
                {
                    if (lpClassName != null && GetClassName(hWnd) == lpClassName)
                        result.Add(hWnd);
                    else if (lpClassName == null)
                        result.Add(hWnd);
                }
                return true;
            }, IntPtr.Zero);
            return result;
        }

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            User32.GetClassName(hWnd, sb, 256);
            return sb.ToString();
        }

        /// <summary>
        /// Finds a thread's window by its class name.
        /// </summary>
        /// <param name="dwThreadId"> The id of the thread. </param>
        /// <param name="lpClassName"> The class name. </param>
        /// <returns> A handle to the window. </returns>
        public static IntPtr FindThreadWindowByClassName(uint dwThreadId, string lpClassName)
        {
            IntPtr searchedHWnd = IntPtr.Zero;

            User32.EnumThreadWindows(
                dwThreadId,
                (hWnd, lParam) =>
                {
                    if (hWnd == IntPtr.Zero)
                        return true;

                    string className = GetClassName(hWnd);
                    if (className == lpClassName)
                    {
                        searchedHWnd = hWnd;
                        return false;
                    }
                    return true;
                },
                IntPtr.Zero);

            return searchedHWnd;
        }

        public static uint GetProcessFromWindowHandle(IntPtr hWnd)
        {
            User32.GetWindowThreadProcessId(hWnd, out uint pid);
            return pid;
        }

        public static string GetWindowTitle(IntPtr hWnd)
        {
            const int nMaxCount = 256;
            StringBuilder lpString = new StringBuilder(nMaxCount);

            if (User32.GetWindowText(hWnd, lpString, nMaxCount) > 0)
                return lpString.ToString();

            int errorCode = Marshal.GetLastWin32Error();
            return errorCode == 0 ? string.Empty : null;
        }

        public static void SendPasteKey()
        {
            var shiftKey = new KeyboardKey(Keys.ShiftKey);
            var altKey = new KeyboardKey(Keys.Alt);
            var ctrlKey = new KeyboardKey(Keys.ControlKey);
            var vKey = new KeyboardKey(Keys.V);

            // Before injecting a paste command, first make sure that no modifiers are already
            // being pressed (which will throw off the Ctrl+v).
            // Since key state is notoriously unreliable, set a max sleep so that we don't get stuck
            var maxSleep = 250;

            // minimum sleep time
            Thread.Sleep(150);

            while (maxSleep > 0 && (shiftKey.State != 0 || altKey.State != 0 || ctrlKey.State != 0))
            {
                Thread.Sleep(maxSleep);
                maxSleep -= 50;
            }

            // Press keys in sequence. Don't use PressAndRelease since that seems to be too fast for most applications and the sequence gets lost.
            ctrlKey.Press();
            vKey.Press();
            Thread.Sleep(25);
            vKey.Release();
            Thread.Sleep(25);
            ctrlKey.Release();
        }
    }
}