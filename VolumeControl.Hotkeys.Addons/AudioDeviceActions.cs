using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Hotkeys.Attributes;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioDevices in the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [ActionAddon(nameof(AudioDeviceActions))]
    public sealed class AudioDeviceActions
    {
        #region Properties
        /// <summary>
        /// The <see cref="Audio.AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private AudioAPI AudioAPI => _audioAPI ??= VCAPI.Default.AudioAPI;
        private AudioAPI? _audioAPI = null;

        public AudioDevice? SelectedDevice { get; set; } = null;
        #endregion Properties

        #region DeviceSelection
        public int GetDeviceVolume() => this.SelectedDevice?.Volume ?? -1;
        public void SetDeviceVolume(int volume)
        {
            if (this.SelectedDevice is AudioDevice dev)
            {
                dev.Volume = volume;
            }
        }
        public void IncrementDeviceVolume(int amount)
        {
            if (this.SelectedDevice is AudioDevice dev)
            {
                dev.Volume += amount;
            }
        }
        /// <remarks>This calls <see cref="IncrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="IncrementDeviceVolume(int)"/>
        public void IncrementDeviceVolume() => this.IncrementDeviceVolume(this.AudioAPI.VolumeStepSize);
        /// <summary>
        /// Decrements the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.</param>
        public void DecrementDeviceVolume(int amount)
        {
            if (this.SelectedDevice is AudioDevice dev)
            {
                dev.Volume -= amount;
            }
        }
        /// <remarks>This calls <see cref="DecrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="DecrementDeviceVolume(int)"/>
        public void DecrementDeviceVolume() => this.DecrementDeviceVolume(this.AudioAPI.VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedDevice"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedDevice"/> is not null and is muted; otherwise false.</returns>
        public bool GetDeviceMute() => this.SelectedDevice?.Muted ?? false;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        /// <param name="state">When true, the device will be muted; when false, the device will be unmuted.</param>
        public void SetDeviceMute(bool state)
        {
            if (this.SelectedDevice is AudioDevice dev)
            {
                dev.Muted = state;
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        public void ToggleDeviceMute()
        {
            if (this.SelectedDevice is AudioDevice dev)
            {
                dev.Muted = !dev.Muted;
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring after this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the first element in <see cref="Devices"/> is selected.</remarks>
        public void SelectNextDevice()
        {
            if (this.AudioAPI.Devices.Count == 0)
            {
                if (this.SelectedDevice == null) return;
                this.SelectedDevice = null;
            }
            else if (this.SelectedDevice is AudioDevice device)
            {
                int index = this.AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index += 1) >= this.AudioAPI.Devices.Count)
                    index = 0;
                this.SelectedDevice = this.AudioAPI.Devices[index];
            }
            else
            {
                this.SelectedDevice = this.AudioAPI.Devices[0];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the last element in <see cref="Devices"/> is selected.</remarks>
        public void SelectPreviousDevice()
        {
            if (this.AudioAPI.Devices.Count == 0)
            {
                if (this.SelectedDevice == null) return;
                this.SelectedDevice = null;
            }
            else if (this.SelectedDevice is AudioDevice device)
            {
                int index = this.AudioAPI.Devices.IndexOf(device);
                if (index == -1 || (index -= 1) < 0)
                    index = this.AudioAPI.Devices.Count - 1;
                this.SelectedDevice = this.AudioAPI.Devices[index];
            }
            else
            {
                this.SelectedDevice = this.AudioAPI.Devices[^1];
            }
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see langword="null"/>.<br/>
        /// This does nothing if 
        /// <br/>Does nothing if <see cref="SelectedDevice"/> is already null, or if the selected device is locked.
        /// </summary>
        /// <returns><see langword="true"/> when the selected device was set to null &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool DeselectDevice()
        {
            if (this.SelectedDevice == null)
                return false;

            this.SelectedDevice = null;
            return true;
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see cref="DefaultDevice"/>.<br/>
        /// Does nothing if the selected device is locked, or if the selected device is already set to the default device.
        /// </summary>
        /// <returns><see langword="true"/> when <see cref="SelectedDevice"/> was changed &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool SelectDefaultDevice()
        {
            if (this.AudioAPI.DefaultDevice is AudioDevice device && this.SelectedDevice != device)
            {
                this.SelectedDevice = device;
                return true;
            }
            return false;
        }
        #endregion DeviceSelection

        #region Actions
        private const string GroupColor = "#FF9999";
        private const string GroupName = "Device";

        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Selects the next device in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e) => this.SelectNextDevice();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Selects the previous device in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e) => this.SelectPreviousDevice();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Deselects the selected device.")]
        public void Deselect(object? sender, HandledEventArgs e) => this.DeselectDevice();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Selects the default output device in the list.")]
        public void SelectDefault(object? sender, HandledEventArgs e) => this.SelectDefaultDevice();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Increases the device volume of the selected device.")]
        public void VolumeUp(object? sender, HandledEventArgs e) => this.IncrementDeviceVolume();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Decreases the device volume of the selected device.")]
        public void VolumeDown(object? sender, HandledEventArgs e) => this.DecrementDeviceVolume();
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Mutes the selected device.")]
        public void Mute(object? sender, HandledEventArgs e) => this.SetDeviceMute(true);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Unmutes the selected device.")]
        public void Unmute(object? sender, HandledEventArgs e) => this.SetDeviceMute(false);
        [HotkeyAction(GroupName = GroupName, GroupColor = GroupColor, Description = "Toggles the selected device's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e) => this.ToggleDeviceMute();
        #endregion Actions
    }
}
