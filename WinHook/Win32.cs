using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using WinHook.Enum;
using WinHook.Delegates;

namespace WinHook
{

    /// <summary>
    /// An application-defined or library-defined callback function used with the <see cref="User32.SetWindowsHookExA(int, IntPtr, IntPtr, int)"/> function.
    /// </summary>
    public delegate IntPtr WinCallback(int code, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function whenever the GetMessage or PeekMessage function has retrieved a message from an application message queue. Before returning the retrieved message to the caller, the system passes the message to the hook procedure.<br/>
    /// The HOOKPROC type defines a pointer to this callback function. GetMsgProc is a placeholder for the application-defined or library-defined function name.
    /// </summary>
    /// <param name="code">Specifies whether the hook procedure must process the message. If code is HC_ACTION, the hook procedure must process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
    /// <param name="wParam">Specifies whether the message has been removed from the queue. This parameter can be one of the following values.</param>
    /// <param name="lParam">A pointer to an MSG structure that contains details about the message.</param>
    /// <returns>If code is less than zero, the hook procedure must return the value returned by CallNextHookEx.<br/>If code is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_GETMESSAGE hooks will not receive hook notifications and may behave incorrectly as a result. If the hook procedure does not call CallNextHookEx, the return value should be zero.</returns>
    public delegate IntPtr GetMsgProc(int code, IntPtr wParam, IntPtr lParam);

    public enum EGetMsgProc_WParam : int
    {
        /// <summary>
        /// The message has not been removed from the queue. (An application called the PeekMessage function, specifying the PM_NOREMOVE flag.)
        /// </summary>
        PM_NOREMOVE = 0x0000,
        /// <summary>
        /// The message has been removed from the queue. (An application called GetMessage, or it called the PeekMessage function, specifying the PM_REMOVE flag.)
        /// </summary>
        PM_REMOVE = 0x0001,
    }



    /// <summary>
    /// Contains functions from the User32 dll.
    /// </summary>
    public static class Win32
    {
        #region SetWindowPos
        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values. <see cref="HWND_BOTTOM"/>, <see cref="HWND_NOTOPMOST"/>, <see cref="HWND_TOP"/>, or <see cref="HWND_TOPMOST"/>.</param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br/>If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        public extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values. <see cref="HWND_BOTTOM"/>, <see cref="HWND_NOTOPMOST"/>, <see cref="HWND_TOP"/>, or <see cref="HWND_TOPMOST"/>.</param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="eFlags">The window sizing and positioning flags of type <see cref="EUFlags"/>.</param>
        /// <returns>If the function succeeds, the return value is nonzero.<br/>If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, ESetWindowPos_Flag eFlags) => SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, (uint)eFlags);

        /// <summary>
        /// Possible values to pass to the <see cref="SetWindowPos(IntPtr, IntPtr, int, int, int, int, ESetWindowPos_Flag)"/> function.
        /// </summary>
        [Flags]
        public enum ESetWindowPos_Flag : uint
        {
            /// <summary>
            ///  If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. 
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,
            /// <summary>
            ///  Prevents generation of the WM_SYNCPAINT message. 
            /// </summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>
            ///  Draws a frame (defined in the window's class description) around the window. 
            /// </summary>
            SWP_DRAWFRAME = 0x0020,
            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function.Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>
            /// Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>
            /// Does not activate the window.If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>
            /// Discards the entire contents of the client area.If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>
            /// Does not redraw changes.If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved.When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,
            /// <summary>
            /// Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>
            /// Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>
            /// Displays the window. 
            /// </summary>
            SWP_SHOWWINDOW = 0x0040,
        }
        #endregion SetWindowPos
        #region SetActiveWindow
        /// <summary>
        /// Activates a window. The window must be attached to the calling thread's message queue.
        /// </summary>
        /// <param name="hWnd">A handle to the top-level window to be activated.</param>
        /// <remarks>
        /// The SetActiveWindow function activates a window, but not if the application is in the background. The window will be brought into the foreground (top of Z-Order) if its application is in the foreground when the system activates the window.<br/>
        /// If the window identified by the hWnd parameter was created by the calling thread, the active window status of the calling thread is set to hWnd.Otherwise, the active window status of the calling thread is set to NULL.
        /// </remarks>
        /// <returns>If the function succeeds, the return value is the handle to the window that was previously active.<br/>If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        public extern static bool SetActiveWindow(IntPtr hWnd);
        #endregion SetActiveWindow
        #region ShowWindow
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <remarks>
        /// To perform certain special effects when showing or hiding a window, use AnimateWindow.
        /// The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.
        /// As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the structure.In this case, ShowWindow uses the information specified in the STARTUPINFO structure to show the window.On subsequent calls, the application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application.This behavior is designed for the following situations:
        /// <list type="bullet">
        /// <item><description>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.</description></item>
        /// <item><description>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and later call ShowWindow with the <see cref="EShowWindow_CmdShow.SW_SHOW"/> flag set to make it visible.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>If the window was previously visible, the return value is nonzero.</description></item>
        /// <item><term>false</term><description>If the window was previously hidden, the return value is zero.</description></item>
        /// </list></returns>
        [DllImport("user32.dll")]
        public extern static bool ShowWindow(IntPtr hWnd, int nCmdShow);
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <remarks>
        /// The first time this function is called, the <paramref name="eCmdShow"/> parameter is ignored in order to initialize with <paramref name="hWnd"/>.<br/>
        /// In most cases, you can get this out of the way in the <see cref="Form"/>'s constructor by calling: <code>ShowWindow(<paramref name="hWnd"/>, <see cref="EShowWindow_CmdShow.SW_HIDE"/>)</code>
        /// </remarks>
        /// <param name="hWnd">A handle to the window.<br/>This can be acquired with: <code>Form.Handle</code></param>
        /// <param name="eCmdShow">Controls how the window is to be shown.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>If the window was previously visible, the return value is nonzero.</description></item>
        /// <item><term>false</term><description>If the window was previously hidden, the return value is zero.</description></item>
        /// </list></returns>
        public static bool ShowWindow(IntPtr hWnd, EShowWindow_CmdShow eCmdShow) => ShowWindow(hWnd, (int)eCmdShow);
        /// <summary>
        /// Modes that define how a window shown by calling <see cref="ShowWindow(IntPtr, int)"/> acts.<br/>
        /// This should be passed as the <b>nCmdShow</b> parameter.
        /// </summary>
        /// <remarks>You can use <see cref="ShowWindow(IntPtr, ECmdShow)"/> as an alias for converting the enum value to an integer.</remarks>
        public enum EShowWindow_CmdShow : int
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            SW_HIDE = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SW_SHOWNORMAL = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SW_SHOWMINIMIZED = 2,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value is similar to <see cref="SW_SHOWNORMAL"/>, except that the window is not activated.
            /// </summary>
            /// <remarks>This is especially useful </remarks>
            SW_SHOWNOACTIVATE = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOW = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            SW_MINIMIZE = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to <see cref="SW_SHOWMINIMIZED"/>, except the window is not activated.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is similar to <see cref="SW_SHOW"/>, except that the window is not activated.
            /// </summary>
            SW_SHOWNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            /// </summary>
            SW_RESTORE = 9,
            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            /// </summary>
            SW_SHOWDEFAULT = 10,
            /// <summary>
            /// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            SW_FORCEMINIMIZE = 11,
        }
        /// <summary>
        ///  Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows. 
        /// </summary>
        public static readonly IntPtr HWND_BOTTOM = new(1);
        /// <summary>
        ///  Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window. 
        /// </summary>
        public static readonly IntPtr HWND_NOTOPMOST = new(-2);
        /// <summary>
        ///  Places the window at the top of the Z order. 
        /// </summary>
        public static readonly IntPtr HWND_TOP = new(0);
        /// <summary>
        ///  Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated. 
        /// </summary>
        public static readonly IntPtr HWND_TOPMOST = new(-1);
        #endregion ShowWindow
        #region SetWindowsHookExA
        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events. These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process.</param>
        /// <param name="hmod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread. For Windows Store apps, see the Remarks section.</param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.<br/>
        /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookExA(int idHook, IntPtr lpfn, IntPtr hmod, int dwThreadId);
        public static IntPtr SetWindowsHookExA(ESetWindowsHookExA_IdHook idHook, IntPtr lpfn, IntPtr hmod, int dwThreadId) => SetWindowsHookExA(idHook, lpfn, hmod, dwThreadId);
        public enum ESetWindowsHookExA_IdHook : int
        {

            /// <summary>
            /// Installs a hook procedure that monitors messages before the system sends them to the destination window procedure. For more information, see the <see cref="HookProc"/> hook procedure. 
            /// </summary>
            WH_CALLWNDPROC = 4,

            /// <summary>
            /// Installs a hook procedure that monitors messages after they have been processed by the destination window procedure. For more information, see the CallWndRetProc hook procedure.
            /// </summary>
            WH_CALLWNDPROCRET = 12,

            /// <summary>
            /// Installs a hook procedure that receives notifications useful to a CBT application. For more information, see the CBTProc hook procedure.
            /// </summary>
            WH_CBT = 5,

            /// <summary>
            /// Installs a hook procedure useful for debugging other hook procedures. For more information, see the DebugProc hook procedure.
            /// </summary>
            WH_DEBUG = 9,

            /// <summary>
            /// Installs a hook procedure that will be called when the application's foreground thread is about to become idle. This hook is useful for performing low priority tasks during idle time. For more information, see the ForegroundIdleProc hook procedure.
            /// </summary>
            WH_FOREGROUNDIDLE = 11,

            /// <summary>
            /// Installs a hook procedure that monitors messages posted to a message queue. For more information, see the GetMsgProc hook procedure.
            /// </summary>
            WH_GETMESSAGE = 3,

            /// <summary>
            /// Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure. For more information, see the JournalPlaybackProc hook procedure.
            /// </summary>
            WH_JOURNALPLAYBACK = 1,

            /// <summary>
            /// Installs a hook procedure that records input messages posted to the system message queue. This hook is useful for recording macros. For more information, see the JournalRecordProc hook procedure.
            /// </summary>
            WH_JOURNALRECORD = 0,

            /// <summary>
            /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure.
            /// </summary>
            WH_KEYBOARD = 2,

            /// <summary>
            /// Installs a hook procedure that monitors low-level keyboard input events. For more information, see the LowLevelKeyboardProc hook procedure.
            /// </summary>
            WH_KEYBOARD_LL = 13,

            /// <summary>
            /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure.
            /// </summary>
            WH_MOUSE = 7,

            /// <summary>
            /// Installs a hook procedure that monitors low-level mouse input events. For more information, see the LowLevelMouseProc hook procedure.
            /// </summary>
            WH_MOUSE_LL = 14,

            /// <summary>
            /// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. For more information, see the MessageProc hook procedure.
            /// </summary>
            WH_MSGFILTER = -1,

            /// <summary>
            /// Installs a hook procedure that receives notifications useful to shell applications. For more information, see the ShellProc hook procedure.
            /// </summary>
            WH_SHELL = 10,

            /// <summary>
            /// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar. The hook procedure monitors these messages for all applications in the same desktop as the calling thread. For more information, see the SysMsgProc hook procedure.
            /// </summary>
            WH_SYSMSGFILTER = 6,
        }
        #endregion SetWindowsHookExA
        #region RegisterHotkey
        /// <summary>
        /// This is a wrapper function for the 'RegisterHotKey' function from user32.dll
        /// </summary>
        /// <param name="hWnd">The owner window's handle.</param>
        /// <param name="id">The hotkeys ID.</param>
        /// <param name="fsModifiers">A bitfield defining which (if any) modifiers are required by the hotkey.</param>
        /// <param name="vk">The primary keyboard key.</param>
        /// <returns>True when registration was successful.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        #endregion RegisterHotkey
        #region UnregisterHotkey
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);
        /// <summary>
        /// This is a wrapper function for the 'UnregisterHotKey' function from user32.dll.
        /// </summary>
        /// <param name="hWnd">The owner window's handle.</param>
        /// <param name="id">The hotkey's ID.</param>
        /// <returns>True when unregistration was successful.</returns>
        public static bool UnregisterHotkey(IntPtr hWnd, int id)
            => UnregisterHotKey(hWnd, id) != 0;
        #endregion UnregisterHotkey
        #region FormatMessage
        [Flags]
        public enum EFORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, [Out] StringBuilder lpBuffer, uint nSize, IntPtr Arguments);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr pArguments);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, string[] Arguments);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int FormatMessage(EFORMAT_MESSAGE dwFlags, IntPtr lpSource, int dwMessageId, uint dwLanguageId, out StringBuilder msgOut, int nSize, IntPtr Arguments);
        #endregion FormatMessage
        #region GetLastError
        /// <summary>
        /// Retrieve the last win32 error code integer.
        /// </summary>
        /// <returns>The last Win32 error's identifier.</returns>
        public static int GetLastError()
            => Marshal.GetLastWin32Error();
        /// <summary>
        /// Format the given win32 error into a more descriptive string.
        /// </summary>
        /// <param name="err">Win32 error code/identifier.</param>
        /// <returns>String with the name associated with the error code specified by <paramref name="err"/>, or null if <paramref name="err"/> is 0 (no error).</returns>
        public static string? FormatErrorCode(int err)
        {
            if (err == 0) //< no error
                return null;
            _ = FormatMessage(
                EFORMAT_MESSAGE.ALLOCATE_BUFFER | EFORMAT_MESSAGE.FROM_SYSTEM | EFORMAT_MESSAGE.IGNORE_INSERTS,
                IntPtr.Zero,
                err,
                0,
                out StringBuilder msg,
                256,
                IntPtr.Zero
            );
            return msg.ToString().Trim();
        }
        /// <summary>
        /// Retrieve the last win32 error string.
        /// </summary>
        /// <returns>The last win32 error's string identifier.</returns>
        public static string? GetLastErrorString() => FormatErrorCode(GetLastError());
        /// <summary>
        /// Retrieve both the integer and string representations of the last error code.
        /// </summary>
        /// <returns>(<see cref="int"/>, <see cref="string"/>)</returns>
        public static (int, string?) GetLastWin32Error()
        {
            int err = GetLastError();
            return (err, FormatErrorCode(err));
        }
        #endregion GetLastError
        #region PeekMessageA
        /// <summary>
        /// Dispatches incoming nonqueued messages, checks the thread message queue for a posted message, and retrieves the message (if any exist).
        /// </summary>
        /// <remarks>
        /// PeekMessage retrieves messages associated with the window identified by the hWnd parameter or any of its children as specified by the IsChild function, and within the range of message values given by the wMsgFilterMin and wMsgFilterMax parameters. Note that an application can only use the low word in the wMsgFilterMin and wMsgFilterMax parameters; the high word is reserved for the system.<br/>
        /// Note that PeekMessage always retrieves WM_QUIT messages, no matter which values you specify for wMsgFilterMin and wMsgFilterMax.<br/>
        /// During this call, the system dispatches (DispatchMessage) pending, nonqueued messages, that is, messages sent to windows owned by the calling thread using the SendMessage, SendMessageCallback, SendMessageTimeout, or SendNotifyMessage function. Then the first queued message that matches the specified filter is retrieved. The system may also process internal events. If no filter is specified, messages are processed in the following order:<br/>
        /// <list type="bullet">
        /// <item><description>Sent messages</description></item>
        /// <item><description>Posted messages</description></item>
        /// <item><description>Input (hardware) messages and system internal events</description></item>
        /// <item><description>Sent messages (again)</description></item>
        /// <item><description>WM_PAINT messages</description></item>
        /// <item><description>WM_TIMER messages</description></item>
        /// </list>
        ///  To retrieve input messages before posted messages, use the wMsgFilterMin and wMsgFilterMax parameters.<br/>
        ///  The PeekMessage function normally does not remove WM_PAINT messages from the queue. WM_PAINT messages remain in the queue until they are processed. However, if a WM_PAINT message has a NULL update region, PeekMessage does remove it from the queue.<br/>
        ///  If a top-level window stops responding to messages for more than several seconds, the system considers the window to be not responding and replaces it with a ghost window that has the same z-order, location, size, and visual attributes. This allows the user to move it, resize it, or even close the application. However, these are the only actions available because the application is actually not responding. When an application is being debugged, the system does not generate a ghost window.
        /// </remarks>
        /// <param name="lpMsg">A pointer to an MSG structure that receives message information.</param>
        /// <param name="hWnd">
        /// A handle to the window whose messages are to be retrieved. The window must belong to the current thread.<br/>
        /// If hWnd is NULL, PeekMessage retrieves messages for any window that belongs to the current thread, and any messages on the current thread's message queue whose hwnd value is NULL (see the MSG structure). Therefore if hWnd is NULL, both window messages and thread messages are processed.<br/>
        /// If hWnd is -1, PeekMessage retrieves only messages on the current thread's message queue whose hwnd value is NULL, that is, thread messages as posted by PostMessage (when the hWnd parameter is NULL) or PostThreadMessage.
        /// </param>
        /// <param name="wMsgFilterMin">
        /// The value of the first message in the range of messages to be examined. Use WM_KEYFIRST (0x0100) to specify the first keyboard message or WM_MOUSEFIRST (0x0200) to specify the first mouse message.<br/>
        /// If wMsgFilterMin and wMsgFilterMax are both zero, PeekMessage returns all available messages (that is, no range filtering is performed).
        /// </param>
        /// <param name="wMsgFilterMax">
        /// The value of the last message in the range of messages to be examined. Use WM_KEYLAST to specify the last keyboard message or WM_MOUSELAST to specify the last mouse message.<br/>
        /// If wMsgFilterMin and wMsgFilterMax are both zero, PeekMessage returns all available messages (that is, no range filtering is performed).
        /// </param>
        /// <param name="wRemoveMsg">Specifies how messages are to be handled.</param>
        /// <returns>If a message is available, the return value is nonzero.<br/>If no messages are available, the return value is zero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PeekMessageA(out IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        public const int WM_KEYLAST = 265;
        public const int WM_MOUSELAST = 526;
        public enum EPeekMessageA_RemoveMsg : uint
        {
            /// <summary>
            /// Messages are not removed from the queue after processing by PeekMessage.
            /// </summary>
            PM_NOREMOVE = 0x0000,

            /// <summary>
            /// Messages are removed from the queue after processing by PeekMessage.
            /// </summary>
            PM_REMOVE = 0x0001,

            /// <summary>
            /// Prevents the system from releasing any thread that is waiting for the caller to go idle (see WaitForInputIdle).
            /// </summary>
            /// <remarks>Combine this value with either PM_NOREMOVE or PM_REMOVE.</remarks>
            PM_NOYIELD = 0x0002,

            /// <summary>
            /// Process mouse and keyboard messages.
            /// </summary>
            PM_QS_INPUT = (EQS.QS_INPUT << 16),

            /// <summary>
            /// Process paint messages.
            /// </summary>
            PM_QS_PAINT = (EQS.QS_PAINT << 16),

            /// <summary>
            /// Process all posted messages, including timers and hotkeys.
            /// </summary>
            PM_QS_POSTMESSAGE = ((EQS.QS_POSTMESSAGE | EQS.QS_HOTKEY | EQS.QS_TIMER) << 16),

            /// <summary>
            /// Process all sent messages.
            /// </summary>
            PM_QS_SENDMESSAGE = (EQS.QS_SENDMESSAGE << 16),
        }
        [Flags]
        public enum EQS : int
        {

            QS_INPUT = 1031,
            QS_PAINT = 32,
            QS_POSTMESSAGE = 8,
            QS_HOTKEY = 128,
            QS_TIMER = 16,
            QS_SENDMESSAGE = 64,
        }
        #endregion PeekMessageA

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, EShowWindowCmd nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, EMenuFlags uEnable);

        // Overload for system menu items
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, ESysCommands uIDEnableItem, EMenuFlags uEnable);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk"> A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx(HookType,HookProc,IntPtr,uint)"/>. </param>
        /// <returns> <c>true</c> or nonzero if the function succeeds, <c>false</c> or zero if the function fails. </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

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

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void KeyboardEvent(EVirtualKeyCode virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);
    }
}