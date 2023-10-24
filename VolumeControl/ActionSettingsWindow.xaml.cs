using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;
using VolumeControl.ViewModels;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for ActionSettingsWindow.xaml
    /// </summary>
    public partial class ActionSettingsWindow : Window, INotifyPropertyChanged
    {
        #region Initializers
        public ActionSettingsWindow()
        {
            InitializeComponent();
        }
        public ActionSettingsWindow(Window owner, HotkeyVM hotkey) : this()
        {
            Owner = owner;
            Title = hotkey.Hotkey.Name;
            Hotkey = hotkey;
        }
        #endregion Initializers

        #region Events
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => this.RaisePropertyChanged(propertyName);
        #endregion Events

        #region Properties
        private VolumeControlVM VCSettings => (FindResource("Settings") as VolumeControlVM)!;
        public HotkeyVM? Hotkey
        {
            get => _hotkey;
            set
            {
                _hotkey = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ActionSettings));
            }
        }
        private HotkeyVM? _hotkey;
        public IActionSettingInstance[]? ActionSettings => this.Hotkey?.Hotkey.Action?.ActionSettings;
        #endregion Properties

        #region EventHandlers

        #region Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.ChangedButton.Equals(MouseButton.Left)) return;

            this.DragMove();
            e.Handled = true;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            VCSettings.HotkeyAPI.SaveHotkeys();
        }
        #endregion Window

        #region ListBoxRemoveButton
        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listBox = (ListBox)button.Tag;

            var setting = (IActionSettingInstance)listBox.DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            var index = listBox.ItemContainerGenerator.IndexFromContainer((ListBoxItem)button.DataContext);

            specifier.Targets.RemoveAt(index);
        }
        #endregion ListBoxRemoveButton

        #region AddTargetBox
        /// <summary>
        /// Adds the clicked suggestion to the list of target overrides.
        /// </summary>
        private void AddTargetBox_SuggestionClicked(object sender, WPF.Controls.TextBoxWithCompletionOptions.SuggestionClickedEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            var setting = (IActionSettingInstance)((ListBox)addTargetBox.Tag).DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            specifier.Targets.Add(e.SuggestionText);
        }
        /// <summary>
        /// Adds (and attempts to resolve) the committed text to the list of target overrides.
        /// </summary>
        private void AddTargetBox_CommittedText(object sender, WPF.Controls.TextBoxWithCompletionOptions.CommittedTextEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            if (addTargetBox.Text.Trim().Length == 0)
            {
                e.Handled = true;
                return;
            }

            var setting = (IActionSettingInstance)((ListBox)addTargetBox.Tag).DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            if (VCAPI.Default.AudioSessionManager.FindSessionWithProcessName(addTargetBox.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
            {
                specifier.Targets.Add(session.ProcessName);
            }
            else // add whatever text was entered
            {
                specifier.Targets.Add(addTargetBox.Text);
            }

            addTargetBox.Text = string.Empty;
        }
        /// <summary>
        /// Removes the last item in the list of target overrides.
        /// </summary>
        private void AddTargetBox_BackPressed(object sender, RoutedEventArgs e)
        {
            var addTargetBox = (WPF.Controls.TextBoxWithCompletionOptions)sender;
            var listBox = (ListBox)addTargetBox.Tag;

            if (listBox.Items.Count == 0) return;

            var setting = (IActionSettingInstance)listBox.DataContext;
            var specifier = (ActionTargetSpecifier)setting.Value!;

            specifier.Targets.RemoveAt(listBox.Items.Count - 1);//< remove the last item in the list
        }
        #endregion AddTargetBox

        #region ApplicationCommands
        /// <summary>
        /// Closes the window when ESC is pressed.
        /// </summary>
        private void ApplicationCommands_Close_Executed(object sender, ExecutedRoutedEventArgs e) => Close();
        #endregion ApplicationCommands

        #region CloseWindowButton
        /// <summary>
        /// Closes the window.
        /// </summary>
        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) => Close();
        #endregion CloseWindowButton

        #region ResetToDefaultButton
        private void ResetToDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (Hotkey == null || Hotkey.Hotkey.Action == null) return;

            var hk = Hotkey.Hotkey;
            var actionDefinition = hk.Action.HotkeyActionDefinition;

            for (int i = 0, max = hk.Action.ActionSettings.Length; i < max; ++i)
            {
                var currentSetting = hk.Action.ActionSettings[i];
                var defaultSetting = actionDefinition.GetActionSettingDefinition(currentSetting.Name)?.CreateInstance();

                if (defaultSetting == null) continue; //< skip settings that we can't get a default value for

                if (currentSetting.Value is ActionTargetSpecifier currentTargetSpecifier)
                {
                    currentTargetSpecifier.Targets.Clear();
                    if (defaultSetting.Value is ActionTargetSpecifier defaultTargetSpecifier && defaultTargetSpecifier.Targets.Count > 0)
                    { // this will probably never be triggered
                        currentTargetSpecifier.Targets.AddRange(defaultTargetSpecifier.Targets);
                    }
                }
                else
                {
                    currentSetting.Value = defaultSetting.Value;
                }
            }
        }
        #endregion ResetToDefaultButton

        #region DataTemplateErrorTextBlock
        private void DataTemplateErrorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;

            FLog.Critical($"No {nameof(DataTemplate)} was found for object type \"{textBlock.DataContext.GetType()}\"!");
        }
        #endregion DataTemplateErrorTextBlock

        #endregion EventHandlers
    }
}
