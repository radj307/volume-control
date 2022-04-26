using System.Runtime.InteropServices;

namespace VolumeControl.Core
{
    /// <summary>
    /// Contains various functions from user32.dll
    /// </summary>
    public static class User32
    {
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
        public extern static bool ShowWindow(IntPtr hWnd, int nCmdShow);
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

    }
}
