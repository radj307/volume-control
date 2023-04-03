using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using VolumeControl.Audio;
using VolumeControl.Audio.Interfaces;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Interfaces;
using VolumeControl.Hotkeys.Helpers;
using VolumeControl.SDK;
using VolumeControl.WPF.Converters;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Hotkeys
{
    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioAPI object.
    /// <see cref="HandledEventHandler"/>
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        static AudioSessionActions()
        {
            // sessions
            ListDisplayTarget ldtSessions = new(DisplayTargetName);
            ldtSessions.SetBinding(ListDisplayTarget.ItemsSourceProperty, new Binding()
            {
                Source = AudioAPI,
                Path = new PropertyPath(nameof(AudioAPI.Sessions))
            });
            ldtSessions.SetBinding(ListDisplayTarget.SelectedItemProperty, new Binding()
            {
                Source = AudioAPI,
                Path = new PropertyPath(nameof(AudioAPI.SelectedSession)),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });
            ldtSessions.SetBinding(ListDisplayTarget.LockSelectionProperty, new Binding()
            {
                Source = AudioAPI,
                Path = new PropertyPath(nameof(AudioAPI.LockSelectedSession))
            });
            ldtSessions.SetBinding(ListDisplayTarget.BackgroundProperty, new Binding()
            {
                Source = AudioAPI,
                Path = new PropertyPath(nameof(AudioAPI.LockSelectedSession)),
                Mode = BindingMode.OneWay,
                Converter = new BoolToBrushConverter()
                {
                    WhenTrue = Config.NotificationLockedBrush,
                    WhenFalse = Config.NotificationUnlockedBrush
                }
            });
            ldtSessions.SetBinding(ListDisplayTarget.SelectedItemControlsProperty, new Binding()
            {
                RelativeSource = RelativeSource.Self,
                Path = new PropertyPath($"{nameof(ListDisplayTarget.SelectedItem)}.{nameof(IListDisplayable.DisplayControls)}"),
            });
            // session target show triggers
            var cefSessionSwitched = ldtSessions.AddConditionalEventForward(() => Settings.NotificationsEnabled);
            AudioAPI.SelectedSessionSwitched += (s, e) =>
            {
                cefSessionSwitched.Handler(s, e);
                if (!ldtSessions.IsSelected) return;
                ldtSessions.RaisePropertyChanged(nameof(ldtSessions.SelectedItem));
                ldtSessions.RaisePropertyChanged(nameof(ldtSessions.SelectedItemControls));
            };
            var cefLockSelectedSessionChanged = ldtSessions.AddConditionalEventForward(() => Settings.NotificationsEnabled);
            AudioAPI.LockSelectedSessionChanged += (s, e) =>
            {
                cefLockSelectedSessionChanged.Handler(s, e);
                if (!ldtSessions.IsSelected || !cefSessionSwitched.Condition()) return;
                ldtSessions.RaisePropertyChanged(nameof(ldtSessions.Background));
            };
            AudioAPI.SelectedSessionVolumeChanged += ldtSessions.AddConditionalEventForward(() => Settings.NotificationsEnabled && Settings.NotificationsOnVolumeChange).Handler;

            VCAPI.Default.ListDisplayTargets.Add(ldtSessions);
        }

        private static Config Settings => VCAPI.Default.Settings;
        private static AudioAPI AudioAPI => VCAPI.Default.AudioAPI;
        private const string ActionTargetSpecifierName = "Target Override";
        private const string ActionTargetSpecifierDescription = "Overrides the target audio session so that this action only affects the specified audio session(s).";
        public const string DisplayTargetName = "Audio Sessions";

        [HotkeyAction(Description = "Increases the volume of the selected session by the value of VolumeStep.")]
        [HotkeyActionSetting(ActionTargetSpecifierName, typeof(SessionSpecifier), ActionTargetSpecifierDescription)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
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
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
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
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
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
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
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
            if (e.GetActionSettingValue<SessionSpecifier>(ActionTargetSpecifierName) is SessionSpecifier specifier && specifier.Targets.Count > 0)
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
