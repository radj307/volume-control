using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VolumeControl.Core;
using VolumeControl.Core.HelperTypes;
using VolumeControl.Core.Interfaces;
using VolumeControl.Log;
using VolumeControl.Win32;

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

            var assembly = Assembly.GetExecutingAssembly();
            versionLabel.Content = $"v{assembly.GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";
            _startupHelper.ExecutablePath = assembly.Location;

            ShowInTaskbar = Settings.ShowInTaskbar;
            Topmost = Settings.AlwaysOnTop;
            cbStartMinimized.IsChecked = Settings.StartMinimized;
            if (Settings.RunAtStartup && !_startupHelper.RunAtStartup)
                _startupHelper.RunAtStartup = true;
            cbRunAtStartup.IsChecked = _startupHelper.RunAtStartup;

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Icon = null;
            trayIcon.Dispose();

            // Apply Window Settings:
            Settings.AdvancedHotkeys = cbAdvancedHotkeys.IsChecked ?? Settings.AdvancedHotkeys;
            Settings.ShowInTaskbar = ShowInTaskbar;
            Settings.AlwaysOnTop = Topmost;
            Settings.StartMinimized = cbStartMinimized.IsChecked.GetValueOrDefault(false);
            Settings.RunAtStartup = cbRunAtStartup.IsChecked.GetValueOrDefault(false);
            // Save Window Settings:
            Settings.Save();
            Settings.Reload();
            // Save CoreSettings:
            AudioAPI.SaveSettings();
            HotkeyAPI.SaveHotkeys();
            HotkeyAPI.RemoveHook();
            e.Cancel = false;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            trayIcon = new();
            trayIcon.Text = $"Volume Control v{Assembly.GetExecutingAssembly().GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";
            trayIcon.Click += Handle_TrayIconClick;
            trayIcon.DoubleClick += Handle_TrayIconClick;
            trayIcon.Icon = Properties.Resources.iconSilvered;

            if (Settings.StartMinimized)
                WindowState = WindowState.Minimized;
        }
        private void Window_Loaded(object sender, EventArgs e)
        {
            trayIcon.Visible = true;
        }
        #endregion Init

        #region Fields
        private System.Windows.Forms.NotifyIcon trayIcon = null!;
        private readonly VolumeControlRunAtStartup _startupHelper = new();
        #endregion Fields

        #region Properties
        private Core.AudioAPI AudioAPI => (FindResource("AudioAPI") as AudioAPI)!;
        private Core.HotkeyManager HotkeyAPI => (FindResource("HotkeyAPI") as HotkeyManager)!;
        private LogWriter Log => FLog.Log;
        private static Properties.Settings Settings => Properties.Settings.Default;
        private ISession CurrentlySelectedGridRow => (ISession)MixerGrid.CurrentCell.Item;
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
        private void Handle_ShowActionBindingsChecked(object sender, RoutedEventArgs e)
            => Resources["HotkeyActionBindingVisibility"] = Visibility.Visible;
        private void Handle_ShowActionBindingsUnchecked(object sender, RoutedEventArgs e)
            => Resources["HotkeyActionBindingVisibility"] = Visibility.Collapsed;
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
            if (cbAdvancedHotkeys != null && cbAdvancedHotkeys.IsChecked.GetValueOrDefault(false) && sender is TabControl tc && tc.SelectedValue is TabItem ti)
            { // if we're switching to the hotkey tab & advanced hotkeys mode is enabled, widen the window
                if (ti.Equals(HotkeysTab))
                {
                    Width = Settings.WindowWidthWide;
                }
                else
                {
                    Width = Settings.WindowWidthDefault;
                }
            }
        }
        private void Handle_TrayIconClick(object? sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
            Focus();
        }
        private void Handle_RunAtStartupChecked(object sender, RoutedEventArgs e)
        {
            _startupHelper.RunAtStartup = true;
        }
        private void Handle_RunAtStartupUnchecked(object sender, RoutedEventArgs e)
        {
            _startupHelper.RunAtStartup = false;
        }
        private void Handle_StartMinimizeChecked(object sender, RoutedEventArgs e)
            => Settings.StartMinimized = true;
        private void Handle_StartMinimizeUnchecked(object sender, RoutedEventArgs e)
            => Settings.StartMinimized = false;
        private void Handle_ResetSettingsClick(object sender, RoutedEventArgs e)
        {
            Log.Info("Reset settings button was clicked, displaying confirmation dialog.");
            if (MessageBox.Show("Are you sure you want to reset your settings?\n\nThis cannot be undone!", "Reset Settings?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No).Equals(MessageBoxResult.Yes))
            {
                Settings.Reset();
                Settings.Save();
                Settings.Reload();
                Log.Info("User settings were reset to default.");
            }
        }
        #endregion EventHandlers
    }
}
