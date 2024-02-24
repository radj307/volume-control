using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.SDK;
using VolumeControl.SDK.DataTemplates;

namespace VolumeControl.HotkeyActions
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioDeviceManager object.
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99", DefaultDataTemplateProvider = typeof(DataTemplateDictionary))]
    public sealed class AudioSessionActions
    {
        // NOTE: Don't change any names without also changing the translation keys in en-HotkeyActions.json!

        #region Fields
        // Target Override(s)
        private const string Setting_TargetOverride_Name = "Target Override(s)";
        private const string Setting_TargetOverride_Description = "Causes this action to only affect the specified audio sessions.";
        // Select Target Override(s)
        private const string Setting_SelectTarget_Name = "Select Target Override(s)";
        private const string Setting_SelectTarget_Description = "Selects the target override sessions when the action is triggered.";
        // Target All Sessions
        private const string Setting_TargetAll_Name = "Target All Sessions";
        private const string Setting_TargetAll_Description = "Ignores the target override(s) and applies the action to all audio sessions when checked.";
        // Volume Step
        private const string Setting_VolumeStep_Name = "Volume Step Override";
        private const string Setting_VolumeStep_Description = "Overrides the global volume step for this action.";
        // Volume Level
        private const string Setting_VolumeLevel_Name = "Volume Level";
        private const string Setting_VolumeLevel_Description = "The volume level to set the target session(s) to.";
        // Mute State
        private const string Setting_MuteState_Name = "Mute State";
        private const string Setting_MuteState_Description = "The mute state to set the target session(s) to.";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        private static AudioSessionMultiSelector MultiSelector => VCAPI.AudioSessionMultiSelector;
        private static IReadOnlyList<AudioSession> SelectedSessions => MultiSelector.SelectedSessions;
        private static AudioSession? CurrentSession => MultiSelector.CurrentSession;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeUp(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            // get the volume step size
            int volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    foreach (var session in VCAPI.AudioSessionManager.FindSessionsWithName(specifier.Targets[i], includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions))
                    {
                        session.IncreaseVolume(volumeStep);
                        sessions.Add(session);
                        showNotification = true;
                    }
                }
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions.ToArray());
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].IncreaseVolume(volumeStep);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.IncreaseVolume(volumeStep);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Decreases the volume of the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), "VolumeStepDataTemplate", DefaultValue = 2, Description = Setting_VolumeStep_Description, IsToggleable = true)]
        public void VolumeDown(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            // get the volume step size
            int volumeStep = e.GetValueOrDefault(Setting_VolumeStep_Name, VCAPI.Settings.VolumeStepSize);

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    foreach (var session in VCAPI.AudioSessionManager.FindSessionsWithName(specifier.Targets[i], includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions))
                    {
                        session.DecreaseVolume(volumeStep);
                        sessions.Add(session);
                        showNotification = true;
                    }
                }
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions.ToArray());
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    SelectedSessions[i].DecreaseVolume(volumeStep);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.DecreaseVolume(volumeStep);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Sets the volume and/or mute state of the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_TargetAll_Name, typeof(bool), Description = Setting_TargetAll_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeLevel_Name, typeof(int), "VolumeLevelDataTemplate", DefaultValue = 50, Description = Setting_VolumeLevel_Description, IsToggleable = true, StartsEnabled = true)]
        [HotkeyActionSetting(Setting_MuteState_Name, typeof(bool), Description = Setting_MuteState_Description, IsToggleable = true)]
        public void SetVolume(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            var (setVolumeLevel, volumeLevel) = e.GetSetting<int>(Setting_VolumeLevel_Name);
            var (setMuteState, muteState) = e.GetSetting<bool>(Setting_MuteState_Name);

            var targetAllSessions = e.GetValue<bool>(Setting_TargetAll_Name);
            var targetSpecifier = e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name);

            if (targetAllSessions || targetSpecifier.Targets.Count > 0)
            { // operate on target overrides:
                var selectTargets = e.GetValue<bool>(Setting_SelectTarget_Name);

                foreach (var session in VCAPI.AudioSessionManager.Sessions)
                {
                    if (targetAllSessions || targetSpecifier.Targets.Any(targetName => session.HasMatchingName(targetName, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (setVolumeLevel)
                            session.Volume = volumeLevel;
                        if (setMuteState)
                            session.Mute = muteState;
                        sessions.Add(session);
                        showNotification = true;
                    }
                }

                if (selectTargets)
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions!.ToArray());
            }
            else if (SelectedSessions.Count > 0)
            { // operate on selected sessions:
                for (int i = 0, max = SelectedSessions.Count; i < max; ++i)
                {
                    var session = SelectedSessions[i];
                    if (setVolumeLevel)
                        session.Volume = volumeLevel;
                    if (setMuteState)
                        session.Mute = muteState;
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current session:
                if (setVolumeLevel)
                    CurrentSession.Volume = volumeLevel;
                if (setMuteState)
                    CurrentSession.Mute = muteState;
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Mutes the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Mute(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    foreach (var session in VCAPI.AudioSessionManager.FindSessionsWithName(specifier.Targets[i], includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions))
                    {
                        session.SetMute(true);
                        sessions.Add(session);
                        showNotification = true;
                    }
                }
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions.ToArray());
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
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Unmutes the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Unmute(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    foreach (var session in VCAPI.AudioSessionManager.FindSessionsWithName(specifier.Targets[i], includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions))
                    {
                        session.SetMute(false);
                        sessions.Add(session);
                        showNotification = true;
                    }
                }
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions.ToArray());
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
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Toggles the mute state of the session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), "ActionTargetSpecifierDataTemplate", Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void ToggleMute(object? sender, HotkeyPressedEventArgs e)
        {
            bool showNotification = false;
            List<AudioSession> sessions = new();

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    foreach (var session in VCAPI.AudioSessionManager.FindSessionsWithName(specifier.Targets[i], includeHiddenSessions: true, includeInactiveSessions: !VCAPI.Settings.HideInactiveSessions))
                    {
                        session.ToggleMute();
                        sessions.Add(session);
                        showNotification = true;
                    }
                }
                if (e.GetValue<bool>(Setting_SelectTarget_Name))
                    MultiSelector.SetSelectedSessionsOrCurrentSession(sessions.ToArray());
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
                VCAPI.ShowSessionListNotification(sessions);
        }
        [HotkeyAction(Description = "Changes the current session to the next session in the list.")]
        public void SelectNext(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.IncrementCurrentIndex();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Changes the current session to the previous session in the list.")]
        public void SelectPrevious(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.DecrementCurrentIndex();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unsets the current session.")]
        public void Deselect(object? sender, HotkeyPressedEventArgs e)
        {
            if (MultiSelector.HasSelectedSessions)
            {
                MultiSelector.ClearSelectedSessions();
            }
            MultiSelector.DeselectCurrentItem();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Locks the current session, preventing it from being changed.")]
        public void Lock(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.LockSelection = true;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unlocks the current session, allowing it to be changed again.")]
        public void Unlock(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.LockSelection = false;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles whether the current session is locked or unlocked. When locked, it cannot be changed until unlocked.")]
        public void ToggleLock(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.LockSelection = !MultiSelector.LockSelection;

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles whether the current session is selected or not. Multiple sessions can be selected, but only one can be the current session.")]
        public void ToggleSelected(object? sender, HotkeyPressedEventArgs e)
        {
            MultiSelector.ToggleSelectCurrentItem();

            VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Shows the session notification.")]
        public void Show(object? sender, HotkeyPressedEventArgs e)
        {
            VCAPI.ShowSessionListNotification();
        }
        #endregion Action Methods
    }
}
