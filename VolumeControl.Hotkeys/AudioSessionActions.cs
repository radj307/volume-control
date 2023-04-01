using System.ComponentModel;
using VolumeControl.Audio;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Hotkeys.Helpers;
using VolumeControl.SDK;

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
        private const string ActionTargetSpecifierName = "Target Override";
        private const string ActionTargetSpecifierDescription = "Overrides the target audio session so that this action only affects the specified audio session(s).";

        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(specifier.Targets[i].Value) is ISession session)
                        session.IncreaseVolume(AudioAPI.VolumeStepSize);
                }
            }
            else AudioAPI.IncrementSessionVolume();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(specifier.Targets[i].Value) is ISession session)
                        session.DecreaseVolume(AudioAPI.VolumeStepSize);
                }
            }
            else AudioAPI.DecrementSessionVolume();
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(specifier.Targets[i].Value) is ISession session)
                        session.Muted = true;
                }
            }
            else AudioAPI.SetSessionMute(true);
        }
        [HotkeyAction(Description = "Unmutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(specifier.Targets[i].Value) is ISession session)
                        session.Muted = false;
                }
            }
            else AudioAPI.SetSessionMute(false);
        }
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (AudioAPI.FindSessionWithName(specifier.Targets[i].Value) is ISession session)
                        session.Muted = !session.Muted;
                }
            }
            else AudioAPI.ToggleSessionMute();
        }
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
    }
}
