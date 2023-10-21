using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.CoreAudio;
using VolumeControl.CoreAudio.Helpers;
using VolumeControl.SDK;
using VolumeControl.WPF.Behaviors;
using VolumeControl.WPF.Collections;
using VolumeControl.WPF.Controls;

namespace VolumeControl.HotkeyActions
{
    #region DataTemplateProviders
    public abstract class NumericUpDown_DataTemplateProvider : DataTemplateProvider
    {
        protected FrameworkElementFactory GetNumericUpDownFactory()
        {
            var numericUpDownFactory = new FrameworkElementFactory(typeof(NumericUpDown));

            // Set appearance-related values
            numericUpDownFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            numericUpDownFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(3, 1, 3, 1));

            // Bind NumericUpDown.Value => IActionSettingInstance.Value
            numericUpDownFactory.SetBinding(NumericUpDown.ValueProperty, new Binding(nameof(IActionSettingInstance.Value)));

            // Attach behaviors through the Loaded event
            numericUpDownFactory.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler((sender, e) =>
            {
                Interaction.GetBehaviors((NumericUpDown)sender).Add(new MouseWheelNumericUpDownBehavior());
            }));

            return numericUpDownFactory;
        }
    }
    /// <summary>
    /// <see cref="NumericUpDown_DataTemplateProvider"/> with MinValue 1 and MaxValue 100
    /// </summary>
    public class VolumeStep_NumericUpDown_DataTemplateProvider : NumericUpDown_DataTemplateProvider
    {
        public override DataTemplate ProvideDataTemplate()
        {
            var numericUpDownFactory = GetNumericUpDownFactory();

            // Set Min/Max values
            numericUpDownFactory.SetValue(NumericUpDown.MinValueProperty, 1m);
            numericUpDownFactory.SetValue(NumericUpDown.MaxValueProperty, 100m);

            return new DataTemplate(typeof(int)) { VisualTree = numericUpDownFactory };
        }
    }
    /// <summary>
    /// <see cref="NumericUpDown_DataTemplateProvider"/> with MinValue 0 and MaxValue 100
    /// </summary>
    public class VolumeLevel_NumericUpDown_DataTemplateProvider : NumericUpDown_DataTemplateProvider
    {
        public override DataTemplate ProvideDataTemplate()
        {
            var numericUpDownFactory = GetNumericUpDownFactory();

            // Set Min/Max values
            numericUpDownFactory.SetValue(NumericUpDown.MinValueProperty, 0m);
            numericUpDownFactory.SetValue(NumericUpDown.MaxValueProperty, 100m);

            return new DataTemplate(typeof(int)) { VisualTree = numericUpDownFactory };
        }
    }
    #endregion DataTemplateProviders

    /// <summary>
    /// Contains hotkey action handlers that interact with AudioSessions in the AudioDeviceManager object.
    /// </summary>
    [HotkeyActionGroup("Session", GroupColor = "#99FF99")]
    public sealed class AudioSessionActions
    {
        #region Fields
        // Target Override(s)
        private const string Setting_TargetOverride_Name = "Target Override(s)";
        private const string Setting_TargetOverride_Description = "Causes this action to only affect the specified audio sessions.";
        // Select Target Override(s)
        private const string Setting_SelectTarget_Name = "Select Target Override(s)";
        private const string Setting_SelectTarget_Description = "Selects the target override sessions when the action is triggered.";
        // Volume Step
        private const string Setting_OverrideVolumeStep_Name = "Override Volume Step";
        private const string Setting_OverrideVolumeStep_Description = "Uses the volume step specified below instead of the default global volume step.";
        private const string Setting_VolumeStep_Name = "Volume Step";
        private const string Setting_VolumeStep_Description = "Overrides the default volume step.";
        // Volume Level
        private const string Setting_VolumeLevel_Name = "Volume Level";
        private const string Setting_VolumeLevel_Description = "The volume level to set the target sessions to.";
        #endregion Fields

        #region Properties
        private static VCAPI VCAPI => VCAPI.Default;
        private static AudioSessionMultiSelector MultiSelector => VCAPI.AudioSessionMultiSelector;
        private static IReadOnlyList<AudioSession> SelectedSessions => MultiSelector.SelectedSessions;
        private static AudioSession? CurrentSession => MultiSelector.CurrentSession;
        #endregion Properties

        #region Action Methods
        [HotkeyAction(Description = "Increases the volume of the selected session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_OverrideVolumeStep_Name, typeof(bool), Description = Setting_OverrideVolumeStep_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), typeof(VolumeStep_NumericUpDown_DataTemplateProvider), DefaultValue = 2, Description = Setting_VolumeStep_Description)]
        public void VolumeUp(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;

            // get the volume step size
            int volumeStepSize = e.GetValue<bool>(Setting_OverrideVolumeStep_Name)
                ? e.GetValue<int>(Setting_VolumeStep_Name)
                : VCAPI.Settings.VolumeStepSize;

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
                    {
                        session.IncreaseVolume(volumeStepSize);
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
                    SelectedSessions[i].IncreaseVolume(volumeStepSize);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.IncreaseVolume(volumeStepSize);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Decreases the volume of the selected session(s).")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_OverrideVolumeStep_Name, typeof(bool), Description = Setting_OverrideVolumeStep_Description)]
        [HotkeyActionSetting(Setting_VolumeStep_Name, typeof(int), typeof(VolumeStep_NumericUpDown_DataTemplateProvider), DefaultValue = 2, Description = Setting_VolumeStep_Description)]
        public void VolumeDown(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;

            // get the volume step size
            int volumeStepSize = e.GetValue<bool>(Setting_OverrideVolumeStep_Name)
                ? e.GetValue<int>(Setting_VolumeStep_Name)
                : VCAPI.Settings.VolumeStepSize;

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
                    {
                        session.DecreaseVolume(volumeStepSize);
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
                    SelectedSessions[i].DecreaseVolume(volumeStepSize);
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            { // operate on current item:
                CurrentSession.DecreaseVolume(volumeStepSize);
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Sets the volume level of the specified session(s) to a pre-configured level.")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        [HotkeyActionSetting(Setting_VolumeLevel_Name, typeof(int), typeof(VolumeLevel_NumericUpDown_DataTemplateProvider), DefaultValue = 50, Description = Setting_VolumeLevel_Description)]
        public void SetVolume(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;

            var volumeLevel = e.GetValue<int>(Setting_VolumeLevel_Name);

            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
                    {
                        session.Volume = volumeLevel;
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
                    SelectedSessions[i].Volume = volumeLevel;
                    showNotification = true;
                }
            }
            else if (CurrentSession != null)
            {
                CurrentSession.Volume = volumeLevel;
                showNotification = true;
            }

            if (!VCAPI.Settings.SessionListNotificationConfig.ShowOnVolumeChanged)
                return; //< don't show notifs if they're disabled on volume change

            if (showNotification)
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Mutes the selected session.")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Mute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
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
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Unmutes the selected session.")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void Unmute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
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
                VCAPI.ShowSessionListNotification();
        }
        [HotkeyAction(Description = "Toggles the selected session's mute state.")]
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), Description = Setting_TargetOverride_Description)]
        [HotkeyActionSetting(Setting_SelectTarget_Name, typeof(bool), Description = Setting_SelectTarget_Description)]
        public void ToggleMute(object? sender, HotkeyActionPressedEventArgs e)
        {
            bool showNotification = false;
            if (e.GetValue<ActionTargetSpecifier>(Setting_TargetOverride_Name) is ActionTargetSpecifier specifier && specifier.Targets.Count > 0)
            { // operate on target overrides:
                List<AudioSession> sessions = new();
                for (int i = 0, max = specifier.Targets.Count; i < max; ++i)
                {
                    if (VCAPI.AudioSessionManager.FindSessionWithName(specifier.Targets[i]) is AudioSession session)
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
