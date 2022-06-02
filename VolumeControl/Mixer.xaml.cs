using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Audio;
using VolumeControl.Core.Enum;
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
        #region Init
        public Mixer()
        {
            InitializeComponent();

            ShowInTaskbar = Settings.ShowInTaskbar;
            Topmost = Settings.AlwaysOnTop;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Settings.StartMinimized)
            {
                WindowState = WindowState.Minimized;
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            HotkeyAPI.Dispose();

            // Apply Window Settings:
            Settings.ShowInTaskbar = ShowInTaskbar;
            Settings.AlwaysOnTop = Topmost;
            // Save Window Settings:
            Settings.Save();
            Settings.Reload();
            // Save Log Settings
            LogSettings.Save();
            LogSettings.Reload();

            e.Cancel = false;
        }
        #endregion Init

        #region Properties
        private ListNotification ListNotification => (FindResource("Notification") as ListNotification)!;
        private VolumeControlSettings VCSettings => (FindResource("Settings") as VolumeControlSettings)!;
        private AudioAPI AudioAPI => VCSettings.AudioAPI;
        private HotkeyManager HotkeyAPI => VCSettings.HotkeyAPI;
        private static LogWriter Log => FLog.Log;
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static Log.Properties.Settings LogSettings => VolumeControl.Log.Properties.Settings.Default;
        public static bool LogEnabled
        {
            get => LogSettings.EnableLogging;
            set => LogSettings.EnableLogging = value;
        }
        public static string LogFilePath
        {
            get => LogSettings.LogPath;
            set => LogSettings.LogPath = value;
        }
        #endregion Properties

        #region EventHandlers
        /// <summary>Handles the reload session list button's click event.</summary>
        private void Handle_ReloadDevicesClick(object sender, RoutedEventArgs e)
        {
            AudioAPI.ForceReloadAudioDevices();
        }
        /// <summary>Handles the reload session list button's click event.</summary>
        private void Handle_ReloadSessionsClick(object sender, RoutedEventArgs e)
        {
            AudioAPI.ForceReloadSessionList();
        }

        /// <summary>Handles the Select process button's click event.</summary>
        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (MixerGrid.CurrentCell.Item is AudioSession session)
            {
                AudioAPI.SelectedSession = session;
            }
        }
        /// <summary>Handles the create new hotkey button's click event.</summary>
        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e)
        {
            HotkeyAPI.AddHotkey();
        }

        /// <summary>Handles the remove hotkey button's click event.</summary>
        private void Handle_HotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.CommandParameter is int id)
            {
                HotkeyAPI.DelHotkey(id);
            }
        }
        /// <summary>Ensures there is enough space to display the hotkeys data grid when advanced mode is enabled.</summary>
        //private void Handle_TabControlChange(object sender, RoutedEventArgs e)
        //{
        //    if (sender is TabControl tc && tc.SelectedValue is TabItem ti)
        //    {
        //        if (ti.Equals(HotkeysTab) && VCSettings.AdvancedHotkeyMode)
        //            MaxWidth = Settings.WindowWidthWide;
        //        else if (MaxWidth != Settings.WindowWidthDefault)
        //            MaxWidth = Settings.WindowWidthDefault;
        //    }
        //}
        /// <inheritdoc cref="VolumeControlSettings.ResetHotkeySettings"/>
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e) => VCSettings.ResetHotkeySettings();

        private void Handle_BrowseForLogFilePathClick(object sender, RoutedEventArgs e)
        {
            string myDir = Path.GetDirectoryName(VCSettings.ExecutablePath) ?? string.Empty;
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
            sfd.ShowDialog(this);
            string path = Path.GetRelativePath(myDir, sfd.FileName);
            if (!path.Equals(logPath.Text, StringComparison.Ordinal))
            {
                logPath.Text = path;
            }
        }
        private void Handle_OpenGithubClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(Settings.UpdateURL)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't open '{Settings.UpdateURL}' because of an exception:\n'{ex.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
        }
        private void Handle_LogFilterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals("Value", StringComparison.Ordinal) ?? false)
            {
                LogSettings.LogAllowedEventTypeFlag = (uint)(FindResource("EventTypeOptions") as BindableEventType)!.Value;
            }
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState.Equals(WindowState.Minimized))
            {
                Hide();
                ShowInTaskbar = true;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListNotification? lnotif = ListNotification;
            lnotif.Owner = this;

            AudioAPI.SelectedSessionSwitched += (s, e) => ListNotification.HandleShow(DisplayTarget.Sessions);
            AudioAPI.LockSelectedSessionChanged += (s, e) => ListNotification.HandleShow(DisplayTarget.Sessions);
            AudioAPI.SelectedSessionVolumeChanged += (s, e) => ListNotification.HandleShow(DisplayTarget.Sessions, false);

            Log.Debug($"Finished binding event handler method '{nameof(ListNotification.HandleShow)}' to {AudioAPI} events.");
        }
        private void Handle_TargetNameBoxDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            targetbox.SelectionStart = 0;
            targetbox.SelectionLength = targetbox.Text.Length;
        }
        private void Handle_ThreeStateCheckboxClick(object sender, RoutedEventArgs e)
        { // this prevents the user from being able to set the checkbox to indeterminate directly
            if (e.Source is CheckBox cb)
                if (!cb.IsChecked.HasValue)
                    cb.IsChecked = false;
        }
        private void Handle_MinimizeClick(object sender, RoutedEventArgs e) => Hide();
        private void Handle_MaximizeClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Maximized;
        private void Handle_CloseClick(object sender, RoutedEventArgs e) => Close();
        private void Handle_CheckForUpdatesClick(object sender, RoutedEventArgs e) => VCSettings.Updater.Update();
        private void Handle_CaptionUpdateClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (VCSettings.Updater.ShowUpdatePrompt())
                VCSettings.Updater.Update(true);
        }
        #endregion EventHandlers
    }
}
