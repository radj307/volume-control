using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Toastify.Core;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;

// ReSharper disable InconsistentNaming
namespace Toastify
{
    internal static class Win32API
    {
        private static readonly IntPtr hDesktop = User32.GetDesktopWindow();
        private static readonly IntPtr hProgman = User32.GetShellWindow();
        private static readonly IntPtr hShellDll = GetShellDllDefViewWindow();

        private static IntPtr GetShellDllDefViewWindow()
        {
            IntPtr _SHELLDLL_DefView = User32.FindWindowEx(hProgman, IntPtr.Zero, "SHELLDLL_DefView", null);

            if (_SHELLDLL_DefView.Equals(IntPtr.Zero))
            {
                User32.EnumWindows((hWnd, lParam) =>
                {
                    if (Windows.GetClassName(hWnd) == "WorkerW")
                    {
                        IntPtr child = User32.FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
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

        public static bool IsForegroundAppAFullscreenVideogame()
        {
            // Get the dimensions of the active window.
            IntPtr hWnd = User32.GetForegroundWindow();

            Debug.WriteLine($"[Win32API::IsForegroundAppAFullscreenVideogame] hWnd=0x{hWnd.ToInt64():X8}, className=\"{Windows.GetClassName(hWnd)}\"");

            if (!hWnd.Equals(IntPtr.Zero))
            {
                // Check we haven't picked up the desktop or the shell or the parent of one of them (Progman, WorkerW)
                List<IntPtr> childWindows = Windows.GetChildWindows(hWnd);
                if (!hWnd.Equals(hDesktop) && !hWnd.Equals(hProgman) && !hWnd.Equals(hShellDll) &&
                    !childWindows.Contains(hDesktop) && !childWindows.Contains(hProgman) && !childWindows.Contains(hShellDll))
                {
                    Rectangle screenRect = Screen.FromHandle(hWnd).Bounds;
                    User32.GetClientRect(hWnd, out ToastifyAPI.Native.Structs.Rect clientRect);

                    if (clientRect.Height == screenRect.Height && clientRect.Width == screenRect.Width)
                    {
                        var processId = Windows.GetProcessFromWindowHandle(hWnd);
                        var modules = Processes.GetProcessModules(processId);
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
                                    Debug.WriteLine($"[Win32API::IsForegroundAppAFullscreenVideogame] Fullscreen videogame: \"{Processes.GetProcessName(processId)}\", module found: \"{module.ModuleName}\"");
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

        public static void SendMediaKey(ToastifyActionEnum action)
        {
            VirtualKeyCode virtualKey;
            switch (action)
            {
                case ToastifyActionEnum.Stop:
                    virtualKey = VirtualKeyCode.VK_MEDIA_STOP;
                    break;

                case ToastifyActionEnum.PlayPause:
                    virtualKey = VirtualKeyCode.VK_MEDIA_PLAY_PAUSE;
                    break;

                case ToastifyActionEnum.Mute:
                    virtualKey = VirtualKeyCode.VK_VOLUME_MUTE;
                    break;

                case ToastifyActionEnum.VolumeDown:
                    virtualKey = VirtualKeyCode.VK_VOLUME_DOWN;
                    break;

                case ToastifyActionEnum.VolumeUp:
                    virtualKey = VirtualKeyCode.VK_VOLUME_UP;
                    break;

                case ToastifyActionEnum.PreviousTrack:
                    virtualKey = VirtualKeyCode.VK_MEDIA_PREV_TRACK;
                    break;

                case ToastifyActionEnum.NextTrack:
                    virtualKey = VirtualKeyCode.VK_MEDIA_NEXT_TRACK;
                    break;

                case ToastifyActionEnum.None:
                case ToastifyActionEnum.FastForward:
                case ToastifyActionEnum.Rewind:
                case ToastifyActionEnum.ShowToast:
                case ToastifyActionEnum.ShowSpotify:
                case ToastifyActionEnum.CopyTrackInfo:
                case ToastifyActionEnum.SettingsSaved:
                case ToastifyActionEnum.PasteTrackInfo:
                case ToastifyActionEnum.ThumbsUp:
                case ToastifyActionEnum.ThumbsDown:
                case ToastifyActionEnum.Exit:
#if DEBUG
                case ToastifyActionEnum.ShowDebugView:
#endif
                default:
                    return;
            }

            User32.KeyboardEvent(virtualKey, 0, 1, IntPtr.Zero);
        }
    }
}