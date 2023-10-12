using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
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
        private static AudioSessionMultiSelector MultiSelector => VCAPI.AudioSessionMultiSelector;
        private static IReadOnlyList<AudioSession> SelectedSessions => MultiSelector.SelectedSessions;
        private static AudioSession? CurrentSession => MultiSelector.CurrentSession;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i].Value) is AudioSession session)
                    {
                        session.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
                        showNotification = true;
                    }
                }
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].IncreaseVolume(VCAPI.Settings.VolumeStepSize);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i].Value) is AudioSession session)
                    {
                        session.DecreaseVolume(VCAPI.Settings.VolumeStepSize);
                        showNotification = true;
                    }
                }
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].DecreaseVolume(VCAPI.Settings.VolumeStepSize);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.DecreaseVolume(VCAPI.Settings.VolumeStepSize);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier), ActionTargetSpecifierDescription)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i].Value) is AudioSession session)
                    {
                        session.SetMute(true);
                        showNotification = true;
                    }
                }
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].SetMute(true);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.SetMute(true);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unmutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier), ActionTargetSpecifierDescription)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i].Value) is AudioSession session)
                    {
                        session.SetMute(false);
                        showNotification = true;
                    }
                }
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].SetMute(false);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.SetMute(false);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(ActionTargetSpecifier), ActionTargetSpecifierDescription)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetActionSettingValue<ActionTargetSpecifier>(ActionTargetSpecifierName) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i].Value) is AudioSession session)
                    {
                        session.ToggleMute();
                        showNotification = true;
                    }
                }
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].ToggleMute();
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.ToggleMute();
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Moves the selector to the next session in the list.")]
        public void SelectNext(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.IncrementCurrentIndex();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Moves the selector to the previous session in the list.")]
        public void SelectPrevious(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.DecrementCurrentIndex();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.LockSelection = true;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.LockSelection = false;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.LockSelection = !MultiSelector.LockSelection;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unsets the selector.")]
        public void Deselect(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.DeselectCurrentItem();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "(De)selects the current session.")]
        public void ToggleSelected(object? sender, HotkeyActionPressedEventArgs e)
        {
            MultiSelector.ToggleSelectCurrentItem();

            VCAPI.ShowSessionListNotification();
        }
        #endregion Action Methods
    }
}
