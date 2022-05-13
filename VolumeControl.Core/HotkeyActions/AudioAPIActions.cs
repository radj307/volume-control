using System.ComponentModel;
using VolumeControl.Core.HotkeyActions.Attributes;

namespace VolumeControl.Core.HotkeyActions
{
    /// <summary>
    /// Contains hotkey action handlers that interact with the <see cref="Core.AudioAPI"/> object.
    /// </summary>
    public class AudioAPIActions
    {
        public AudioAPIActions(AudioAPI audioAPI) => AudioAPI = audioAPI;

        /// <summary>
        /// The <see cref="AudioAPI"/> instance to use for volume-related events.
        /// </summary>
        private AudioAPI AudioAPI { get; }

        #region Actions
        [Handler] public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [Handler] public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [Handler] public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [Handler] public void NextTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [Handler] public void PreviousTarget(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [Handler] public void ToggleTargetLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [Handler] public void NextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
        [Handler] public void PreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
        [Handler] public void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
        #endregion Actions
    }
}
