using Audio;
using Audio.Helpers;
using System.ComponentModel;
using VolumeControl.Core.Attributes;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys
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
        private static AudioDeviceSelector AudioDeviceSelector => VCAPI.AudioDeviceSelector;
        private static AudioDevice? SelectedDevice => AudioDeviceSelector.Selected;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the device volume of the selected device.")]
        public void VolumeUp(object? sender, HandledEventArgs e)
            => SelectedDevice?.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
        [HotkeyAction(Description = "Decreases the device volume of the selected device.")]
        public void VolumeDown(object? sender, HandledEventArgs e)
            => SelectedDevice?.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
        [HotkeyAction(Description = "Mutes the selected device.")]
        public void Mute(object? sender, HandledEventArgs e)
            => SelectedDevice?.SetMute(true);
        [HotkeyAction(Description = "Unmutes the selected device.")]
        public void Unmute(object? sender, HandledEventArgs e)
            => SelectedDevice?.SetMute(false);
        [HotkeyAction(Description = "Toggles the selected device's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e)
            => SelectedDevice?.ToggleMute();
        [HotkeyAction(Description = "Selects the next device in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.SelectNextDevice();
        [HotkeyAction(Description = "Selects the previous device in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.SelectPreviousDevice();
        [HotkeyAction(Description = "Deselects the selected device.")]
        public void Deselect(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.DeselectDevice();
        [HotkeyAction(Description = "Selects the default output device in the list.")]
        public void SelectDefault(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.SelectDefaultDevice();
        [HotkeyAction(Description = "Locks the selected device, preventing it from being changed.")]
        public void Lock(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.LockSelection = true;
        [HotkeyAction(Description = "Unlocks the selected device, allow it to be changed.")]
        public void Unlock(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.LockSelection = false;
        [HotkeyAction(Description = "Toggles whether the selected device can be changed or not.")]
        public void ToggleLock(object? sender, HandledEventArgs e)
            => AudioDeviceSelector.LockSelection = !AudioDeviceSelector.LockSelection;
        #endregion Action Methods
    }
}
