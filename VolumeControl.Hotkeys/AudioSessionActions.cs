using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.Hotkeys.Helpers;
using VolumeControl.SDK;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioDeviceManager object.
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        #region Fields
        private const string ActionTargetSpecifierName = "Target Override";
        private const string ActionTargetSpecifierDescription = "Overrides the target audio session so that this action only affects the specified audio session(s).";
        public const string DisplayTargetName = "Audio Sessions";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        private static AudioSessionSelector AudioSessionSelector => VCAPI.AudioSessionSelector;
        private static AudioSession? SelectedSession => AudioSessionSelector.Selected;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
                }
            }
            else SelectedSession?.IncreaseVolume(VCAPI.Settings.VolumeStepSize);

            if (!VCAPI.Settings.NotificationsOnVolumeChange) return; //< don't show notifs if they're disabled on volume change

            if (VCAPI.AudioSessionSelector.Selected is not null)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.DecreaseVolume(VCAPI.Settings.VolumeStepSize);
                }
            }
            else SelectedSession?.DecreaseVolume(VCAPI.Settings.VolumeStepSize);

            if (!VCAPI.Settings.NotificationsOnVolumeChange) return; //< don't show notifs if they're disabled on volume change
            
            if (VCAPI.AudioSessionSelector.Selected is not null)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.SetMute(true);
                }
            }
            else SelectedSession?.SetMute(true);

            if (!VCAPI.Settings.NotificationsOnVolumeChange) return; //< don't show notifs if they're disabled on volume change
            
            if (VCAPI.AudioSessionSelector.Selected is not null)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unmutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.SetMute(false);
                }
            }
            else SelectedSession?.SetMute(false);

            if (!VCAPI.Settings.NotificationsOnVolumeChange) return; //< don't show notifs if they're disabled on volume change
            
            if (VCAPI.AudioSessionSelector.Selected is not null)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.ToggleMute();
                }
            }
            else SelectedSession?.ToggleMute();

            if (!VCAPI.Settings.NotificationsOnVolumeChange) return; //< don't show notifs if they're disabled on volume change
            
            if (VCAPI.AudioSessionSelector.Selected is not null)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Selects the next session in the list.")]
        public void SelectNext(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.SelectNextSession();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Selects the previous session in the list.")]
        public void SelectPrevious(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.SelectPreviousSession();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.LockSelection = true;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.LockSelection = false;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.LockSelection = !AudioSessionSelector.LockSelection;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Changes the selected session to null.")]
        public void Deselect(object? sender, HotkeyActionPressedEventArgs e)
        {
            AudioSessionSelector.DeselectSession();

            VCAPI.ShowSessionListNotification();
        }
        #endregion Action Methods
    }
}
