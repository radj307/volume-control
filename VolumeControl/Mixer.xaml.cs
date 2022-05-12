using AudioAPI.Interfaces;
using AudioAPI.Objects;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
            
            versionLabel.Content = $"v{Assembly.GetExecutingAssembly().GetCustomAttribute<Core.Attributes.ExtendedVersion>()?.Version}";
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Apply Window Settings:
            Settings.AdvancedHotkeys = cbAdvancedHotkeys.IsChecked ?? Settings.AdvancedHotkeys;
            // Save Window Settings:
            Settings.Save();
            Settings.Reload();
            // Save CoreSettings:
            AudioAPI.SaveSettings();
            HotkeyAPI.SaveHotkeys();
            HotkeyAPI.RemoveHook();
            e.Cancel = false;
        }
        #endregion Init

        #region Properties
        private Core.AudioAPI AudioAPI => (Resources["AudioAPI"] as Core.AudioAPI)!;
        private Core.HotkeyManager HotkeyAPI => (Resources["HotkeyAPI"] as Core.HotkeyManager)!;
        private static Properties.Settings Settings => Properties.Settings.Default;

        private IAudioSession CurrentlySelectedGridRow => (IAudioSession)MixerGrid.CurrentCell.Item;
        #endregion Properties

        #region EventHandlers
        private void Handle_ReloadClick(object sender, RoutedEventArgs e) => AudioAPI.ReloadSessionList();
        private void Handle_ReloadDevicesClick(object sender, RoutedEventArgs e) => AudioAPI.ReloadDeviceList();

        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedGridRow is AudioSession session)
                AudioAPI.SelectedSession = session;
        }

        private void Handle_ApplyClick(object sender, RoutedEventArgs e)
        {
            BindingExpression bindingExpr = targetNameTextBox.GetBindingExpression(TextBox.TextProperty);
            bindingExpr.UpdateSource();
        }

        private void Handle_ShowActionBindingsChecked(object sender, RoutedEventArgs e) => Resources["HotkeyActionBindingVisibility"] = Visibility.Visible;
        private void Handle_ShowActionBindingsUnchecked(object sender, RoutedEventArgs e) => Resources["HotkeyActionBindingVisibility"] = Visibility.Collapsed;

        private void Handle_CreateNewHotkeyClick(object sender, RoutedEventArgs e) => HotkeyAPI.AddHotkey();
        private void Handle_HotkeyGridSourceUpdate(object sender, RoutedEventArgs e)
        {
        }
        private void Handle_HotkeyGridRemoveClick(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int id)
            {
                HotkeyAPI.DelHotkey(id);
            }
        }
        private void Handle_TabControlChange(object sender, RoutedEventArgs e)
        {
            if (cbAdvancedHotkeys.IsChecked.GetValueOrDefault(false) && sender is TabControl tc && tc.SelectedValue is TabItem ti)
            {

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
        #endregion EventHandlers
    }
}
