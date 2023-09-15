using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Interfaces;
using VolumeControl.SDK;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

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
        public ActionSettingsWindow(Window owner, IBindableHotkey hotkey) : this()
        {
            Owner = owner;
            Title = hotkey.Name;
            Hotkey = hotkey;
        }
        #endregion Initializers

        #region Events
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => this.RaisePropertyChanged(propertyName);
        #endregion Events

        #region Fields
        private int _lastSelectedSuggestionIndex = 0;
        #endregion Fields

        #region Properties
        public IBindableHotkey? Hotkey
        {
            get => _hotkey;
            set
            {
                _hotkey = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ActionSettings));
            }
        }
        private IBindableHotkey? _hotkey;
        public ObservableImmutableList<IHotkeyActionSetting>? ActionSettings => this.Hotkey?.ActionSettings;
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
            VCAPI.Default.Settings.Save();
        }
        #endregion Window

        #region ListBoxRemoveButton
        private void ListBoxRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listBox = (ListBox)button.Tag;

            var setting = (HotkeyActionSetting)listBox.DataContext;

            if (setting?.Value is not ActionTargetSpecifier list) return;

            var index = listBox.ItemContainerGenerator.IndexFromContainer((ListBoxItem)button.DataContext);

            list.Targets.RemoveAt(index);
        }
        #endregion ListBoxRemoveButton

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

        #region AddTargetBox
        /// <summary>
        /// Adds the clicked suggestion to the list of target overrides.
        /// </summary>
        private void AddTargetBox_SuggestionClicked(object sender, WPF.Controls.TextBoxWithCompletionOptions.SuggestionClickedEventArgs e)
        {
            var box = (WPF.Controls.TextBoxWithCompletionOptions)sender;
            var listBox = (ListBox)box.Tag;

            if (((HotkeyActionSetting)listBox.DataContext)?.Value is not ActionTargetSpecifier list) return;

            list.Targets.Add(new() { Value = e.SuggestionText });
        }
        /// <summary>
        /// Adds (and attempts to resolve) the committed text to the list of target overrides.
        /// </summary>
        private void AddTargetBox_CommittedText(object sender, WPF.Controls.TextBoxWithCompletionOptions.CommittedTextEventArgs e)
        {
            var box = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            if (box.Text.Trim().Length == 0)
            {
                e.Handled = true;
                return;
            }

            var listBox = (ListBox)box.Tag;

            if (((HotkeyActionSetting)listBox.DataContext)?.Value is not ActionTargetSpecifier list) return;

            if (VCAPI.Default.AudioSessionManager.FindSessionWithProcessName(box.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
            {
                list.Targets.Add(new() { Value = session.ProcessName });
            }
            else
            {
                list.Targets.Add(new() { Value = box.Text });
            }

            box.Text = string.Empty;
        }
        /// <summary>
        /// Removes the last item in the list of target overrides.
        /// </summary>
        private void AddTargetBox_BackPressed(object sender, RoutedEventArgs e)
        {
            var box = (WPF.Controls.TextBoxWithCompletionOptions)sender;

            var listBox = (ListBox)box.Tag;

            if (listBox.Items.Count == 0) return;

            if (((HotkeyActionSetting)listBox.DataContext)?.Value is not ActionTargetSpecifier list) return;

            list.Targets.RemoveAt(listBox.Items.Count - 1);//< remove the last item in the list
        }
        #endregion AddTargetBox

        #endregion EventHandlers
    }
}
