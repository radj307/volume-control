using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Toastify.Helpers;

// ReSharper disable InconsistentNaming
namespace Toastify
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    internal static class Win32API
    {
        private static readonly IntPtr hDesktop = GetDesktopWindow();
        private static readonly IntPtr hProgman = GetShellWindow();
        private static readonly IntPtr hShellDll = GetShellDllDefViewWindow();

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        #region DLL imports

        [DllImport("user32.dll")]
        internal static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCmd nCmdShow);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionStateFlags SetThreadExecutionState(ExecutionStateFlags esFlags);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, MapVirtualKeyType uMapType);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKeyEx(uint uCode, MapVirtualKeyType uMapType, IntPtr dwhkl);

        #endregion DLL imports

        #region Native methods' wrappers

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumChildWindows(
                    parent,
                    (hWnd, lParam) =>
                    {
                        GCHandle gch = GCHandle.FromIntPtr(lParam);
                        List<IntPtr> list = gch.Target as List<IntPtr>;
                        if (list == null)
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

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetClassName(hWnd, sb, 256);
            return sb.ToString();
        }

        #endregion

        internal static IntPtr GetShellDllDefViewWindow()
        {
            IntPtr _SHELLDLL_DefView = FindWindowEx(hProgman, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (_SHELLDLL_DefView.Equals(IntPtr.Zero))
            {
                EnumWindows((hWnd, lParam) =>
                {
                    if (GetClassName(hWnd) == "WorkerW")
                    {
                        IntPtr child = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                        if (child != IntPtr.Zero)
                        {
                            _SHELLDLL_DefView = child;
                            return false;
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            }
            return _SHELLDLL_DefView;
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

            EnumThreadWindows(
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
                            SendWindowMessage(hWnd, WindowsMessagesFlags.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            SendWindowMessage(hWnd, WindowsMessagesFlags.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
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

        public static bool IsForegroundAppAFullscreenVideogame()
        {
            // Get the dimensions of the active window.
            IntPtr hWnd = GetForegroundWindow();

            Debug.WriteLine($"[Win32API::IsForegroundAppAFullscreenVideogame] hWnd=0x{hWnd.ToInt64():X8}, className=\"{GetClassName(hWnd)}\"");

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // Check we haven't picked up the desktop or the shell or the parent of one of them (Progman, WorkerW)
                List<IntPtr> childWindows = GetChildWindows(hWnd);
                if (!hWnd.Equals(hDesktop) && !hWnd.Equals(hProgman) && !hWnd.Equals(hShellDll) &&
                    !childWindows.Contains(hDesktop) && !childWindows.Contains(hProgman) && !childWindows.Contains(hShellDll))
                {
                    Rectangle screenRect = Screen.FromHandle(hWnd).Bounds;
                    GetClientRect(hWnd, out Rect clientRect);
                    
                    if (clientRect.Height == screenRect.Height && clientRect.Width == screenRect.Width)
                    {
                        var processId = GetProcessFromWindowHandle(hWnd);
                        var modules = GetProcessModules(processId);
                        if (modules != null)
                        {
                            Regex regex_d3d = new Regex(@"(?:(?:d3d[0-9]+)|(?:dxgi))\.dll", RegexOptions.IgnoreCase);

                            // ReSharper disable once LoopCanBeConvertedToQuery
                            foreach (var module in modules)
                            {
                                bool isDirectX = regex_d3d.Match(module.ModuleName).Success;
                                bool isOpenGL = module.ModuleName.Equals("opengl32.dll", StringComparison.InvariantCultureIgnoreCase);
                                if (isDirectX || isOpenGL)
                                {
                                    Debug.WriteLine($"[Win32API::IsForegroundAppAFullscreenVideogame] Fullscreen videogame: \"{GetProcessName(processId)}\", module found: \"{module.ModuleName}\"");
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
            }
            return false;
        }

        private static uint GetProcessFromWindowHandle(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out uint pid);
            return pid;
        }

        private static string GetProcessName(uint processId)
        {
            if (processId == 0)
                return null;

            var process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }

        private static IEnumerable<ProcessModule> GetProcessModules(uint processId)
        {
            if (processId == 0)
                return null;

            var process = Process.GetProcessById((int)processId);
            return process.Modules.Cast<ProcessModule>();
        }

        /// <summary>
        /// Checks if a window is minimized.
        /// </summary>
        /// <param name="hWnd"> The window handle. </param>
        /// <param name="toTray">
        /// Whether to check if the window is minimized to the tray or not.
        /// If this parameter is true and the window is minimized to the taskbar, then it returns false;
        /// if this parameter is false and the window is minimized to the tray, then it returns true.
        /// </param>
        /// <returns> Whether the window is minimized or not. </returns>
        public static bool IsWindowMinimized(IntPtr hWnd, bool toTray = false)
        {
            if (hWnd == IntPtr.Zero)
                return false;

            WindowStylesFlags windowStyles = (WindowStylesFlags)GetWindowLongPtr(hWnd, GWL.GWL_STYLE);
            if ((windowStyles & WindowStylesFlags.WS_MINIMIZE) > 0L)
            {
                // If a window is minimized to the taskbar, it is still WS_VISIBLE;
                // if it is minimized to the tray, it is not.
                return !toTray || (windowStyles & WindowStylesFlags.WS_VISIBLE) == 0L;
            }
            return false;
        }

        public static bool SendWindowMessage(IntPtr hWnd, WindowsMessagesFlags msg, IntPtr wParam, IntPtr lParam, bool postMessage = false)
        {
            return postMessage ? PostMessage(hWnd, (uint)msg, wParam, lParam) : SendMessage(hWnd, (uint)msg, wParam, lParam);
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

        internal enum ShowWindowCmd
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE            = 0,

            /// <summary>
            /// Activates and displays a window.
            /// If the window is minimized or maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL      = 1,

            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED   = 2,

            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            SW_MAXIMIZE        = 3,

            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED   = SW_MAXIMIZE,

            /// <summary>
            /// Displays a window in its most recent size and position.
            /// This value is similar to <see cref="SW_SHOWNORMAL"/>, except that the window is not activated.
            /// </summary>
            SW_SHOWNOACTIVATE  = 4,

            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOW            = 5,

            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            SW_MINIMIZE        = 6,

            /// <summary>
            /// Displays the window as a minimized window.
            /// This value is similar to <see cref="SW_SHOWMINIMIZED"/>, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,

            /// <summary>
            /// Displays the window in its current size and position.
            /// This value is similar to <see cref="SW_SHOW"/>, except that the window is not activated.
            /// </summary>
            SW_SHOWNA          = 8,

            /// <summary>
            /// Activates and displays the window.
            /// If the window is minimized or maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE         = 9,

            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            /// </summary>
            SW_SHOWDEFAULT     = 10,

            /// <summary>
            /// Minimizes a window, even if the thread that owns the window is not responding.
            /// This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            SW_FORCEMINIMIZE   = 11
        }

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

        /// <summary>
        /// Values to use with <see cref="Win32API.GetWindowLongPtr"/>.
        /// </summary>
        internal enum GWL
        {
            GWL_EXSTYLE    = -20,
            GWL_HINSTANCE  = -6,
            GWL_HWNDPARENT = -8,
            GWL_ID         = -12,
            GWL_STYLE      = -16,
            GWL_USERDATA   = -21,
            GWL_WNDPROC    = -4
        }
        
        /// <summary>
        /// Window styles (<see cref="GWL.GWL_STYLE"/>).
        /// </summary>
        [Flags]
        internal enum WindowStylesFlags : long
        {
            WS_BORDER           = 0x00800000L,
            WS_CAPTION          = 0x00C00000L,
            WS_CHILD            = 0x40000000L,
            WS_CHILDWINDOW      = 0x40000000L,
            WS_CLIPCHILDREN     = 0x02000000L,
            WS_CLIPSIBLINGS     = 0x04000000L,
            WS_DISABLED         = 0x08000000L,
            WS_DLGFRAME         = 0x00400000L,
            WS_GROUP            = 0x00020000L,
            WS_HSCROLL          = 0x00100000L,
            WS_ICONIC           = 0x20000000L,
            WS_MAXIMIZE         = 0x01000000L,
            WS_MAXIMIZEBOX      = 0x00010000L,
            WS_MINIMIZE         = 0x20000000L,
            WS_MINIMIZEBOX      = 0x00020000L,
            WS_OVERLAPPED       = 0x00000000L,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP            = 0x80000000L,
            WS_POPUPWINDOW      = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEBOX          = 0x00040000L,
            WS_SYSMENU          = 0x00080000L,
            WS_TABSTOP          = 0x00010000L,
            WS_THICKFRAME       = 0x00040000L,
            WS_TILED            = 0x00000000L,
            WS_TILEDWINDOW      = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_VISIBLE          = 0x10000000L,
            WS_VSCROLL          = 0x00200000L
        }

        /// <summary>
        /// Extended window styles (<see cref="GWL.GWL_EXSTYLE"/>).
        /// </summary>
        [Flags]
        internal enum ExtendedWindowStylesFlags : long
        {
            WS_EX_ACCEPTFILES         = 0x00000010L,
            WS_EX_APPWINDOW           = 0x00040000L,
            WS_EX_CLIENTEDGE          = 0x00000200L,
            WS_EX_COMPOSITED          = 0x02000000L,
            WS_EX_CONTEXTHELP         = 0x00000400L,
            WS_EX_CONTROLPARENT       = 0x00010000L,
            WS_EX_DLGMODALFRAME       = 0x00000001L,
            WS_EX_LAYERED             = 0x00080000L,
            WS_EX_LAYOUTRTL           = 0x00400000L,
            WS_EX_LEFT                = 0x00000000L,
            WS_EX_LEFTSCROLLBAR       = 0x00004000L,
            WS_EX_LTRREADING          = 0x00000000L,
            WS_EX_MDICHILD            = 0x00000040L,
            WS_EX_NOACTIVATE          = 0x08000000L,
            WS_EX_NOINHERITLAYOUT     = 0x00100000L,
            WS_EX_NOPARENTNOTIFY      = 0x00000004L,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
            WS_EX_OVERLAPPEDWINDOW    = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW       = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT               = 0x00001000L,
            WS_EX_RIGHTSCROLLBAR      = 0x00000000L,
            WS_EX_RTLREADING          = 0x00002000L,
            WS_EX_STATICEDGE          = 0x00020000L,
            WS_EX_TOOLWINDOW          = 0x00000080L,
            WS_EX_TOPMOST             = 0x00000008L,
            WS_EX_TRANSPARENT         = 0x00000020L,
            WS_EX_WINDOWEDGE          = 0x00000100L
        }

        [Flags]
        internal enum ExecutionStateFlags : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS        = 0x80000000,
            ES_DISPLAY_REQUIRED  = 0x00000002,
            ES_SYSTEM_REQUIRED   = 0x00000001,
            ES_USER_PRESENT      = 0x00000004 // Legacy flag, should not be used.
        }

        [Flags]
        internal enum WindowsMessagesFlags : uint
        {
            // Window Messages & Notifications
            WM_CLOSE           = 0x0010,
            WM_QUIT            = 0x0012,

            // Keyboard Input Messages & Notifications
            WM_KEYDOWN         = 0x0100,
            WM_KEYUP           = 0x0101,
            WM_CHAR            = 0x0102,
            WM_SYSKEYDOWN      = 0x0104,
            WM_SYSKEYUP        = 0x0105,
            WM_SYSCHAR         = 0x0106,

            WM_APPCOMMAND      = 0x0319,

            // Menu Messages & Notifications
            WM_COMMAND         = 0x0111,
            WM_UNINITMENUPOPUP = 0x0125,

            // Keyboard Accelerator Messages & Notifications
            WM_INITMENU        = 0x0116,
            WM_INITMENUPOPUP   = 0x0117,
        }

        internal enum MapVirtualKeyType : uint
        {
            MAPVK_VK_TO_VSC    = 0,
            MAPVK_VSC_TO_VK    = 1,
            MAPVK_VK_TO_CHAR   = 2,
            MAPVK_VSC_TO_VK_EX = 3,
            MAPVK_VK_TO_VSC_EX = 4
        }

        #endregion Enums

        #region Internal classes and structs

        internal struct WindowPlacement
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
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

        #endregion Internal classes and structs
    }
}