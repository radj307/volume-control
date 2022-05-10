using AudioAPI.Interfaces;
using AudioAPI.Objects;
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
        #region Constructors
        public Mixer() => InitializeComponent();
        #endregion Constructors

        #region Properties
        private Core.AudioAPI AudioAPI => (Resources["AudioAPI"] as Core.AudioAPI)!;
        private Core.HotkeyManager HotkeyAPI => (Resources["HotkeyAPI"] as Core.HotkeyManager)!;

        private IAudioSession CurrentlySelectedGridRow => (IAudioSession)MixerGrid.CurrentCell.Item;
        #endregion Properties

        #region WindowEvents
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AudioAPI.SaveSettings();
            HotkeyAPI.SaveHotkeys();
            HotkeyAPI.RemoveHook();
            e.Cancel = false;
        }
        #endregion WindowEvents

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
        #endregion EventHandlers
    }
}
