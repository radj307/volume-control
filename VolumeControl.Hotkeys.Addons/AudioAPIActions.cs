using System.ComponentModel;
using VolumeControl.API;
using VolumeControl.Audio;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [ActionAddon(nameof(AudioAPIActions))]
    public class AudioAPIActions
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
        public int GetDeviceVolume() => SelectedDevice?.EndpointVolume ?? -1;
        public void SetDeviceVolume(int volume)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume = volume;
            }
        }
        public void IncrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume += amount;
            }
        }
        /// <remarks>This calls <see cref="IncrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="IncrementDeviceVolume(int)"/>
        public void IncrementDeviceVolume() => IncrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Decrements the endpoint volume of the currently selected device.<br/>
        /// This affects the maximum volume of all sessions using this endpoint.
        /// </summary>
        /// <param name="amount">The amount to decrease the volume by.</param>
        public void DecrementDeviceVolume(int amount)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointVolume -= amount;
            }
        }
        /// <remarks>This calls <see cref="DecrementDeviceVolume(int)"/> using <see cref="VolumeStepSize"/>.</remarks>
        /// <inheritdoc cref="DecrementDeviceVolume(int)"/>
        public void DecrementDeviceVolume() => DecrementDeviceVolume(AudioAPI.VolumeStepSize);
        /// <summary>
        /// Gets whether the <see cref="SelectedDevice"/> is currently muted.
        /// </summary>
        /// <returns>True if <see cref="SelectedDevice"/> is not null and is muted; otherwise false.</returns>
        public bool GetDeviceMute() => SelectedDevice?.EndpointMuted ?? false;
        /// <summary>
        /// Sets the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        /// <param name="state">When true, the device will be muted; when false, the device will be unmuted.</param>
        public void SetDeviceMute(bool state)
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointMuted = state;
            }
        }
        /// <summary>
        /// Toggles the mute state of <see cref="SelectedDevice"/>.<br/>Does nothing if <see cref="SelectedDevice"/> is null.
        /// </summary>
        /// <remarks>Note that this affects all sessions using this device.</remarks>
        public void ToggleDeviceMute()
        {
            if (SelectedDevice is AudioDevice dev)
            {
                dev.EndpointMuted = !dev.EndpointMuted;
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
            else SelectedDevice = AudioAPI.Devices[0];
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to the device occurring before this one in <see cref="Devices"/>.
        /// <br/>Automatically loops back around if the selection index goes out of range.<br/>
        /// Does nothing unless device selection is unlocked.
        /// </summary>
        /// <remarks>If <see cref="SelectedDevice"/> is set to null, the last element in <see cref="Devices"/> is selected.</remarks>
        public void SelectPreviousDevice()
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
            else SelectedDevice = AudioAPI.Devices[^1];
        }
        /// <summary>
        /// Sets <see cref="SelectedDevice"/> to <see langword="null"/>.<br/>
        /// This does nothing if 
        /// <br/>Does nothing if <see cref="SelectedDevice"/> is already null, or if the selected device is locked.
        /// </summary>
        /// <returns><see langword="true"/> when the selected device was set to null &amp; the <see cref="SelectedDeviceSwitched"/> event was fired; otherwise <see langword="false"/>.</returns>
        public bool DeselectDevice()
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
        public bool SelectDefaultDevice()
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
        [HotkeyAction(ActionDescription = "Increases the volume of the selected session by the value of VolumeStep.")]
        public void IncreaseSessionVolume(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [HotkeyAction(ActionDescription = "Decreases the volume of the selected session by the value of VolumeStep.")]
        public void DecreaseSessionVolume(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [HotkeyAction(ActionDescription = "Mutes the selected session.")]
        public void MuteSession(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(true);
        [HotkeyAction(ActionDescription = "Unmutes the selected session.")]
        public void UnmuteSession(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(false);
        [HotkeyAction(ActionDescription = "Toggles the selected session's mute state.")]
        public void ToggleSessionMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction(ActionDescription = "Selects the next session in the list.")]
        public void SelectNextSession(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction(ActionDescription = "Selects the previous session in the list.")]
        public void SelectPreviousSession(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction(ActionDescription = "Locks the selected session, preventing it from being changed.")]
        public void LockSession(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = true;
        [HotkeyAction(ActionDescription = "Unlocks the selected session, allowing it to be changed.")]
        public void UnlockSession(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = false;
        [HotkeyAction(ActionDescription = "Toggles whether the selected session can be changed or not.")]
        public void ToggleSessionLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction(ActionDescription = "Changes the selected session to null.")]
        public void DeselectSession(object? sender, HandledEventArgs e) => AudioAPI.DeselectSession();
        [HotkeyAction(ActionDescription = "Selects the next device in the list.")]
        public void SelectNextDevice(object? sender, HandledEventArgs e) => SelectNextDevice();
        [HotkeyAction(ActionDescription = "Selects the previous device in the list.")]
        public void SelectPreviousDevice(object? sender, HandledEventArgs e) => SelectPreviousDevice();
        [HotkeyAction(ActionDescription = "Deselects the selected device.")]
        public void DeselectDevice(object? sender, HandledEventArgs e) => DeselectDevice();
        [HotkeyAction(ActionDescription = "Selects the default output device in the list.")]
        public void SelectDefaultDevice(object? sender, HandledEventArgs e) => SelectDefaultDevice();
        [HotkeyAction(ActionDescription = "Increases the device volume of the selected device.")]
        public void IncreaseDeviceVolume(object? sender, HandledEventArgs e) => IncrementDeviceVolume();
        [HotkeyAction(ActionDescription = "Decreases the device volume of the selected device.")]
        public void DecreaseDeviceVolume(object? sender, HandledEventArgs e) => DecrementDeviceVolume();
        [HotkeyAction(ActionDescription = "Mutes the selected device.")]
        public void MuteDevice(object? sender, HandledEventArgs e) => SetDeviceMute(true);
        [HotkeyAction(ActionDescription = "Unmutes the selected device.")]
        public void UnmuteDevice(object? sender, HandledEventArgs e) => SetDeviceMute(false);
        [HotkeyAction(ActionDescription = "Toggles the selected device's mute state.")]
        public void ToggleDeviceMute(object? sender, HandledEventArgs e) => ToggleDeviceMute();
        #endregion Actions
    }
}
