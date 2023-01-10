using CodingSeb.Localization;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.Core.Interfaces;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

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
            // Initialize the list notification window so it can appear
            _ = WindowHandleGetter.GetWindowHandle((this.FindResource("Notification") as ListNotification)!);
#if DEBUG
            //(FindResource("DebugWindow") as DebugWindow)!.Show();
#endif
        }
        #endregion Setup

        #region Teardown
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.HotkeyAPI.Dispose();
            e.Cancel = false;
        }
        #endregion Teardown

        #region Properties
        private ListNotification ListNotification => (this.FindResource("Notification") as ListNotification)!;
        private VolumeControlSettings VCSettings => (this.FindResource("Settings") as VolumeControlSettings)!;
        private AudioAPI AudioAPI => this.VCSettings.AudioAPI;
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
        /// <summary>Handles the reload session list button's click event.</summary>
        private void Handle_ReloadDevicesClick(object sender, RoutedEventArgs e) => this.AudioAPI.ForceReloadAudioDevices();
        /// <summary>Handles the reload session list button's click event.</summary>
        private void Handle_ReloadSessionsClick(object sender, RoutedEventArgs e) => this.AudioAPI.ForceReloadSessionList();

        /// <summary>Handles the Select process button's click event.</summary>
        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (MixerGrid.CurrentCell.Item is AudioSession session)
            {
                this.AudioAPI.SelectedSession = session;
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
                        $"- 'Cancel'  Disable these confirmation prompts in the future. (Does not delete the hotkey)\n")
                        .Replace("${ID}", id.ToString()),
                        Loc.Tr("VolumeControl.Dialogs.RemoveHotkey.Caption", "Confirm Remove"),
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

        /// <inheritdoc cref="VolumeControlSettings.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e)
        {
            if (!Settings.AdvancedHotkeys)
            {

            }
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
        private void Handle_CheckForUpdatesClick(object sender, RoutedEventArgs e) => this.VCSettings.Updater.CheckNow();
        private void Handle_LogFilterBoxSelectionChanged(object sender, SelectionChangedEventArgs e) => logFilterBox.SelectedItem = null;
        private void Handle_KeySelectorKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.Equals(System.Windows.Input.Key.Tab))
                return; //< don't capture the tab key or it breaks keyboard navigation
            if (sender is ComboBox cmb)
            {
                int i = cmb.Items.IndexOf(e.Key);

                if (i.Equals(-1))
                    return;

                object? item = cmb.Items[i];

                if (this.TryFindResource("KeyOptions") is KeyOptions keys && keys.Contains(item))
                {
                    cmb.SelectedItem = e.Key;
                    e.Handled = true;
                }
            }
        }
        private void Handle_ReloadLanguageConfigs(object sender, RoutedEventArgs e) => LocalizationHelper.ReloadLanguageConfigs();

        private void Handle_HotkeyActionSettingsClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.CommandParameter is int hkid && HotkeyAPI.Hotkeys.FirstOrDefault(hk => hk.ID.Equals(hkid)) is IBindableHotkey hk)
            {
                ActionSettingsWindow settingsWindow = new();
                settingsWindow.VM.Hotkey = hk;
                settingsWindow.VM.WindowTitle = hk.Name;
                settingsWindow.Owner = this;
                settingsWindow.ShowDialog();
            }
        }
        private void Handle_ResetNotificationPositionClick(object sender, RoutedEventArgs e)
        {
            ListNotification.Left = this.Left;
            ListNotification.Top = this.Top;
            ListNotification.Show();
        }
        private void Handle_DisplayTargetsHyperlinkClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A display target is a collection of triggers, controls, colors, & logic that determine the behaviour and appearance of the notification window.\nDisplay targets also change when the notification window appears. For example, the Audio Sessions display target shows the notification window when the selected session is changed or (un)locked, or when the session's volume or mute state changes.", "Help", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
        #endregion EventHandlers
    }
}
