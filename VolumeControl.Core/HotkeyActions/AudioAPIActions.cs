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
        [Handler("Volume Up")] public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [Handler("Volume Down")] public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [Handler("Toggle Mute")] public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [Handler("Next Session")] public void SelectNextSession(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [Handler("Previous Session")] public void SelectPreviousSession(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [Handler("Toggle Session Lock")] public void ToggleSessionLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [Handler("Next Device")] public void SelectNextDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectNextDevice();
        [Handler("Previous Device")] public void SelectPreviousDevice(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousDevice();
        [Handler("Toggle Device Lock")] public void ToggleDeviceLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedDevice = !AudioAPI.LockSelectedDevice;
        #endregion Actions
    }
}
