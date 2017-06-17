using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Toastify
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    internal class Win32API
    {
        private static readonly IntPtr desktopHandle = GetDesktopWindow();
        private static readonly IntPtr shellHandle = GetShellWindow();

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        #region DLL imports

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out Rect rc);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        #endregion DLL imports

        private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error;
            IntPtr result;
            // Win32API SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if (result == IntPtr.Zero && error != 0)
                throw new System.ComponentModel.Win32Exception(error);

            return result;
        }

        public static void KillProc(string name)
        {
            // let's play nice and try to gracefully clear out all Sync processes
            var procs = Process.GetProcessesByName(name);

            foreach (var proc in procs)
            {
                // lParam == Band Process Id, passed in below
                EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
                    {
                        GetWindowThreadProcessId(hWnd, out uint processId);

                        // Essentially: Find every hWnd associated with this process and ask it to go away
                        if (processId == (uint)lParam)
                        {
                            SendMessage(hWnd, Constants.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            SendMessage(hWnd, Constants.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                        }

                        return true;
                    },
                    (IntPtr)proc.Id);
            }

            // let everything calm down
            Thread.Sleep(1000);

            procs = Process.GetProcessesByName(name);

            // ok, no more mister nice guy. Sadly.
            foreach (var proc in procs)
            {
                try
                {
                    proc.Kill();
                }
                catch
                {
                    // ignore exceptions (usually due to trying to kill non-existant child processes
                }
            }
        }

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        public static void AddToolWindowStyle(System.Windows.Window window)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(window);
            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GwlExstyle);
            exStyle |= (int)ExtendedWindowStyles.WsExToolwindow;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GwlExstyle, (IntPtr)exStyle);
        }

        public static bool IsAnyAppRunningInFullscreen()
        {
            // Get the dimensions of the active window.
            IntPtr hWnd = GetForegroundWindow();
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // Check we haven't picked up the desktop or the shell.
                if (!hWnd.Equals(desktopHandle) && !hWnd.Equals(shellHandle))
                {
                    GetWindowRect(hWnd, out Rect appBounds);

                    // Determine if window is fullscreen.
                    Rectangle screenBounds = Screen.FromHandle(hWnd).Bounds;
                    if (appBounds.bottom - appBounds.top == screenBounds.Height && appBounds.right - appBounds.left == screenBounds.Width)
                        return true;
                }
            }
            return false;
        }

        public static void SendPasteKey()
        {
            var shiftKey = new ManagedWinapi.KeyboardKey(Keys.ShiftKey);
            var altKey = new ManagedWinapi.KeyboardKey(Keys.Alt);
            var ctrlKey = new ManagedWinapi.KeyboardKey(Keys.ControlKey);
            var vKey = new ManagedWinapi.KeyboardKey(Keys.V);

            // Before injecting a paste command, first make sure that no modifiers are already
            // being pressed (which will throw off the Ctrl+v).
            // Since key state is notoriously unreliable, set a max sleep so that we don't get stuck
            var maxSleep = 250;

            // minimum sleep time
            Thread.Sleep(150);

            //System.Diagnostics.Debug.WriteLine("shift: " + shiftKey.State + " alt: " + altKey.State + " ctrl: " + ctrlKey.State);

            while (maxSleep > 0 && (shiftKey.State != 0 || altKey.State != 0 || ctrlKey.State != 0))
                Thread.Sleep(maxSleep -= 50);

            //System.Diagnostics.Debug.WriteLine("maxSleep: " + maxSleep);

            // Press keys in sequence. Don't use PressAndRelease since that seems to be too fast for most applications and the sequence gets lost.
            ctrlKey.Press();
            vKey.Press();
            Thread.Sleep(25);
            vKey.Release();
            Thread.Sleep(25);
            ctrlKey.Release();
        }

        #region Enums

        [Flags]
        internal enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,

            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,

            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,

            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,

            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,

            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,

            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,

            /// <summary>Retains the current position (ignores X and y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,

            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,

            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,

            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,

            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,

            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,

            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,

            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        [Flags]
        public enum ExtendedWindowStyles
        {
            WsExToolwindow = 0x00000080,
        }

        public enum GetWindowLongFields
        {
            GwlExstyle = -20,
        }

        [Flags]
        public enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        #endregion Enums

        #region Internal classes and structs

        internal struct WindowPlacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        internal class Constants
        {
            internal const uint WM_APPCOMMAND = 0x0319;

            internal const int SW_SHOWMINIMIZED = 2;
            internal const int SW_SHOWNOACTIVATE = 4;
            internal const int SW_SHOWMINNOACTIVE = 7;
            internal const int SW_SHOW = 5;
            internal const int SW_RESTORE = 9;

            internal const int WM_CLOSE = 0x10;
            internal const int WM_QUIT = 0x12;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion Internal classes and structs
    }
}