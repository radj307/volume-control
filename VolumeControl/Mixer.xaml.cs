﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VolumeControl.Core;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.Interfaces;
using VolumeControl.Helpers;
using VolumeControl.Log;
using VolumeControl.Win32;
using VolumeControl.WPF;

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
            cbAdvancedHotkeys.IsChecked = Settings.AdvancedHotkeys;

            var assembly = Assembly.GetAssembly(typeof(Mixer));
            string version = $"v{assembly?.GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";
            versionLabel.Content = version;
            _startupHelper = new();

            var appDomain = AppDomain.CurrentDomain;
            _startupHelper.ExecutablePath = Path.Combine(appDomain.RelativeSearchPath ?? appDomain.BaseDirectory, Path.ChangeExtension(appDomain.FriendlyName, ".exe"));

            ShowInTaskbar = Settings.ShowInTaskbar;
            Topmost = Settings.AlwaysOnTop;
            cbStartMinimized.IsChecked = Settings.StartMinimized;
            if (Settings.RunAtStartup && !_startupHelper.RunAtStartup)
                _startupHelper.RunAtStartup = true;
            cbRunAtStartup.IsChecked = _startupHelper.RunAtStartup;
            cbShowIcons.IsChecked = Settings.ShowIcons;
            cbEnableNotifications.IsChecked = Settings.NotificationEnabled;

            (logFilterComboBox.ItemsSource as BindableEventType)!.Value = FLog.EventFilter;

            // Tray icon
            _notifyIcon = new();
            _notifyIcon.Icon = Properties.Resources.iconSilvered;
            _notifyIcon.Visible = true;
            _notifyIcon.Text = $"Volume Control {version}";
            _notifyIcon.Click += (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Settings.StartMinimized)
                WindowState = WindowState.Minimized;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _notifyIcon.Dispose();

            HotkeyAPI.Dispose();

            // Apply Window Settings:
            Settings.AdvancedHotkeys = cbAdvancedHotkeys.IsChecked ?? Settings.AdvancedHotkeys;
            Settings.ShowInTaskbar = ShowInTaskbar;
            Settings.AlwaysOnTop = Topmost;
            Settings.StartMinimized = cbStartMinimized.IsChecked.GetValueOrDefault(false);
            Settings.RunAtStartup = cbRunAtStartup.IsChecked.GetValueOrDefault(false);
            Settings.ShowIcons = cbShowIcons.IsChecked ?? Settings.ShowIcons;
            // Save Window Settings:
            Settings.Save();
            Settings.Reload();
            // Save Log Settings
            LogSettings.Save();
            LogSettings.Reload();

            e.Cancel = false;
        }
        #endregion Init

        #region Fields
        private readonly VolumeControlRunAtStartup _startupHelper;
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon = null!;
        #endregion Fields

        #region Properties
        private ListNotification ListNotification => (FindResource("Notification") as ListNotification)!;
        private VolumeControlSettings VCSettings => (FindResource("Settings") as VolumeControlSettings)!;
        private AudioAPI AudioAPI => VCSettings.AudioAPI;
        private HotkeyManager HotkeyAPI => VCSettings.HotkeyAPI;
        private static LogWriter Log => FLog.Log;
        private static Properties.Settings Settings => Properties.Settings.Default;
        private static CoreSettings CoreSettings => CoreSettings.Default;
        private static Log.Properties.Settings LogSettings => VolumeControl.Log.Properties.Settings.Default;
        private ISession CurrentlySelectedGridRow => (ISession)MixerGrid.CurrentCell.Item;
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
        private void Handle_ReloadClick(object sender, RoutedEventArgs e)
            => AudioAPI.ReloadSessionList();
        private void Handle_ReloadDevicesClick(object sender, EventArgs e)
            => AudioAPI.ReloadDeviceList();
        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedGridRow is AudioSession session)
            {
                AudioAPI.SelectedSession = session;
            }
        }
        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e)
            => HotkeyAPI.AddHotkey();
        private void Handle_HotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int id)
            {
                HotkeyAPI.DelHotkey(id);
            }
        }
        private void Handle_TabControlChange(object sender, RoutedEventArgs e)
        {
            if (sender is TabControl tc && tc.SelectedValue is TabItem ti)
            {
                if (ti.Equals(HotkeysTab))
                {
                    if (cbAdvancedHotkeys != null && cbAdvancedHotkeys.IsChecked.GetValueOrDefault(false))
                    {
                        MaxWidth = Settings.WindowWidthWide;
                        return;
                    }
                }
                MaxWidth = Settings.WindowWidthDefault;
            }
        }
        private void Handle_RunAtStartupChecked(object sender, RoutedEventArgs e) => _startupHelper.RunAtStartup = true;
        private void Handle_RunAtStartupUnchecked(object sender, RoutedEventArgs e) => _startupHelper.RunAtStartup = false;
        private void Handle_StartMinimizeChecked(object sender, RoutedEventArgs e) => Settings.StartMinimized = true;
        private void Handle_StartMinimizeUnchecked(object sender, RoutedEventArgs e) => Settings.StartMinimized = false;
        private void Handle_ResetSettingsClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Reset settings button was clicked, displaying confirmation dialog.");
            if (MessageBox.Show("Are you sure you want to reset your settings?\n\nThis cannot be undone!", "Reset Settings?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No).Equals(MessageBoxResult.Yes))
            {
                Settings.Reset();
                CoreSettings.Reset();
                LogSettings.Reset();

                Settings.Save();
                CoreSettings.Save();
                LogSettings.Save();

                Settings.Reload();
                CoreSettings.Reload();
                LogSettings.Reload();

                Log.Info("User settings were reset to default.");
            }
        }
        private void Handle_ResetHotkeysClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset your hotkeys to their default values?\n\nThis cannot be undone!", "Reset Hotkeys?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                HotkeyAPI.ResetHotkeys();

                Log.Info("Hotkey definitions were reset to default.");
            }
        }
        private void Handle_BrowseForLogFilePathClick(object sender, RoutedEventArgs e)
        {
            string myDir = Path.GetDirectoryName(_startupHelper.ExecutablePath) ?? string.Empty;
            if (Path.GetDirectoryName(LogFilePath) is not string initial || initial.Length == 0)
                initial = myDir;

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
                LogSettings.LogAllowedEventTypeFlag = (uint)(FindResource("EventTypeOptions") as BindableEventType)!.Value;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState.Equals(WindowState.Minimized))
                Hide();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var lnotif = ListNotification;
            lnotif.Owner = this;

            AudioAPI.SelectedSessionSwitched += (s, e) => ListNotification.Show();
            AudioAPI.LockSelectedSessionChanged += (s, e) => ListNotification.Show();
        }
        #endregion EventHandlers
    }
}
