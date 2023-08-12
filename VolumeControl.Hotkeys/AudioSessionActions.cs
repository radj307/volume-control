using Audio;
using Audio.Helpers;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Helpers;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Hotkeys.Helpers;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioDeviceManager object.
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;

        private static AudioSession? _selectedSession;
        public static AudioSession? SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                VCAPI.Settings.Target = _selectedSession?.GetTargetInfo() ?? TargetInfo.Empty;
            }
        }
        public static bool SessionSelectionLocked
        {
            get => VCAPI.Settings.LockTargetSession;
            set => VCAPI.Settings.LockTargetSession = value;
        }

        private const string ActionTargetSpecifierName = "Target Override";
        private const string ActionTargetSpecifierDescription = "Overrides the target audio session so that this action only affects the specified audio session(s).";
        public const string DisplayTargetName = "Audio Sessions";
        #endregion Properties

        #region Methods
        private void SelectNextSession()
        {
            // if the selected session is locked, return
            if (SessionSelectionLocked) return;

            if (VCAPI.AudioSessionManager.Sessions.Count == 0)
            {
                if (SelectedSession == null) return;
                SelectedSession = null;
            }
            if (SelectedSession is AudioSession session)
            { // a valid audio session is selected
                //ResolveTarget();
                int index = VCAPI.AudioSessionManager.Sessions.IndexOf(session);
                if (index == -1 || (index += 1) >= VCAPI.AudioSessionManager.Sessions.Count)
                    index = 0;
                SelectedSession = VCAPI.AudioSessionManager.Sessions[index];
            }
            // nothing is selected, select the first element in the list
            else if (VCAPI.AudioSessionManager.Sessions.Count > 0)
            {
                SelectedSession = VCAPI.AudioSessionManager.Sessions[0];
            }

            // TODO: Notify
        }
        private void SelectPreviousSession()
        {
            // if the selected session is locked, return
            if (SessionSelectionLocked) return;

            if (VCAPI.AudioSessionManager.Sessions.Count == 0)
            {
                if (SelectedSession == null) return;
                SelectedSession = null;
            }
            if (SelectedSession is AudioSession session)
            { // a valid audio session is selected
                //ResolveTarget();
                int index = VCAPI.AudioSessionManager.Sessions.IndexOf(session);
                if (index == -1 || (index -= 1) < 0)
                    index = VCAPI.AudioSessionManager.Sessions.Count - 1;
                SelectedSession = VCAPI.AudioSessionManager.Sessions[index];
            }
            // nothing is selected, select the last element in the list
            else if (VCAPI.AudioSessionManager.Sessions.Count > 0)
            {
                SelectedSession = VCAPI.AudioSessionManager.Sessions[^1];
            }

            // TODO: Notify
        }
        private void DeselectSession()
        {
            // if the selected session is locked, return
            if (SessionSelectionLocked) return;

            SelectedSession = null;

            // TODO: Notify
        }
        #endregion Methods

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioDeviceManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
                }
            }
            else SelectedSession?.IncreaseVolume(VCAPI.Settings.VolumeStepSize);
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioDeviceManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.DecreaseVolume(VCAPI.Settings.VolumeStepSize);
                }
            }
            else SelectedSession?.DecreaseVolume(VCAPI.Settings.VolumeStepSize);
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioDeviceManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.SetMute(true);
                }
            }
            else SelectedSession?.SetMute(true);
        }
        [HotkeyAction(Description = "Unmutes the selected session.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioDeviceManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.SetMute(false);
                }
            }
            else SelectedSession?.SetMute(false);
        }
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
            {
                for (int i = 0; i < specifier.Targets.Count; ++i)
                {
                    if (VCAPI.AudioDeviceManager.FindSessionWithProcessName(specifier.Targets[i].Value) is AudioSession session)
                        session.ToggleMute();
                }
            }
            else SelectedSession?.ToggleMute();
        }
        [HotkeyAction(Description = "Selects the next session in the list.")]
        public void SelectNext(object? sender, HotkeyActionPressedEventArgs e)
            => SelectNextSession();
        [HotkeyAction(Description = "Selects the previous session in the list.")]
        public void SelectPrevious(object? sender, HotkeyActionPressedEventArgs e)
            => SelectPreviousSession();
        [HotkeyAction(Description = "Locks the selected session, preventing it from being changed.")]
        public void Lock(object? sender, HotkeyActionPressedEventArgs e)
            => SessionSelectionLocked = true;
        [HotkeyAction(Description = "Unlocks the selected session, allowing it to be changed.")]
        public void Unlock(object? sender, HotkeyActionPressedEventArgs e)
            => SessionSelectionLocked = false;
        [HotkeyAction(Description = "Toggles whether the selected session can be changed or not.")]
        public void ToggleLock(object? sender, HotkeyActionPressedEventArgs e)
            => SessionSelectionLocked = !SessionSelectionLocked;
        [HotkeyAction(Description = "Changes the selected session to null.")]
        public void Deselect(object? sender, HotkeyActionPressedEventArgs e)
            => DeselectSession();
        #endregion Action Methods
    }
}
