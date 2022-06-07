using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using VolumeControl.API;
using VolumeControl.Audio;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.Hotkeys.Enum;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that use windows API functions.
    /// </summary>
    [ActionAddon(nameof(WindowsAPIActions))]
    public class WindowsAPIActions
    {
        #region Actions
        [HotkeyAction(ActionDescription = "Switches to the next media track for supported applications.")]
        public void NextTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_NEXT_TRACK);
        [HotkeyAction(ActionDescription = "Switches to the previous media track for supported applications.")]
        public void PreviousTrack(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PREV_TRACK);
        [HotkeyAction(ActionDescription = "Toggles media playback for supported applications.")]
        public void TogglePlayback(object? sender, HandledEventArgs e) => SendKeyboardEvent(EVirtualKeyCode.VK_MEDIA_PLAY_PAUSE);
        [HotkeyAction(ActionDescription = "Moves the VolumeControl window in front of all other windows.")]
        public void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(ActionDescription = "Moves the VolumeControl window behind all other windows.")]
        public void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(ActionDescription = "Hides the VolumeControl window.")]
        public void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_MINIMIZE);
        [HotkeyAction(ActionDescription = "Restores the VolumeControl window after minimizing it.")]
        public void Unminimize(object? sender, HandledEventArgs e)
        {
            if (HwndSource.FromHwnd(MainHWnd).RootVisual is Window w)
            {
                User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE);
                w.Visibility = Visibility.Visible;
            }
        }
        #endregion Actions

        #region Statics
        /// <summary>
        /// The handle of the main WPF window.
        /// </summary>
        private static IntPtr MainHWnd => VCAPI.Default.MainWindowHWnd;
        /// <summary>
        /// Helper function for sending virtual key presses.
        /// </summary>
        /// <inheritdoc cref="User32.KeyboardEvent(EVirtualKeyCode, byte, uint, IntPtr)"/>
        private static void SendKeyboardEvent(EVirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
        #endregion Statics
    }
}
