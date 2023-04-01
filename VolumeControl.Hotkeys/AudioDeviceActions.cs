using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Audio.Events;
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
        #region Properties
        /// <summary>
        /// The <see cref="Audio.AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private static AudioAPI AudioAPI => _audioAPI ??= VCAPI.Default.AudioAPI;
        private static AudioAPI? _audioAPI = null;

        private static AudioDevice? _selectedDevice;
        public static AudioDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                NotifySelectedDeviceSwitched();
            }
        }
        #endregion Properties

        #region Events
        /// <summary>Triggered when the volume or mute state of the <see cref="SelectedDevice"/> is changed.</summary>
        public static event VolumeChangedEventHandler? SelectedDeviceVolumeChanged;
        private static void NotifySelectedDeviceVolumeChanged(VolumeChangedEventArgs e) => SelectedDeviceVolumeChanged?.Invoke(SelectedDevice, e);
        /// <summary>Triggered when the selected session is changed.</summary>
        public static event EventHandler? SelectedDeviceSwitched;
        private static void NotifySelectedDeviceSwitched() => SelectedDeviceSwitched?.Invoke(SelectedDevice, new());
        #endregion Events

        #region DeviceSelection
        public static int GetDeviceVolume() => SelectedDevice?.Volume ?? -1;
        public static void SetDeviceVolume(int volume)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume = volume;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        public static void IncrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume += amount;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <remarks>This calls <see cref="IncrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="IncrementDeviceVolume(int)"/>
        public static void IncrementDeviceVolume() => IncrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Decrements the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.</param>
        public static void DecrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Volume -= amount;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <remarks>This calls <see cref="DecrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="DecrementDeviceVolume(int)"/>
        public static void DecrementDeviceVolume() => DecrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedDevice"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedDevice"/> is not null and is muted; otherwise false.</returns>
        public static bool GetDeviceMute() => SelectedDevice?.Muted ?? false;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        /// <param name="state">When true, the device will be muted; when false, the device will be unmuted.</param>
        public static void SetDeviceMute(bool state)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Muted = state;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        public static void ToggleDeviceMute()
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.Muted = !dev.Muted;
                NotifySelectedDeviceVolumeChanged(new(dev.Volume, dev.Muted));
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the first element in <see cref="Devices"/> is selected.</remarks>
        public static void SelectNextDevice()
        {
            if (AudioAPI.Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= AudioAPI.Devices.Count)
                    index = 0;
                SelectedDevice = AudioAPI.Devices[index];
            }
            else
            {
                SelectedDevice = AudioAPI.Devices[0];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the last element in <see cref="Devices"/> is selected.</remarks>
        public static void SelectPreviousDevice()
        {
            if (AudioAPI.Devices.Count == 0)
            {
                if (SelectedDevice == null) return;
                SelectedDevice = null;
            }
            else if (SelectedDevice is AudioDevice device)
            {
                int index = AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = AudioAPI.Devices.Count - 1;
                SelectedDevice = AudioAPI.Devices[index];
            }
            else
            {
                SelectedDevice = AudioAPI.Devices[^1];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see langword="null"/>.<br/>
        /// This does nothing if 
        /// <br/>Does nothing if <see cref="SelectedDevice"/> is already null, or if the selected device is locked.
        /// </summary>
        /// <returns><see langword="true"/> when the selected device was set to null &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public static bool DeselectDevice()
        {
            if (SelectedDevice == null)
                return false;

            SelectedDevice = null;
            return true;
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see cref="DefaultDevice"/>.<br/>
        /// Does nothing if the selected device is locked, or if the selected device is already set to the default device.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedDevice"/> was changed &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public static bool SelectDefaultDevice()
        {
            if (AudioAPI.DefaultDevice is AudioDevice device && SelectedDevice != device)
            {
                SelectedDevice = device;
                return true;
            }
            return false;
        }
        #endregion DeviceSelection

        #region Actions
        [HotkeyAction(Description = "Increases the device volume of the selected device.")]
        public void VolumeUp(object? sender, HandledEventArgs e) => IncrementDeviceVolume();
        [HotkeyAction(Description = "Decreases the device volume of the selected device.")]
        public void VolumeDown(object? sender, HandledEventArgs e) => DecrementDeviceVolume();
        [HotkeyAction(Description = "Mutes the selected device.")]
        public void Mute(object? sender, HandledEventArgs e) => SetDeviceMute(true);
        [HotkeyAction(Description = "Unmutes the selected device.")]
        public void Unmute(object? sender, HandledEventArgs e) => SetDeviceMute(false);
        [HotkeyAction(Description = "Toggles the selected device's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e) => ToggleDeviceMute();
        [HotkeyAction(Description = "Selects the next device in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e) => SelectNextDevice();
        [HotkeyAction(Description = "Selects the previous device in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e) => SelectPreviousDevice();
        [HotkeyAction(Description = "Deselects the selected device.")]
        public void Deselect(object? sender, HandledEventArgs e) => DeselectDevice();
        [HotkeyAction(Description = "Selects the default output device in the list.")]
        public void SelectDefault(object? sender, HandledEventArgs e) => SelectDefaultDevice();
        #endregion Actions
    }
}
