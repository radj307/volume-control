using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Audio;
using VolumeControl.Core;
using VolumeControl.Helpers;
using VolumeControl.Hotkeys;
using VolumeControl.Log;
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
        private void Handle_HotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is int id)
            {
                this.HotkeyAPI.DelHotkey(id);
            }
        }
        /// <inheritdoc cref="VolumeControlSettings.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e) => this.VCSettings.ResetHotkeySettings();

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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListNotification? lnotif = this.ListNotification;
            lnotif.Owner = this;

            //this.AudioAPI.SelectedSessionSwitched += (s, e) => this.ListNotification.HandleShow(DisplayTarget.Sessions);
            //this.AudioAPI.LockSelectedSessionChanged += (s, e) => this.ListNotification.HandleShow(DisplayTarget.Sessions);
            //this.AudioAPI.SelectedSessionVolumeChanged += (s, e) => this.ListNotification.HandleShow(DisplayTarget.Sessions, false);

            //Log.Debug($"Finished binding event handler method '{nameof(ListNotification.HandleShow)}' to {this.AudioAPI} events.");
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
        private void Handle_CloseClick(object sender, RoutedEventArgs e) => this.Close();
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
        #endregion EventHandlers
    }
}
