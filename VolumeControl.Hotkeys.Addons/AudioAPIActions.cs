using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Hotkeys.Attributes;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with the <see cref="Audio.AudioAPI"/> object.
    /// </summary>
    [ActionAddon]
    public class AudioAPIActions
    {
        public AudioAPIActions(AudioAPI audioAPI) => AudioAPI = audioAPI;

        /// <summary>
        /// The <see cref="AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private AudioAPI AudioAPI { get; }

        #region Actions
        [HotkeyAction] public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [HotkeyAction] public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [HotkeyAction] public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction] public void SelectNextSession(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction] public void SelectPreviousSession(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction] public void ToggleSessionLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction] public void SelectNextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
        [HotkeyAction] public void SelectPreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
        [HotkeyAction] public void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
        #endregion Actions
    }
}
