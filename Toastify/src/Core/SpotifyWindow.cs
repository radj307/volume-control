using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using log4net;
using ToastifyAPI.Common;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;
using NativeWindows = ToastifyAPI.Native.Windows;

namespace Toastify.Core
{
    public class SpotifyWindow
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SpotifyWindow));

        #region Static Fields and Properties

        public static string PausedTitle { get; } = "Spotify";
        private static int GetWindowHandleTimeout { get; } = 2000;

        #endregion

        private readonly Process process;

        #region Public Properties

        public IntPtr Handle { get; private set; } = IntPtr.Zero;

        public string Title
        {
            get { return !this.IsValid ? null : NativeWindows.GetWindowTitle(this.Handle); }
        }

        public bool IsValid
        {
            get
            {
                if (this.Handle == IntPtr.Zero)
                    return false;
                return !this.process.HasExited;
            }
        }

        public bool IsMinimized
        {
            get
            {
                if (!this.IsValid)
                    return false;

                var windowStyles = (WindowStylesFlags)NativeWindows.GetWindowLongPtr(this.Handle, GWL.GWL_STYLE);
                return (windowStyles & WindowStylesFlags.WS_MINIMIZE) != 0L || this.IsMinimizedToTray;
            }
        }

        public bool IsMinimizedToTray
        {
            get
            {
                if (!this.IsValid)
                    return false;

                var windowStyles = (WindowStylesFlags)NativeWindows.GetWindowLongPtr(this.Handle, GWL.GWL_STYLE);
                return (windowStyles & WindowStylesFlags.WS_MINIMIZE) == 0L && (windowStyles & WindowStylesFlags.WS_VISIBLE) == 0L;
            }
        }

        public WindowTitleWatcher TitleWatcher { get; private set; }

        #endregion

        #region Events

        public event EventHandler InitializationFinished;

        #endregion

        public SpotifyWindow() : this(ToastifyAPI.Spotify.FindSpotifyProcess())
        {
        }

        public SpotifyWindow(Process process)
        {
            this.process = process;
            Task.Run(this.Init);
        }

        public Task Minimize()
        {
            return this.Minimize(0);
        }

        public Task Minimize(int delay)
        {
            return Task.Run(async () =>
            {
                if (this.IsValid)
                {
                    await Task.Delay(delay).ConfigureAwait(false);
                    User32.ShowWindow(this.Handle, ShowWindowCmd.SW_SHOWMINIMIZED);
                }
            });
        }

        public void Show()
        {
            if (!this.IsValid)
                return;

            // check Spotify's current window state
            var placement = new WindowPlacement();
            User32.GetWindowPlacement(this.Handle, ref placement);

            var showCommand = ShowWindowCmd.SW_SHOW;
            if (placement.showCmd == ShowWindowCmd.SW_SHOWMINIMIZED || placement.showCmd == ShowWindowCmd.SW_HIDE)
                showCommand = ShowWindowCmd.SW_RESTORE;

            if (this.IsMinimizedToTray)
            {
                // TODO: Restore Spotify if minimized to the tray.

                return;

                //IntPtr renderWindowHandle = NativeWindows.GetProcessWindows((uint)this.process.Id, "Chrome_WidgetWin_0")
                //                                         .Select(NativeWindows.GetChildWindows)
                //                                         .SingleOrDefault(children => children != null && children.Any(h => NativeWindows.GetClassName(h) == "Chrome_RenderWidgetHostHWND"))
                //                                         ?.SingleOrDefault() ?? IntPtr.Zero;

                //User32.ShowWindow(this.hWnd, showCommand);
                //if (renderWindowHandle != IntPtr.Zero)
                //{
                //    IntPtr parent = User32.GetParent(renderWindowHandle);
                //    if (parent != this.hWnd)
                //    {
                //        User32.SetParent(renderWindowHandle, this.hWnd);
                //        NativeWindows.SendWindowMessage(renderWindowHandle, WindowsMessagesFlags.WM_CHILDACTIVATE, IntPtr.Zero, IntPtr.Zero);
                //        User32.ShowWindow(renderWindowHandle, ShowWindowCmd.SW_SHOW);
                //        User32.ShowWindow(renderWindowHandle, ShowWindowCmd.SW_RESTORE);

                //        IntPtr hDC = User32.GetDC(renderWindowHandle);
                //        NativeWindows.SendWindowMessage(renderWindowHandle, WindowsMessagesFlags.WM_ERASEBKGND, hDC, IntPtr.Zero);
                //        User32.ReleaseDC(renderWindowHandle, hDC);

                //        User32.UpdateWindow(renderWindowHandle);
                //    }
                //    else
                //        NativeWindows.AddVisibleWindowStyle(renderWindowHandle);
                //}
            }

            User32.ShowWindow(this.Handle, showCommand);

            User32.SetForegroundWindow(this.Handle);
            User32.SetFocus(this.Handle);
        }

        private async Task Init()
        {
            if (logger.IsDebugEnabled)
                logger.Debug($"Initializing {nameof(SpotifyWindow)} using process with ID {this.process.Id}...");

            await this.GetWindowHandle().ConfigureAwait(false);
            if (this.Handle != IntPtr.Zero)
                this.TitleWatcher = new WindowTitleWatcher(this.Handle);
            else
                logger.Warn($"Null handle returned: can't initialize {nameof(WindowTitleWatcher)}!");

            this.OnInitializationFinished();
        }

        private async Task GetWindowHandle()
        {
            if (this.process != null)
            {
                int timeout = GetWindowHandleTimeout;
                do
                {
                    this.Handle = ToastifyAPI.Spotify.GetMainWindowHandle(unchecked((uint)this.process.Id));

                    if (this.Handle == IntPtr.Zero)
                    {
                        timeout -= 100;
                        await Task.Delay(100).ConfigureAwait(false);
                    }
                } while (this.Handle == IntPtr.Zero && timeout > 0);
            }
        }

        private void OnInitializationFinished()
        {
            if (logger.IsDebugEnabled)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{nameof(SpotifyWindow)} initialization finished. Status = {{")
                  .Append($" IsValid: {this.IsValid},")
                  .Append($" Handle: {this.Handle},")
                  .Append($" Title: \"{this.Title}\"")
                  .Append(" }");
                logger.Debug(sb.ToString());
            }

            this.InitializationFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}