using System.Runtime.InteropServices;
using VolumeControl.Hotkeys.Enum;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains various functions from user32.dll
    /// </summary>
    public static class User32
    {
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
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
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
        public static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, EUFlags eFlags) => SetWindowPos(hWnd, hWndInsertAfter, X, Y, cx, cy, (uint)eFlags);
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
        public static extern bool SetActiveWindow(IntPtr hWnd);
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <remarks>
        /// To perform certain special effects when showing or hiding a window, use AnimateWindow.
        /// The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.
        /// As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the structure.In this case, ShowWindow uses the information specified in the STARTUPINFO structure to show the window.On subsequent calls, the application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application.This behavior is designed for the following situations:
        /// <list type="bullet">
        /// <item><description>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.</description></item>
        /// <item><description>Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and later call ShowWindow with the <see cref="ECmdShow.SW_SHOW"/> flag set to make it visible.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown. This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>If the window was previously visible, the return value is nonzero.</description></item>
        /// <item><term>false</term><description>If the window was previously hidden, the return value is zero.</description></item>
        /// </list></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <remarks>
        /// The first time this function is called, the <paramref name="eCmdShow"/> parameter is ignored in order to initialize with <paramref name="hWnd"/>.<br/>
        /// In most cases, you can get this out of the way in the <see cref="Form"/>'s constructor by calling: <code>ShowWindow(<paramref name="hWnd"/>, <see cref="ECmdShow.SW_HIDE"/>)</code>
        /// </remarks>
        /// <param name="hWnd">A handle to the window.<br/>This can be acquired with: <code>Form.Handle</code></param>
        /// <param name="eCmdShow">Controls how the window is to be shown.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>If the window was previously visible, the return value is nonzero.</description></item>
        /// <item><term>false</term><description>If the window was previously hidden, the return value is zero.</description></item>
        /// </list></returns>
        public static bool ShowWindow(IntPtr hWnd, ECmdShow eCmdShow) => ShowWindow(hWnd, (int)eCmdShow);
        /// <summary>
        /// Modes that define how a window shown by calling <see cref="ShowWindow(IntPtr, int)"/> acts.<br/>
        /// This should be passed as the <b>nCmdShow</b> parameter.
        /// </summary>
        /// <remarks>You can use <see cref="ShowWindow(IntPtr, ECmdShow)"/> as an alias for converting the enum value to an integer.</remarks>
        public enum ECmdShow : int
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

        /// <summary>
        /// Possible values to pass to the <see cref="SetWindowPos(IntPtr, IntPtr, int, int, int, int, EUFlags)"/> function.
        /// </summary>
        [Flags]
        public enum EUFlags : uint
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

        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void KeyboardEvent(EVirtualKeyCode virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
    }
}
