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

    public class ActionTargetSpecifierDataTemplateProvider : DataTemplateProvider
    {
        public override DataTemplate ProvideDataTemplate()
        {
            // create root grid
            var rootGrid = new FrameworkElementFactory(typeof(Grid));

            // create grid row 0
            #region rootGrid_row0
            var rootGrid_row0 = new FrameworkElementFactory(typeof(RowDefinition));
            rootGrid_row0.SetValue(RowDefinition.HeightProperty, GridLength.Auto);

            // create item template for the listbox
            #region TargetListBox ItemTemplate
            var itemTemplate_grid = new FrameworkElementFactory(typeof(Grid));

            // ItemTemplate Column 0
            #region itemTemplate_col0
            var itemTemplate_col0 = new FrameworkElementFactory(typeof(ColumnDefinition));
            itemTemplate_col0.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);

            #region itemTemplate_textBox
            var itemTemplate_textBox = new FrameworkElementFactory(typeof(TextBox), "ItemTextBox");
            itemTemplate_textBox.SetBinding(TextBox.TextProperty, new Binding(".") { UpdateSourceTrigger = UpdateSourceTrigger.LostFocus });
            #endregion itemTemplate_textBox

            itemTemplate_col0.AppendChild(itemTemplate_textBox);
            itemTemplate_grid.AppendChild(itemTemplate_col0);
            #endregion itemTemplate_col0

            // ItemTemplate Column 1
            #region itemTemplate_col1
            var itemTemplate_col1 = new FrameworkElementFactory(typeof(ColumnDefinition));
            itemTemplate_col1.SetValue(ColumnDefinition.WidthProperty, GridLength.Auto);

            #region itemTemplate_button
            var itemTemplate_button = new FrameworkElementFactory(typeof(Button));
            itemTemplate_button.SetBinding(Button.WidthProperty, new Binding(nameof(Button.ActualHeight)) { RelativeSource = RelativeSource.Self });
            itemTemplate_button.SetBinding(Button.HeightProperty, new Binding(nameof(TextBox.Height)) { ElementName = "ItemTextBox" });
            itemTemplate_button.SetBinding(Button.TagProperty, new Binding() { RelativeSource = new() { Mode = RelativeSourceMode.FindAncestor, AncestorType = typeof(ListBox) } });
            itemTemplate_button.SetBinding(Button.DataContextProperty, new Binding() { RelativeSource = new() { Mode = RelativeSourceMode.FindAncestor, AncestorType = typeof(ListBoxItem) } });
            itemTemplate_button.SetValue(Button.ContentProperty, "❌");
            itemTemplate_button.SetValue(Button.FontSizeProperty, 9.0);
            itemTemplate_button.AddHandler(Button.ClickEvent, new RoutedEventHandler((sender, e) =>
            { // Button.Click
                var button = (Button)sender;
                var listBox = (ListBox)button.Tag;
                var listBoxItem = (ListBoxItem)button.DataContext;

                if (((IActionSettingInstance)listBox.DataContext).Value is not ActionTargetSpecifier setting) return;

                var index = listBox.ItemContainerGenerator.IndexFromContainer(listBoxItem);
                setting.Targets.RemoveAt(index);
            }));
            #endregion itemTemplate_button

            itemTemplate_col1.AppendChild(itemTemplate_button);
            itemTemplate_grid.AppendChild(itemTemplate_col1);
            #endregion itemTemplate_col1
            #endregion TargetListBox ItemTemplate

            // create listbox
            #region TargetListBox
            var targetListBox = new FrameworkElementFactory(typeof(ListBox), "TargetListBox");
            targetListBox.SetBinding(ListBox.ItemsSourceProperty, new Binding($"Value.Targets"));
            targetListBox.SetValue(ListBox.ItemTemplateProperty, new DataTemplate(typeof(string)) { VisualTree = itemTemplate_grid });
            #endregion TargetListBox

            // add the row into the root grid
            rootGrid.AppendChild(rootGrid_row0);
            #endregion rootGrid_row0

            // create grid row 1
            #region rootGrid_row1
            var rootGrid_row1 = new FrameworkElementFactory(typeof(RowDefinition));
            rootGrid_row1.SetValue(RowDefinition.HeightProperty, GridLength.Auto);

            // create AddTargetBox
            #region AddTargetBox
            var addTargetBox = new FrameworkElementFactory(typeof(TextBoxWithCompletionOptions), "AddTargetBox");
            addTargetBox.SetBinding(TextBoxWithCompletionOptions.TagProperty, new Binding() { ElementName = "TargetListBox" });
            addTargetBox.SetBinding(TextBoxWithCompletionOptions.CompletionOptionsSourceProperty, new Binding()
            {
                // TODO: Bind this to autocomplete source somehow
            });
            rootGrid_row1.AppendChild(addTargetBox);
            #endregion AddTargetBox

            // add the row into the root grid
            rootGrid.AppendChild(rootGrid_row1);
            #endregion rootGrid_row1

            return new DataTemplate(typeof(ActionTargetSpecifier)) { VisualTree = rootGrid };
        }
    }

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
        [HotkeyActionSetting(Setting_TargetOverride_Name, typeof(ActionTargetSpecifier), typeof(ActionTargetSpecifierDataTemplateProvider), Description = Setting_TargetOverride_Description)]
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
