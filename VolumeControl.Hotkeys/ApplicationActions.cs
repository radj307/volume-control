using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys
{
    [HotkeyActionGroup("Application", GroupColor = "#F2B2FF")]
    public sealed class ApplicationActions
    {
        #region Constructor
        public ApplicationActions() => this.MainHWnd = VCAPI.Default.MainWindowHWnd;
        #endregion Constructor

        #region Properties
        private static Config Settings => VCAPI.Default.Settings;
        private IntPtr MainHWnd { get; }
        #endregion Properties

        #region Methods
        /// <summary>Enables notifications.</summary>
        [HotkeyAction(Description = "Enables notifications.")]
        public void EnableNotification(object? sender, HandledEventArgs e) => Settings.NotificationsEnabled = true;
        /// <summary>Disables notifications.</summary>
        [HotkeyAction(Description = "Disables notifications.")]
        public void DisableNotification(object? sender, HandledEventArgs e) => Settings.NotificationsEnabled = false;
        /// <summary>Toggles notifications.</summary>
        [HotkeyAction(Description = "Toggles notifications.")]
        public void ToggleNotification(object? sender, HandledEventArgs e) => Settings.NotificationsEnabled = !Settings.NotificationsEnabled;
        /// <summary>Enables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(Description = "Enables notifications for volume/mute change events.")]
        public void EnableNotificationOnVolume(object? sender, HandledEventArgs e) => Settings.NotificationsOnVolumeChange = true;
        /// <summary>Disables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(Description = "Disables notifications for volume/mute change events.")]
        public void DisableNotificationOnVolume(object? sender, HandledEventArgs e) => Settings.NotificationsOnVolumeChange = false;
        /// <summary>Toggles notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(Description = "Toggles notifications for volume/mute change events.")]
        public void ToggleNotificationOnVolume(object? sender, HandledEventArgs e) => Settings.NotificationsOnVolumeChange = !Settings.NotificationsOnVolumeChange;
        [HotkeyAction(Description = "Moves the VolumeControl window in front of all other windows.")]
        public void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(this.MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(Description = "Moves the VolumeControl window behind all other windows.")]
        public void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(this.MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(Description = "Hides the VolumeControl window.")]
        public void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(this.MainHWnd, User32.ECmdShow.SW_MINIMIZE);
        [HotkeyAction(Description = "Restores the VolumeControl window after minimizing it.")]
        public void Unminimize(object? sender, HandledEventArgs e)
        {
            if (HwndSource.FromHwnd(this.MainHWnd).RootVisual is Window w)
            {
                _ = User32.ShowWindow(this.MainHWnd, User32.ECmdShow.SW_RESTORE);
                w.Visibility = Visibility.Visible;
            }
        }
        #endregion Methods
    }
}
