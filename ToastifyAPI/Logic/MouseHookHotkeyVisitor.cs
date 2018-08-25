using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;
using ToastifyAPI.Common;
using ToastifyAPI.Core;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;
using ToastifyAPI.Native;
using ToastifyAPI.Native.Delegates;
using ToastifyAPI.Native.Enums;
using ToastifyAPI.Native.Structs;

namespace ToastifyAPI.Logic
{
    public class MouseHookHotkeyVisitor : IMouseHookHotkeyVisitor
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MouseHookHotkeyVisitor));

        private readonly List<IMouseHookHotkey> registeredHotkeys = new List<IMouseHookHotkey>();

        private IntPtr hMouseHook;
        private LowLevelMouseHookProc mouseHookProc;

        public MouseHookHotkeyVisitor()
        {
            this.hMouseHook = IntPtr.Zero;
            this.mouseHookProc = this.MouseHookProc;
        }

        /// <inheritdoc />
        public void Visit(IMouseHookHotkey hotkey)
        {
            hotkey?.PerformAction();
        }

        /// <inheritdoc />
        public void RegisterHook(IMouseHookHotkey hotkey)
        {
            this.EnsureMouseHookEnabledIfNeeded();

            if (hotkey == null)
                throw new ArgumentNullException(nameof(hotkey));
            if (!hotkey.MouseButton.HasValue)
                return;

            this.registeredHotkeys?.Add(hotkey);
        }

        /// <inheritdoc />
        public void UnregisterHook(IMouseHookHotkey hotkey)
        {
            this.EnsureMouseHookEnabledIfNeeded();

            if (hotkey == null)
                throw new ArgumentNullException(nameof(hotkey));
            if (!hotkey.MouseButton.HasValue)
                return;

            this.registeredHotkeys?.RemoveAll(h => h?.Action == hotkey.Action);
        }

        private IntPtr MouseHookProc(int nCode, WindowsMessagesFlags wParam, LowLevelMouseHookStruct lParam)
        {
            if (nCode >= 0 && this.registeredHotkeys != null)
            {
                foreach (IMouseHookHotkey h in this.registeredHotkeys)
                {
                    var union = new Union32(lParam.mouseData);

                    bool isXButton = h.MouseButton == MouseAction.XButton1 || h.MouseButton == MouseAction.XButton2;
                    bool isMWheel = h.MouseButton == MouseAction.MWheelUp || h.MouseButton == MouseAction.MWheelDown;

                    if (isXButton && wParam == WindowsMessagesFlags.WM_XBUTTONUP)
                    {
                        if (h.AreModifiersPressed() && (union.High == 0x0001 && h.MouseButton == MouseAction.XButton1 ||
                                                        union.High == 0x0002 && h.MouseButton == MouseAction.XButton2))
                            this.Visit(h);
                    }
                    else if (isMWheel && wParam == WindowsMessagesFlags.WM_MOUSEWHEEL)
                    {
                        short delta = unchecked((short)union.High);
                        if (h.AreModifiersPressed() && (delta > 0 && h.MouseButton == MouseAction.MWheelUp ||
                                                        delta < 0 && h.MouseButton == MouseAction.MWheelDown))
                            this.Visit(h);
                    }
                }
            }

            return User32.CallNextHookEx(this.hMouseHook, nCode, wParam, lParam);
        }

        private void EnsureMouseHookEnabledIfNeeded()
        {
            // If no hotkeys need the mouse hook, then unregister it
            if (this.registeredHotkeys.Count <= 0)
            {
                if (this.hMouseHook != IntPtr.Zero)
                {
                    bool success = User32.UnhookWindowsHookEx(this.hMouseHook);
                    if (!success)
                        logger.Error($"Failed to un-register a low-level mouse hook. Error code: {Marshal.GetLastWin32Error()}");
                    this.hMouseHook = IntPtr.Zero;
                }
            }
            else
            {
                if (this.hMouseHook == IntPtr.Zero)
                    this.hMouseHook = Processes.SetLowLevelMouseHook(ref this.mouseHookProc);
            }
        }
    }
}