using System.ComponentModel;
using VolumeControl.API;
using VolumeControl.Core.Helpers;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    [ActionAddon(nameof(ApplicationActions))]
    public class ApplicationActions
    {
        private static VCSettingsContainer VCSettings => VCAPI.Default.Settings;

        /// <summary>Enables notifications.</summary>
        [HotkeyAction(ActionDescription = "Enables notifications.")]
        public void EnableNotification(object? sender, HandledEventArgs? e) => VCSettings.NotificationEnabled = true;
        /// <summary>Disables notifications.</summary>
        [HotkeyAction(ActionDescription = "Disables notifications.")]
        public void DisableNotification(object? sender, HandledEventArgs? e) => VCSettings.NotificationEnabled = false;
        /// <summary>Toggles notifications.</summary>
        [HotkeyAction(ActionDescription = "Toggles notifications.")]
        public void ToggleNotification(object? sender, HandledEventArgs? e) => VCSettings.NotificationEnabled = !VCSettings.NotificationEnabled;
        /// <summary>Enables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(ActionDescription = "Enables notifications for volume/mute change events.")]
        public void EnableNotificationOnVolume(object? sender, HandledEventArgs? e) => VCSettings.NotificationShowsVolumeChange = true;
        /// <summary>Disables notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(ActionDescription = "Disables notifications for volume/mute change events.")]
        public void DisableNotificationOnVolume(object? sender, HandledEventArgs? e) => VCSettings.NotificationShowsVolumeChange = false;
        /// <summary>Toggles notifications for volume change events.</summary>
        /// <remarks>This requires notifications to be enabled.</remarks>
        [HotkeyAction(ActionDescription = "Toggles notifications for volume/mute change events.")]
        public void ToggleNotificationOnVolume(object? sender, HandledEventArgs? e) => VCSettings.NotificationShowsVolumeChange = !VCSettings.NotificationShowsVolumeChange;
    }
}
