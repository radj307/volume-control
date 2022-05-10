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
        public Mixer()
        {
            InitializeComponent();
            MixerGrid.ItemsSource = AudioAPI.Sessions;
            HotkeyGrid.ItemsSource = HotkeyAPI.Hotkeys;
            DeviceSelectorBox.ItemsSource = AudioAPI.Devices;

            //targetNameTextBox.SourceUpdated += (s, e) =>
            //{
            //    if (e.Property.Name.Equals("Content", System.StringComparison.Ordinal))
            //    {
            //        int cursorLen = targetNameTextBox.SelectionLength;
            //        int cursorPos = targetNameTextBox.SelectionStart;

            //        if (cursorLen > 0)
            //        {

            //        }
            //        else
            //        {

            //        }

            //        e.Handled = true;
            //    }
            //};
        }

        private Core.AudioAPI AudioAPI => (Resources["AudioAPI"] as Core.AudioAPI)!;
        private Core.HotkeyManager HotkeyAPI => (Resources["HotkeyAPI"] as Core.HotkeyManager)!;

        private IAudioSession CurrentlySelectedGridRow => (IAudioSession)MixerGrid.CurrentCell.Item;

        private void Handle_ReloadClick(object sender, RoutedEventArgs e) => AudioAPI.RefreshSessions();

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

        private void Handle_HKCancelClick(object sender, RoutedEventArgs e)
        {

        }
        private void Handle_HKApplyClick(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
