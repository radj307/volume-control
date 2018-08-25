using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ToastifyAPI.Native.Delegates;
using ToastifyAPI.Native.Enums;

namespace ToastifyAPI.Native
{
    public static class Processes
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Processes));

        public static string GetProcessName(uint processId)
        {
            if (processId == 0)
                return null;

            var process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }

        public static IEnumerable<ProcessModule> GetProcessModules(uint processId)
        {
            if (processId == 0)
                return null;

            var process = Process.GetProcessById((int)processId);
            return process.Modules.Cast<ProcessModule>();
        }

        /// <summary>
        /// Returns true if the current application has focus, false otherwise.
        /// </summary>
        public static bool IsCurrentProcessFocused()
        {
            var foregroundHwnd = User32.GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero)
                return false;
            
            User32.GetWindowThreadProcessId(foregroundHwnd, out uint processId);
            return processId == Process.GetCurrentProcess().Id;
        }

        public static IntPtr SetLowLevelMouseHook(ref LowLevelMouseHookProc mouseHookProc)
        {
            IntPtr hHook;
            using (Process process = Process.GetCurrentProcess())
            {
                using (ProcessModule module = process.MainModule)
                {
                    IntPtr hModule = Kernel32.GetModuleHandle(module.ModuleName);

                    hHook = User32.SetWindowsHookEx(HookType.WH_MOUSE_LL, mouseHookProc, hModule, 0);
                    if (hHook == IntPtr.Zero)
                        logger.Error($"Failed to register a low-level mouse hook. Error code: {Marshal.GetLastWin32Error()}");
                }
            }

            return hHook;
        }
    }
}