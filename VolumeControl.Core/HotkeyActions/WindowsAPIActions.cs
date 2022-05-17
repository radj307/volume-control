using System.ComponentModel;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.HelperTypes.Enum;
using VolumeControl.Core.HotkeyActions.Attributes;

namespace VolumeControl.Core.HotkeyActions
{
    /// <summary>
    /// Contains hotkey action handlers that use windows API functions.
    /// </summary>
    public class WindowsAPIActions
    {
        /// <summary>
        /// <see cref="ActionHandlers"/> constructor.
        /// </summary>
        /// <param name="mainWindowHandle">The handle of the main WPF window.</param>
        /// <param name="audioAPI">The <see cref="AudioAPI"/> instance to use for volume-related hotkeys.</param>
        public WindowsAPIActions(IntPtr mainWindowHandle)
        {
            MainHWnd = mainWindowHandle;
        }

        /// <summary>
        /// The handle of the main WPF window.
        /// </summary>
        private IntPtr MainHWnd { get; }

        #region Actions
        [HotkeyAction] public void NextTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        [HotkeyAction] public void PreviousTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        [HotkeyAction] public void TogglePlayback(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        [HotkeyAction] public void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction] public void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction] public void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_MINIMIZE);
        [HotkeyAction] public void UnMinimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE);
        #endregion Actions

        #region Statics
        /// <summary>
        /// Helper function for sending virtual key presses.
        /// </summary>
        /// <inheritdoc cref="User32.KeyboardEvent(EVirtualKeyCode, byte, uint, IntPtr)"/>
        private static void SendKeyboardEvent(EVirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
        #endregion Statics
    }
}
