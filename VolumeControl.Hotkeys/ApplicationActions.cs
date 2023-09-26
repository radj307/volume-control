using System.ComponentModel;
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

        #region Action Methods
        [HotkeyAction(Description = "Toggles whether Session notifications are enabled.")]
        public void ToggleSessionNotifications(object? sender, HandledEventArgs e)
        {
            Settings.SessionListNotificationConfig.Enabled = !Settings.SessionListNotificationConfig.Enabled;
        }
        [HotkeyAction(Description = "Toggles whether Device notifications are enabled.")]
        public void ToggleDeviceNotifications(object? sender, HandledEventArgs e)
        {
            Settings.DeviceListNotificationConfig.Enabled = !Settings.DeviceListNotificationConfig.Enabled;
        }
        [HotkeyAction(Description = "Moves the Volume Control window in front of all other windows.")]
        public void BringToForeground(object? sender, HandledEventArgs e)
        {
            User32.SetWindowPos(this.MainHWnd, User32.HWND_TOP, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        }
        [HotkeyAction(Description = "Moves the Volume Control window behind all other windows.")]
        public void SendToBackground(object? sender, HandledEventArgs e)
        {
            User32.SetWindowPos(this.MainHWnd, User32.HWND_BOTTOM, 0, 0, 0, 0, User32.EUFlags.SWP_NOSIZE | User32.EUFlags.SWP_NOMOVE);
        }
        [HotkeyAction(Description = "Hides the Volume Control window.")]
        public void Minimize(object? sender, HandledEventArgs e)
        {
            User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_HIDE);
        }
        [HotkeyAction(Description = "Shows the Volume Control window.")]
        public void Unminimize(object? sender, HandledEventArgs e)
        {
            User32.ShowWindow(MainHWnd, User32.ECmdShow.SW_RESTORE); //< undo effects from other calls to User32.ShowWindow
            VCAPI.Default.ShowMixer(); //< show the main window
        }
        #endregion Action Methods
    }
}
