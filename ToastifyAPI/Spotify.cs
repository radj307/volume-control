using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToastifyAPI
{
    public static partial class Spotify
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Spotify));

        /// <summary>
        /// List of names the Spotify main window had across different versions of the software.
        /// </summary>
        private static readonly List<string> spotifyMainWindowNames = new List<string>
        {
            "SpotifyMainWindow",
            "Chrome_WidgetWin_0" // Since v1.0.75.483.g7ff4a0dc
        };

        public static string GetSpotifyPath()
        {
            string spotifyPath = GetSpotifyPath_common() ?? GetSpotifyPath_platform();

            if (string.IsNullOrEmpty(spotifyPath))
                throw new ArgumentException("Could not find spotify path.");

            return spotifyPath;
        }

        private static string GetSpotifyPath_common()
        {
            string spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Spotify", string.Empty, string.Empty) as string;

            // Try in the secondary location.
            if (string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Uninstall\Spotify", "InstallLocation", string.Empty) as string;

            if (!string.IsNullOrEmpty(spotifyPath))
                spotifyPath = Path.Combine(spotifyPath, "Spotify.exe");

            return spotifyPath;
        }

        public static Process FindSpotifyProcess()
        {
            if (logger.IsDebugEnabled)
                logger.Debug("Looking for Spotify process...");

            var spotifyProcesses = Process.GetProcessesByName("spotify").ToList();
            var windowedProcesses = spotifyProcesses.Where(p => p.MainWindowHandle != IntPtr.Zero).ToList();

            if (windowedProcesses.Count > 1)
            {
                var classNames = windowedProcesses.Select(p => $"\"{Native.Windows.GetClassName(p.MainWindowHandle)}\"");
                logger.Warn($"More than one ({windowedProcesses.Count}) \"spotify\" process has a non-null main window: {string.Join(", ", classNames)}");
            }

            var process = windowedProcesses.FirstOrDefault();

            // If none of the Spotify processes found has a valid MainWindowHandle,
            // then Spotify has probably been minimized to the tray: we need to check every window.
            if (process == null)
            {
                foreach (var p in spotifyProcesses)
                {
                    if (IsMainSpotifyProcess((uint)p.Id))
                        return p;
                }
            }

            return process;
        }

        public static IntPtr GetMainWindowHandle(uint pid)
        {
            if (pid == 0)
                return IntPtr.Zero;

            var windows = Native.Windows.GetProcessWindows(pid);
            var possibleMainWindows = windows.Where(h =>
            {
                string className = Native.Windows.GetClassName(h);
                string windowName = Native.Windows.GetWindowTitle(h);
                return !string.IsNullOrWhiteSpace(windowName) && spotifyMainWindowNames.Contains(className);
            }).ToList();

            if (possibleMainWindows.Count > 1)
            {
                var classNames = possibleMainWindows.Select(h => $"\"{Native.Windows.GetClassName(h)}\"");
                logger.Warn($"More than one ({possibleMainWindows.Count}) possible main windows located for Spotify: {string.Join(", ", classNames)}");
            }

            return possibleMainWindows.FirstOrDefault();
        }

        public static bool IsMainSpotifyProcess(uint pid)
        {
            var windows = Native.Windows.GetProcessWindows(pid);
            IntPtr hWnd = windows.FirstOrDefault(h => spotifyMainWindowNames.Contains(Native.Windows.GetClassName(h)));
            return hWnd != IntPtr.Zero;
        }
    }
}