using log4net;
using ManagedWinapi;
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
using Toastify.Core;
using Toastify.Helpers;

// ReSharper disable InconsistentNaming
namespace Toastify
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    internal static class Win32API
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Win32API));

        private static readonly IntPtr hDesktop = GetDesktopWindow();
        private static readonly IntPtr hProgman = GetShellWindow();
        private static readonly IntPtr hShellDll = GetShellDllDefViewWindow();

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        internal delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        internal delegate IntPtr LowLevelMouseHookProc(int code, WindowsMessagesFlags wParam, [In] LowLevelMouseHookStruct lParam);

        internal delegate void SendMessageDelegate(IntPtr hWnd, uint uMsg, UIntPtr dwData, IntPtr lResult);

        internal delegate bool EnumResNameProcDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

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
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

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

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="hookType"> The type of hook procedure to be installed. </param>
        /// <param name="lpfn">
        ///   A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread
        ///   created by a different process, the lpfn parameter must point to a hook procedure in a DLL.
        ///   Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        ///   A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be
        ///   set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure
        ///   is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///   The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter
        ///   is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.
        /// </param>
        /// <returns> If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL. </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="hookType"> The type of hook procedure to be installed. </param>
        /// <param name="lpfn">
        ///   A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread
        ///   created by a different process, the lpfn parameter must point to a hook procedure in a DLL.
        ///   Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        ///   A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be
        ///   set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure
        ///   is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///   The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter
        ///   is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.
        /// </param>
        /// <returns> If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL. </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(HookType hookType, LowLevelMouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk"> A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx(HookType,HookProc,IntPtr,uint)"/>. </param>
        /// <returns> <c>true</c> or nonzero if the function succeeds, <c>false</c> or zero if the function fails. </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk"> This parameter is ignored. </param>
        /// <param name="nCode">
        ///   The hook code passed to the current hook procedure.
        ///   The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam"> The wParam value passed to the current hook procedure. </param>
        /// <param name="lParam"> The lParam value passed to the current hook procedure. </param>
        /// <returns> The meaning of the return value depends on the hook type. </returns>
        /// <remarks>
        /// <para>
        ///   Hook procedures are installed in chains for particular hook types.
        ///   <see cref="CallNextHookEx(IntPtr,int,IntPtr,IntPtr)" /> calls the next hook in the chain.
        /// </para>
        /// <para>
        ///   Calling CallNextHookEx is optional, but it is highly recommended; otherwise, other applications that have
        ///   installed hooks will not receive hook notifications and may behave incorrectly as a result. You should call
        ///   <see cref="CallNextHookEx(IntPtr,int,IntPtr,IntPtr)" /> unless you absolutely need to prevent the notification
        ///   from being seen by other applications.
        /// </para>
        /// </remarks>
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // overload for use with LowLevelMouseProc
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, WindowsMessagesFlags wParam, [In] LowLevelMouseHookStruct lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool SendMessageCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, SendMessageDelegate lpCallBack, UIntPtr dwData);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(int dwErrorCode);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern ExecutionStateFlags SetThreadExecutionState(ExecutionStateFlags esFlags);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, MapVirtualKeyType uMapType);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKeyEx(uint uCode, MapVirtualKeyType uMapType, IntPtr dwhkl);

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        internal static extern void KeyboardEvent(VirtualKeyCode virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        /// <summary>
        /// Enumerates resources of a specified type within a binary module.
        /// </summary>
        /// <param name="hModule">
        ///   A handle to a module to be searched.
        ///   If this parameter is NULL, that is equivalent to passing in a handle to the module used to create the current process.
        /// </param>
        /// <param name="lpszType"> The type of the resource for which the name is being enumerated. </param>
        /// <param name="lpEnumFunc"> The callback function to be called for each enumerated resource name. </param>
        /// <param name="lParam"> An application-defined value passed to the callback function. This parameter can be used in error checking. </param>
        /// <returns> The return value is TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason. </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumResourceNames(IntPtr hModule, string lpszType, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// Enumerates resources of a specified type within a binary module.
        /// </summary>
        /// <param name="hModule">
        ///   A handle to a module to be searched.
        ///   If this parameter is NULL, that is equivalent to passing in a handle to the module used to create the current process.
        /// </param>
        /// <param name="dwID"> The ID of the resource. </param>
        /// <param name="lpEnumFunc"> The callback function to be called for each enumerated resource ID. </param>
        /// <param name="lParam"> An application-defined value passed to the callback function. This parameter can be used in error checking. </param>
        /// <returns> The return value is TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason. </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumResourceNames(IntPtr hModule, ResourceType dwID, EnumResNameProcDelegate lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #endregion DLL imports

        internal static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            return IntPtr.Size == 4 ? GetWindowLongPtr32(hWnd, nIndex) : GetWindowLongPtr64(hWnd, nIndex);
        }

        internal static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 4 ? new IntPtr(SetWindowLongPtr32(hWnd, nIndex, dwNewLong.ToInt32())) : SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

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
            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint pid);
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
            GetClassName(hWnd, sb, 256);
            return sb.ToString();
        }

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

        public static string GetWindowTitle(IntPtr hWnd)
        {
            const int nMaxCount = 256;
            StringBuilder lpString = new StringBuilder(nMaxCount);
            return GetWindowText(hWnd, lpString, nMaxCount) > 0 ? lpString.ToString() : string.Empty;
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

        public static void SendMediaKey(ToastifyAction action)
        {
            VirtualKeyCode virtualKey = default(VirtualKeyCode);
            switch (action)
            {
                case ToastifyAction.PlayPause:
                    virtualKey = VirtualKeyCode.VK_MEDIA_PLAY_PAUSE;
                    break;

                case ToastifyAction.Mute:
                    virtualKey = VirtualKeyCode.VK_VOLUME_MUTE;
                    break;

                case ToastifyAction.VolumeDown:
                    virtualKey = VirtualKeyCode.VK_VOLUME_DOWN;
                    break;

                case ToastifyAction.VolumeUp:
                    virtualKey = VirtualKeyCode.VK_VOLUME_UP;
                    break;

                case ToastifyAction.PreviousTrack:
                    virtualKey = VirtualKeyCode.VK_MEDIA_PREV_TRACK;
                    break;

                case ToastifyAction.NextTrack:
                    virtualKey = VirtualKeyCode.VK_MEDIA_NEXT_TRACK;
                    break;

                // The FastForward and Rewind actions have been dropped since Spotify version 1.0.75.483.g7ff4a0dc due to issue #31
                case ToastifyAction.FastForward:
                case ToastifyAction.Rewind:
                    break;

                case ToastifyAction.None:
                case ToastifyAction.ShowToast:
                case ToastifyAction.ShowSpotify:
                case ToastifyAction.CopyTrackInfo:
                case ToastifyAction.SettingsSaved:
                case ToastifyAction.PasteTrackInfo:
                case ToastifyAction.ThumbsUp:
                case ToastifyAction.ThumbsDown:
                case ToastifyAction.Exit:
                case ToastifyAction.ShowDebugView:
                default:
                    return;
            }

            KeyboardEvent(virtualKey, 0, 1, IntPtr.Zero);
        }

        public static IntPtr SetLowLevelMouseHook(ref LowLevelMouseHookProc mouseHookProc)
        {
            IntPtr hHook;
            using (Process process = Process.GetCurrentProcess())
            {
                using (ProcessModule module = process.MainModule)
                {
                    IntPtr hModule = GetModuleHandle(module.ModuleName);

                    hHook = SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookProc, hModule, 0);
                    if (hHook == IntPtr.Zero)
                        logger.Error($"Failed to register a low-level mouse hook. Error code: {Marshal.GetLastWin32Error()}");
                }
            }

            return hHook;
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

        internal enum HookType
        {
            WH_JOURNALRECORD   = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD        = 2,
            WH_GETMESSAGE      = 3,
            WH_CALLWNDPROC     = 4,
            WH_CBT             = 5,
            WH_SYSMSGFILTER    = 6,
            WH_MOUSE           = 7,
            WH_HARDWARE        = 8,
            WH_DEBUG           = 9,
            WH_SHELL           = 10,
            WH_FOREGROUNDIDLE  = 11,
            WH_CALLWNDPROCRET  = 12,
            WH_KEYBOARD_LL     = 13,
            WH_MOUSE_LL        = 14
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
            WM_ERASEBKGND      = 0x0014,
            WM_CHILDACTIVATE   = 0x0022,
            WM_GETICON         = 0x007F,

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
            WM_MENUSELECT      = 0x011F,

            // Mouse Input Notifications
            WM_LBUTTONDOWN     = 0x0201,
            WM_LBUTTONUP       = 0x0202,
            WM_LBUTTONDBLCLK   = 0x0203,
            WM_RBUTTONDOWN     = 0x0204,
            WM_RBUTTONUP       = 0x0205,
            WM_RBUTTONDBLCLK   = 0x0206,
            WM_MBUTTONDOWN     = 0x0207,
            WM_MBUTTONUP       = 0x0208,
            WM_MBUTTONDBLCLK   = 0x0209,
            WM_MOUSEWHEEL      = 0x020A,
            WM_XBUTTONDOWN     = 0x020B,
            WM_XBUTTONUP       = 0x020C,
            WM_XBUTTONDBLCLK   = 0x020D,
            WM_MOUSEHWHEEL     = 0x020E,
            WM_NCXBUTTONDOWN   = 0x00AB,
            WM_NCXBUTTONUP     = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
        }

        internal enum MapVirtualKeyType : uint
        {
            MAPVK_VK_TO_VSC    = 0,
            MAPVK_VSC_TO_VK    = 1,
            MAPVK_VK_TO_CHAR   = 2,
            MAPVK_VSC_TO_VK_EX = 3,
            MAPVK_VK_TO_VSC_EX = 4
        }

        internal enum VirtualKeyCode : byte
        {
            VK_VOLUME_MUTE      = 0xAD,
            VK_VOLUME_DOWN      = 0xAE,
            VK_VOLUME_UP        = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP       = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
    }

        internal enum ResourceType
        {
            RT_CURSOR       = 1,
            RT_BITMAP       = 2,
            RT_ICON         = 3,
            RT_MENU         = 4,
            RT_DIALOG       = 5,
            RT_STRING       = 6,
            RT_FONTDIR      = 7,
            RT_FONT         = 8,
            RT_ACCELERATOR  = 9,
            RT_RCDATA       = 10,
            RT_MESSAGETABLE = 11,
            RT_GROUP_CURSOR = 12,
            RT_GROUP_ICON   = 14,
            RT_VERSION      = 16,
            RT_DLGINCLUDE   = 17,
            RT_PLUGPLAY     = 19,
            RT_VXD          = 20,
            RT_ANICURSOR    = 21,
            RT_ANIICON      = 22,
            RT_HTML         = 23,
            RT_MANIFEST     = 24
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct LowLevelMouseHookStruct
        {
            /// <summary>
            /// The x- and y-coordinates of the cursor, in per-monitor-aware screen coordinates.
            /// </summary>
            public Point pt;

            /// <summary> Mouse data. </summary>
            /// <remarks>
            /// If the message is <see cref="WindowsMessagesFlags.WM_MOUSEWHEEL"/>, the high-order word of this member
            /// is the wheel delta. The low-order word is reserved.
            /// A positive value indicates that the wheel was rotated forward, away from the user;
            /// a negative value indicates that the wheel was rotated backward, toward the user.
            /// One wheel click is defined as WHEEL_DELTA, which is 120.
            /// 
            /// If the message is <see cref="WindowsMessagesFlags.WM_XBUTTONDOWN"/>, <see cref="WindowsMessagesFlags.WM_XBUTTONUP"/>,
            /// <see cref="WindowsMessagesFlags.WM_XBUTTONDBLCLK"/>, <see cref="WindowsMessagesFlags.WM_NCXBUTTONDOWN"/>, 
            /// <see cref="WindowsMessagesFlags.WM_NCXBUTTONUP"/>, or <see cref="WindowsMessagesFlags.WM_NCXBUTTONDBLCLK"/>,
            /// the high-order word specifies which X button was pressed or released, and the low-order word is reserved.
            /// This value can be one or more of the following values. Otherwise, mouseData is not used.
            /// 
            /// <list type="bullet">
            /// <item>
            ///   <term> XBUTTON1 </term>
            ///   <description> 0x0001 </description>
            /// </item>
            /// <item>
            ///   <term> XBUTTON2 </term>
            ///   <description> 0x0002 </description>
            /// </item>
            /// </list>
            /// </remarks>
            public int mouseData;

            /// <summary>
            /// The event-injected flags.
            /// <para>
            /// An application can use the following values to test the flags.
            /// Testing LLMHF_INJECTED (bit 0 set) will tell you whether the event was injected. If it was, then testing LLMHF_LOWER_IL_INJECTED (bit 1 set)
            /// will tell you whether or not the event was injected from a process running at lower integrity level.
            /// </para>
            /// </summary>
            public int flags;

            /// <summary>
            /// The time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            /// Additional information associated with the message.
            /// </summary>
            public UIntPtr dwExtraInfo;
        }

        #endregion Internal classes and structs
    }
}