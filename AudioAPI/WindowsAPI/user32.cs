using AudioAPI.WindowsAPI.Delegates;
using AudioAPI.WindowsAPI.Enum;
using AudioAPI.WindowsAPI.Struct;
using System.Runtime.InteropServices;
using System.Text;

namespace AudioAPI.WindowsAPI
{
    public static class User32
    {
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
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

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
    }
}
