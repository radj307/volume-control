using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.SDK;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioDevices in the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [HotkeyActionGroup("Device", GroupColor = "#FF9999")]
    public sealed class AudioDeviceActions
    {
        #region Fields
        /// <summary>
        /// The name of the display target associated with this class.
        /// </summary>
        public const string DisplayTargetName = "Audio Devices";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        private static Config Settings => VCAPI.Settings;
        private static AudioDeviceSelector AudioDeviceSelector => VCAPI.AudioDeviceSelector;
        private static AudioDevice? SelectedDevice => AudioDeviceSelector.Selected;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the device volume of the selected device.")]
        public void VolumeUp(object? sender, HandledEventArgs e)
        {
            if (SelectedDevice == null) return;

            SelectedDevice.IncreaseVolume(VCAPI.Settings.VolumeStepSize);

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Decreases the device volume of the selected device.")]
        public void VolumeDown(object? sender, HandledEventArgs e)
        {
            if (SelectedDevice == null) return;

            SelectedDevice.DecreaseVolume(VCAPI.Settings.VolumeStepSize);

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Mutes the selected device.")]
        public void Mute(object? sender, HandledEventArgs e)
        {
            if (SelectedDevice == null) return;

            SelectedDevice.SetMute(true);

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Unmutes the selected device.")]
        public void Unmute(object? sender, HandledEventArgs e)
        {
            if (SelectedDevice == null) return;

            SelectedDevice.SetMute(false);

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Toggles the selected device's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e)
        {
            if (SelectedDevice == null) return;

            SelectedDevice.ToggleMute();

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Selects the next device in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.SelectNextDevice();

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Selects the previous device in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.SelectPreviousDevice();

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Deselects the selected device.")]
        public void Deselect(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.DeselectDevice();

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Selects the default output device in the list.")]
        public void SelectDefault(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.SelectDefaultDevice();

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Locks the selected device, preventing it from being changed.")]
        public void Lock(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.LockSelection = true;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Unlocks the selected device, allow it to be changed.")]
        public void Unlock(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.LockSelection = false;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Toggles whether the selected device can be changed or not.")]
        public void ToggleLock(object? sender, HandledEventArgs e)
        {
            AudioDeviceSelector.LockSelection = !AudioDeviceSelector.LockSelection;

            VCAPI.ShowDeviceListNotification();
        }
        //[HotkeyAction(Description = "Sets the selected audio device as the default audio playback device.")]
        //public void SetDefault(object? sender, HandledEventArgs e)
        //{
        //    if (AudioDeviceSelector.Selected is AudioDevice selected && !selected.IsDefault)
        //    {
        //        CPolicyConfigClient policyConfigClient = new();
        //        policyConfigClient.SetDefaultDevice(selected.ID);
        //    }
        //}
        #endregion Action Methods
    }
}
