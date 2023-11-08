using CoreAudio;
using System.ComponentModel;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.SDK;
using VolumeControl.SDK.DataTemplates;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioDevices in the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [HotkeyActionGroup("Device", GroupColor = "#FF9999", DefaultDataTemplateProvider = typeof(DataTemplateDictionary))]
    public sealed class AudioDeviceActions
    {
        #region Fields
        // volume step
        private const string Setting_VolumeStep_Name = "Volume Step Override";
        private const string Setting_VolumeStep_Description = "Overrides the global volume step for this action.";
        // input/output
        private const string Setting_InputDevice_Name = "Input Device";
        private const string Setting_InputDevice_Description = "When checked, selects the default input device; when unchecked, selects the default output device. Does nothing when input device support is not enabled.";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        private static Config Settings => VCAPI.Settings;
        private static AudioDeviceSelector AudioDeviceSelector => VCAPI.AudioDeviceSelector;
        private static AudioDevice? SelectedDevice => AudioDeviceSelector.Selected;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the selected device.")]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", Description = Setting_VolumeStep_Description, DefaultValue = 2, IsToggleable = true)]
        public void VolumeUp(object? sender, HotkeyPressedEventArgs e)
        {
            if (SelectedDevice == null) return;

            int volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

            SelectedDevice.IncreaseVolume(volumeStep);

            if (!Settings.DeviceListNotificationConfig.ShowOnVolumeChanged) return;

            VCAPI.ShowDeviceListNotification();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected device.")]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", Description = Setting_VolumeStep_Description, DefaultValue = 2, IsToggleable = true)]
        public void VolumeDown(object? sender, HotkeyPressedEventArgs e)
        {
            if (SelectedDevice == null) return;

            int volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

            SelectedDevice.DecreaseVolume(volumeStep);

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
        [HotkeyActionSetting(Setting_InputDevice_Name, typeof(bool), Description = Setting_InputDevice_Description)]
        public void SelectDefault(object? sender, HotkeyPressedEventArgs e)
        {
            AudioDeviceSelector.SelectDefaultDevice(VCAPI.Settings.EnableInputDevices && e.GetValue<bool>(Setting_InputDevice_Name)
                ? DataFlow.Capture
                : DataFlow.Render);

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
        #endregion Action Methods
    }
}
