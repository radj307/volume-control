using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [ActionAddon(nameof(AudioAPIActions))]
    public class AudioAPIActions
    {
        public AudioAPIActions(AudioAPI audioAPI) => AudioAPI = audioAPI;

        /// <summary>
        /// The <see cref="AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private AudioAPI AudioAPI { get; }

        #region Actions
        [HotkeyAction] public void IncreaseSessionVolume(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [HotkeyAction] public void DecreaseSessionVolume(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [HotkeyAction] public void MuteSession(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(true);
        [HotkeyAction] public void UnmuteSession(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(false);
        [HotkeyAction] public void ToggleSessionMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction] public void SelectNextSession(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction] public void SelectPreviousSession(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction] public void ToggleSessionLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction] public void DeselectSession(object? sender, HandledEventArgs e) => AudioAPI.DeselectSession();
        [HotkeyAction] public void SelectNextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
        [HotkeyAction] public void SelectPreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
        [HotkeyAction] public void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
        [HotkeyAction] public void DeselectDevice(object? sender, HandledEventArgs e) => AudioAPI.DeselectDevice();
        [HotkeyAction] public void SelectDefaultDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectDefaultDevice();
        [HotkeyAction] public void IncreaseDeviceVolume(object? sender, HandledEventArgs e) => AudioAPI.IncrementDeviceVolume();
        [HotkeyAction] public void DecreaseDeviceVolume(object? sender, HandledEventArgs e) => AudioAPI.DecrementDeviceVolume();
        [HotkeyAction] public void MuteDevice(object? sender, HandledEventArgs e) => AudioAPI.SetDeviceMute(true);
        [HotkeyAction] public void UnmuteDevice(object? sender, HandledEventArgs e) => AudioAPI.SetDeviceMute(false);
        [HotkeyAction] public void ToggleDeviceMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleDeviceMute();
        #endregion Actions
    }
}
