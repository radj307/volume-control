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
            processDataGrid.ItemsSource = AudioAPI.Sessions;
        }

        private Core.AudioAPI AudioAPI => (Resources["AudioAPI"] as Core.AudioAPI)!;

        private IAudioSession CurrentlySelectedGridRow => (IAudioSession)processDataGrid.CurrentCell.Item;

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
    }
}
