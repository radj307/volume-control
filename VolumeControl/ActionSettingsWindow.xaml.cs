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

            list.Targets.Remove(button.DataContext);
        }
        #endregion ListBoxRemoveButton

        #region ApplicationCommands
        /// <summary>
        /// Closes the window.
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
        /// Creates a new target override entry with the text in the textbox when the Enter key is pressed.
        /// </summary>
        private void AddTargetTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                if (textBox.Text.Trim().Length == 0) return; //< if the text is blank, don't add it to the list

                var listBox = (ListBox)textBox.Tag;
                var setting = (HotkeyActionSetting)listBox.DataContext;

                if (setting?.Value is not ActionTargetSpecifier list) return;

                if (VCAPI.Default.AudioSessionManager.FindSessionWithProcessName(textBox.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
                {
                    list.Targets.Add(new() { Value = session.ProcessName });
                }
                else
                {
                    list.Targets.Add(new() { Value = textBox.Text });
                }

                textBox.Text = string.Empty;
            }
            else if (e.Key == Key.Back)// backspace
            {
                var textBox = (TextBox)sender;
                if (textBox.Text.Length != 0) return;

                var listBox = (ListBox)textBox.Tag;
                if (listBox.Items.Count == 0) return;
                var setting = (HotkeyActionSetting)listBox.DataContext;

                if (setting?.Value is not ActionTargetSpecifier list) return;

                list.Targets.Remove(listBox.Items[listBox.Items.Count - 1]);
            }
        }
        #endregion AddTargetBox

        #endregion EventHandlers
    }
}
