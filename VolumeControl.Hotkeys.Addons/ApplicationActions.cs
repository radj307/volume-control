using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using VolumeControl.API;
using VolumeControl.Core.Helpers;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    [ActionAddon(nameof(ApplicationActions))]
    public class ApplicationActions
    {
        #region Constructor
        public ApplicationActions()
        {
            MainHWnd = VCAPI.Default.MainWindowHWnd;
        }
        #endregion Constructor

        #region Properties
        private static IVCSettings VCSettings => VCAPI.Default.Settings;
        private IntPtr MainHWnd { get; }
        #endregion Properties

        #region Fields
        private const string GroupColor = "#F2B2FF";
        private const string GroupName = "Application";
        #endregion Fields

        #region Methods
        /// <summary>Enables notifications.</summary>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Enables notifications.")]
        public void EnableNotification(object? sender, HandledEventArgs e) => VCSettings.NotificationEnabled = true;
        /// <summary>Disables notifications.</summary>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Disables notifications.")]
        public void DisableNotification(object? sender, HandledEventArgs e) => VCSettings.NotificationEnabled = false;
        /// <summary>Toggles notifications.</summary>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Toggles notifications.")]
        public void ToggleNotification(object? sender, HandledEventArgs e) => VCSettings.NotificationEnabled = !VCSettings.NotificationEnabled;
        /// <summary>Enables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Enables notifications for volume/mute change events.")]
        public void EnableNotificationOnVolume(object? sender, HandledEventArgs e) => VCSettings.NotificationShowsVolumeChange = true;
        /// <summary>Disables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Disables notifications for volume/mute change events.")]
        public void DisableNotificationOnVolume(object? sender, HandledEventArgs e) => VCSettings.NotificationShowsVolumeChange = false;
        /// <summary>Toggles notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Toggles notifications for volume/mute change events.")]
        public void ToggleNotificationOnVolume(object? sender, HandledEventArgs e) => VCSettings.NotificationShowsVolumeChange = !VCSettings.NotificationShowsVolumeChange;
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Moves the VolumeControl window in front of all other windows.")]
        public void BringToForeground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Moves the VolumeControl window behind all other windows.")]
        public void SendToBackground(object? sender, HandledEventArgs e) => User32.SetWindowPos(MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Hides the VolumeControl window.")]
        public void Minimize(object? sender, HandledEventArgs e) => User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_MINIMIZE);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Restores the VolumeControl window after minimizing it.")]
        public void Unminimize(object? sender, HandledEventArgs e)
        {
            if (HwndSource.FromHwnd(MainHWnd).RootVisual is Window w)
            {
                User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE);
                w.Visibility = Visibility.Visible;
            }
        }
        #endregion Methods
    }
}
