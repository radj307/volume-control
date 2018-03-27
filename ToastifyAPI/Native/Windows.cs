using ManagedWinapi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using ToastifyAPI.Helpers;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native
{
    public static class Windows
    {
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            return IntPtr.Size == 4 ? User32.GetWindowLongPtr32(hWnd, nIndex) : User32.GetWindowLongPtr64(hWnd, nIndex);
        }

        internal static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 4 ? new IntPtr(User32.SetWindowLongPtr32(hWnd, nIndex, dwNewLong.ToInt32())) : User32.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

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

        private static void AddWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwLong)
        {
            long longPtr = (long)GetWindowLongPtr(hWnd, nIndex);
            longPtr |= (long)dwLong;
            SetWindowLongPtr(hWnd, nIndex, (IntPtr)longPtr);
        }

        public static void AddToolWindowStyle(IntPtr hWnd)
        {
            AddWindowLongPtr(hWnd, GWL.GWL_EXSTYLE, (IntPtr)ExtendedWindowStylesFlags.WS_EX_TOOLWINDOW);
        }

        public static void AddVisibleWindowStyle(IntPtr hWnd)
        {
            AddWindowLongPtr(hWnd, GWL.GWL_EXSTYLE, (IntPtr)WindowStylesFlags.WS_VISIBLE);
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
            return User32.GetWindowText(hWnd, lpString, nMaxCount) > 0 ? lpString.ToString() : String.Empty;
        }

        public static bool SendWindowMessage(IntPtr hWnd, WindowsMessagesFlags msg, IntPtr wParam, IntPtr lParam, bool postMessage = false)
        {
            return postMessage ? User32.PostMessage(hWnd, (uint)msg, wParam, lParam) : User32.SendMessage(hWnd, (uint)msg, wParam, lParam);
        }

        public static bool SendAppCommandMessage(IntPtr hWnd, IntPtr lParam, bool postMessage = false)
        {
            return SendWindowMessage(hWnd, WindowsMessagesFlags.WM_APPCOMMAND, IntPtr.Zero, lParam, postMessage);
        }

        public static bool SendKeyDown(IntPtr hWnd, Key key, bool postMessage = false, bool extended = false)
        {
            const WindowsMessagesFlags msg = WindowsMessagesFlags.WM_KEYDOWN;
            IntPtr wParam = (IntPtr)key.GetVirtualKey();
            IntPtr lParam = (IntPtr)key.GetLParam(extended: (byte)(extended ? 1 : 0));

            return SendWindowMessage(hWnd, msg, wParam, lParam, postMessage);
        }

        public static bool SendKeyUp(IntPtr hWnd, Key key, bool postMessage = false, bool extended = false)
        {
            const WindowsMessagesFlags msg = WindowsMessagesFlags.WM_KEYUP;
            IntPtr wParam = (IntPtr)key.GetVirtualKey();
            IntPtr lParam = (IntPtr)key.GetLParam(extended: (byte)(extended ? 1 : 0), previousState: 1, transitionState: 1);

            return SendWindowMessage(hWnd, msg, wParam, lParam, postMessage);
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