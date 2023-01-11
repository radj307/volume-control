using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Log;
using VolumeControl.SDK;
using ActionTargetSpecifier = VolumeControl.WPF.Collections.ObservableImmutableList<VolumeControl.Core.Helpers.TargetInfoVM>;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioAPI object.
    /// <see cref="HandledEventHandler"/>
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        private static AudioAPI AudioAPI => VCAPI.Default.AudioAPI;
        private static LogWriter Log => FLog.Log;
        private const string ActionTargetSpecifierName = "Targets";

        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier))]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier targets && targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(targets[i].ProcessName) is ISession session)
                        session.IncreaseVolume(AudioAPI.VolumeStepSize);
                }
            }
            else AudioAPI.IncrementSessionVolume();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier))]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier targets && targets.Count > 0)
            {
                for (int i = 0; i < targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(targets[i].ProcessName) is ISession session)
                        session.DecreaseVolume(AudioAPI.VolumeStepSize);
                }
            }
            else AudioAPI.DecrementSessionVolume();
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.SetSessionMute(true);
        [HotkeyAction(Description = "Unmutes the selected session.")]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.SetSessionMute(false);
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.ToggleSessionMute();
        [HotkeyAction(Description = "Selects the next session in the list.")]
        public void SelectNext(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.SelectNextSession();
        [HotkeyAction(Description = "Selects the previous session in the list.")]
        public void SelectPrevious(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.SelectPreviousSession();
        [HotkeyAction(Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.LockSelectedSession = true;
        [HotkeyAction(Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.LockSelectedSession = false;
        [HotkeyAction(Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.LockSelectedSession = !AudioAPI.LockSelectedSession;
        [HotkeyAction(Description = "Changes the selected session to null.")]
        public void Deselect(object? sender, HotkeyActionPressedEventArgs e) => AudioAPI.DeselectSession();
        [HotkeyAction(Description = "TESTING")]
        public void ActionSettingTestFunction(object? sender, HotkeyActionPressedEventArgs e, [HotkeyActionSetting("Setting", typeof(string))] string param, [HotkeyActionSetting("Toggle Switch", typeof(bool))] bool toggleSwitch)
        {
            Log.Info($"{nameof(ActionSettingTestFunction)} was called with setting: '{param}' ({toggleSwitch})");
        }
    }
}
