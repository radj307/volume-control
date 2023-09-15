using CodingSeb.Localization;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VolumeControl.Core;
using VolumeControl.Core.Enum;
using VolumeControl.Core.Input;
using VolumeControl.Core.Interfaces;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.SDK;
using VolumeControl.ViewModels;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;
using VolumeControl.WPF.Controls;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for Mixer.xaml
    /// </summary>
    public partial class Mixer : Window
    {
        #region Setup
        public Mixer()
        {
            this.InitializeComponent();

            this.ShowInTaskbar = Settings.ShowInTaskbar;
            this.Topmost = Settings.AlwaysOnTop;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Settings.StartMinimized) this.Hide();

            // Initialize secondary windows so they can run their own code to appear:

            // SessionListNotification
            _ = WindowHandleGetter.GetWindowHandle((this.FindResource("SessionListNotification") as SessionListNotification)!);
        }
        #endregion Setup

        #region Properties
        //private ListNotification ListNotification => (this.FindResource("Notification") as ListNotification)!;
        private VolumeControlVM VCSettings => (this.FindResource("Settings") as VolumeControlVM)!;
        private AudioDeviceManagerVM AudioAPI => this.VCSettings.AudioAPI;
        private HotkeyManager HotkeyAPI => this.VCSettings.HotkeyAPI;
        private static LogWriter Log => FLog.Log;
        private static Config Settings => (Config.Default as Config)!;
        public static bool LogEnabled
        {
            get => Settings.EnableLogging;
            set => Settings.EnableLogging = value;
        }
        public static string LogFilePath
        {
            get => Settings.LogPath;
            set => Settings.LogPath = value;
        }
        #endregion Properties

        #region EventHandlers
        /// <summary>Handles the Select process button's click event.</summary>
        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (MixerGrid.CurrentCell.Item is AudioSessionVM session)
            {
                VCAPI.Default.AudioSessionSelector.Selected = session.AudioSession;
                //this.AudioAPI.SelectedSession = session;
            }
        }
        /// <summary>Handles the create new hotkey button's click event.</summary>
        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e) => this.HotkeyAPI.AddHotkey();

        /// <summary>Handles the remove hotkey button's click event.</summary>
        private void Handle_hotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button hotkeyGridRemove && hotkeyGridRemove.CommandParameter is int id)
            {
                if (!this.HotkeyAPI.Hotkeys.Any(hk => hk.ID.Equals(id)))
                    return;

                if (Settings.DeleteHotkeyConfirmation)
                {
                    switch (MessageBox.Show(
                        Loc.Tr("VolumeControl.Dialogs.RemoveHotkey.Message",
                        $"Are you sure you want to delete hotkey {id}?\n" +
                        $"\n" +
                        $"- 'Yes'     Delete the hotkey.\n" +
                        $"- 'No'      Do not delete the hotkey.\n" +
                        $"- 'Cancel'  Don't show this again. (Press again to delete)\n")
                        .Replace("${ID}", id.ToString()),
                        Loc.Tr("VolumeControl.Dialogs.RemoveHotkey.Caption", "Confirm Delete"),
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.No)
                        )
                    {
                    case MessageBoxResult.Yes:
                        break;// continue
                    case MessageBoxResult.No:
                        return;
                    case MessageBoxResult.Cancel:
                        Settings.DeleteHotkeyConfirmation = false;
                        Log.Info($"User specified option '{MessageBoxResult.Cancel:G}' in the remove hotkey confirmation dialog, indicating they want to disable the confirmation dialog; the '{nameof(Settings.DeleteHotkeyConfirmation)}' setting has been set to false.");
                        return;
                    }
                }

                Log.Debug($"User is removing hotkey {id} ({(this.HotkeyAPI.GetHotkey(id) as Hotkey)?.GetStringRepresentation()})");

                this.HotkeyAPI.DelHotkey(id);
            }
        }

        /// <inheritdoc cref="VolumeControlVM.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e)
        {
            this.VCSettings.ResetHotkeySettings();
        }

        private void Handle_BrowseForLogFilePathClick(object sender, RoutedEventArgs e)
        {
            string myDir = Path.GetDirectoryName(this.VCSettings.ExecutablePath) ?? string.Empty;
            if (Path.GetDirectoryName(LogFilePath) is not string initial || initial.Length == 0)
            {
                initial = myDir;
            }

            var sfd = new SaveFileDialog
            {
                OverwritePrompt = false,
                DefaultExt = "log",
                InitialDirectory = initial,
                Title = "Choose a location to save the log file.",
                FileName = LogFilePath
            };
            _ = sfd.ShowDialog(this);
            string path = Path.GetRelativePath(myDir, sfd.FileName);
            if (!path.Equals(logPath.Text, StringComparison.Ordinal))
            {
                logPath.Text = path;
            }
        }
        private void Handle_LogFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals("Value", StringComparison.Ordinal) ?? false)
            {
                Settings.LogFilter = (this.FindResource("EventTypeOptions") as BindableEventType)!.Value;
            }
        }
        private void Handle_TargetNameBoxDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            targetbox.SelectionStart = 0;
            targetbox.SelectionLength = targetbox.Text.Length;
        }
        private void Handle_ThreeStateCheckboxClick(object sender, RoutedEventArgs e)
        { // this prevents the user from being able to set the checkbox to indeterminate directly
            if (e.Source is CheckBox cb)
            {
                if (!cb.IsChecked.HasValue)
                    cb.IsChecked = false;
            }
        }
        private void Handle_MinimizeClick(object sender, RoutedEventArgs e) => this.Hide();
        private void Handle_MaximizeClick(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Maximized;
        private void Handle_CloseClick(object sender, RoutedEventArgs e) => App.Current.Shutdown();
        private void Handle_CheckForUpdatesClick(object sender, RoutedEventArgs e) => this.VCSettings.Updater.CheckForUpdateNow();
        private void Handle_LogFilterBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => logFilterBox.SelectedItem = null;
        private void Handle_KeySelectorKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Tab))
                return; //< don't capture the tab key or it breaks keyboard navigation
            if (!System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) && !System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl))
                return; //< only capture when CTRL is held down to allow proper searching
            if (sender is ComboBox cmb)
            {
                var eventKey = (EFriendlyKey)e.Key;
                int i = cmb.Items.IndexOf(eventKey);

                if (i.Equals(-1))
                    return;

                object? item = cmb.Items[i];

                if (this.TryFindResource("KeyOptions") is KeyOptions keys && keys.Contains(item))
                {
                    cmb.SelectedItem = eventKey;
                    e.Handled = true;
                }
            }
        }
        private void Handle_ReloadLanguageConfigs(object sender, RoutedEventArgs e) => LocalizationHelper.ReloadLanguageConfigs();

        private void Handle_HotkeyActionSettingsClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.CommandParameter is int hkid && HotkeyAPI.Hotkeys.FirstOrDefault(hk => hk.ID.Equals(hkid)) is IBindableHotkey hk)
            {
                ActionSettingsWindow settingsWindow = new(owner: this, hotkey: hk);
                settingsWindow.ShowDialog();
            }
        }
        private void Handle_ResetNotificationPositionClick(object sender, RoutedEventArgs e)
        {
            var appWindows = App.Current.Windows;

            var cp = this.GetPosAtCenterPoint();

            for (int i = 0; i < appWindows.Count; ++i)
            {
                if (appWindows[i] is SessionListNotification sessionListNotificationWindow)
                {
                    sessionListNotificationWindow.SetPosAtCenterPoint(cp);
                    VCAPI.Default.ShowSessionListNotification();
                }
            }
        }
        private void Handle_ForceHideNotificationClick(object sender, RoutedEventArgs e)
        {
            var appWindows = App.Current.Windows;

            for (int i = 0; i < appWindows.Count; ++i)
            {
                if (appWindows[i] is SessionListNotification sessionListNotificationWindow)
                {
                    sessionListNotificationWindow.Hide();
                }
            }
        }
        /// <summary>
        /// Update TextProperty source when the Enter key is pressed while a textbox is focused.
        /// </summary>
        private void Handle_TextBoxKeyUp_UpdateTextBindingSource(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;

                if (textBox.GetBindingExpression(TextBox.TextProperty) is BindingExpression binding)
                {
                    binding.UpdateSource();
                }
            }
        }
        private void HiddenSessionProcessNamesCompletionBox_CommittedText(object sender, TextBoxWithCompletionOptions.CommittedTextEventArgs e)
        {
            if (e.Text.Trim().Length == 0)
            { // don't commit empty text
                e.Handled = true;
                return;
            }

            if (AudioAPI.AudioSessionManager.FindSessionWithProcessName(e.Text, StringComparison.OrdinalIgnoreCase) is CoreAudio.AudioSession session)
            {
                Settings.HiddenSessionProcessNames.Add(session.ProcessName);
            }
            else
            {
                Settings.HiddenSessionProcessNames.Add(e.Text);
            }
            Settings.Save();

            // clear the text box:
            HiddenSessionProcessNamesCompletionBox.Text = string.Empty;
        }
        private void HiddenSessionProcessNamesCompletionBox_SuggestionClicked(object sender, TextBoxWithCompletionOptions.SuggestionClickedEventArgs e)
        {
            Settings.HiddenSessionProcessNames.Add(e.SuggestionText);
            Settings.Save();
        }
        private void HiddenSessionProcessNamesRemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listBoxItem = (ListBoxItem)button.DataContext;

            var index = HiddenSessionProcessNamesListBox.ItemContainerGenerator.IndexFromContainer(listBoxItem);
            Settings.HiddenSessionProcessNames.RemoveAt(index);
            Settings.Save();
        }
        private void HiddenSessionProcessNamesCompletionBox_BackPressed(object sender, RoutedEventArgs e)
        {
            if (Settings.HiddenSessionProcessNames.Count == 0) return;
            Settings.HiddenSessionProcessNames.RemoveAt(Settings.HiddenSessionProcessNames.Count - 1);
            Settings.Save();
        }
        #endregion EventHandlers
    }
}
