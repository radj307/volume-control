using Newtonsoft.Json;
using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Generics;
using VolumeControl.SDK;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Hotkeys.Addons
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioAPI object.
    /// <see cref="HandledEventHandler"/>
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        private static AudioAPI AudioAPI => VCAPI.Default.AudioAPI;

        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        public void VolumeUp(object? sender, HandledEventArgs e) => AudioAPI.IncrementSessionVolume();
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        public void VolumeDown(object? sender, HandledEventArgs e) => AudioAPI.DecrementSessionVolume();
        [HotkeyAction(Description = "Mutes the selected session.")]
        public void Mute(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(true);
        [HotkeyAction(Description = "Unmutes the selected session.")]
        public void Unmute(object? sender, HandledEventArgs e) => AudioAPI.SetSessionMute(false);
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        public void ToggleMute(object? sender, HandledEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction(Description = "Selects the next session in the list.")]
        public void SelectNext(object? sender, HandledEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction(Description = "Selects the previous session in the list.")]
        public void SelectPrevious(object? sender, HandledEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction(Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = true;
        [HotkeyAction(Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = false;
        [HotkeyAction(Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HandledEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction(Description = "Changes the selected session to null.")]
        public void Deselect(object? sender, HandledEventArgs e) => AudioAPI.DeselectSession();
        [HotkeyAction(Description = "TESTING")]
        public void ActionSettingTestFunction(object? sender, HandledEventArgs e, [HotkeyActionSetting("Setting")] string param, [HotkeyActionSetting("Toggle Switch")] bool toggleSwitch)
        {
            Log.FLog.Log.Info($"{nameof(ActionSettingTestFunction)} was called with setting: '{param}' ({toggleSwitch})");
        }
    }
}
