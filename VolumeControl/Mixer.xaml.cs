using AudioAPI.Interfaces;
using AudioAPI.Objects;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace VolumeControl
{
    /// <summary>
    /// Interaction logic for Mixer.xaml
    /// </summary>
    public partial class Mixer : Window
    {
        public Mixer()
        {
            StepSize = 2;
            InitializeComponent();
            processDataGrid.ItemsSource = audioAPI.Sessions;
        }

        private int StepSize { get; set; }
        private bool SettingsMenuIsOpen { get; set; }
        private IAudioSession CurrentlySelectedGridRow => (IAudioSession)processDataGrid.CurrentCell.Item;
        private IProcess CurrentSession => audioAPI.SelectedSession;

        private void Handle_OpenSettingsClick(object sender, RoutedEventArgs e) => SettingsMenuIsOpen = !SettingsMenuIsOpen;

        private void Handle_ReloadClick(object sender, RoutedEventArgs e) => audioAPI.RefreshSessions();

        private void Handle_ProcessSelectClick(object sender, RoutedEventArgs e)
        {
            if (CurrentlySelectedGridRow is AudioSession session)
                audioAPI.SelectedSession = session;
        }
        private void Handle_VolumeDownClick(object sender, RoutedEventArgs e)
            => CurrentlySelectedGridRow.Volume -= StepSize;
        private void Handle_VolumeUpClick(object sender, RoutedEventArgs e)
            => CurrentlySelectedGridRow.Volume += StepSize;
    }
}
