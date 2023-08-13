using Audio;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="Audio.AudioDevice"/> class.
    /// </summary>
    public class AudioDeviceVM : INotifyPropertyChanged, IDisposable
    {
        public AudioDeviceVM(AudioDevice audioDevice)
        {
            AudioDevice = audioDevice;

            IconPair = GetIconPair();

            Sessions = new();

            AudioDevice.SessionManager.SessionAddedToList += this.SessionManager_SessionAddedToList;
            AudioDevice.SessionManager.SessionRemovedFromList += this.SessionManager_SessionRemovedFromList;

            foreach (var session in AudioDevice.SessionManager.Sessions)
            { // initialize SelectedDeviceSessions list:
                Sessions.Add(new AudioSessionVM(session));
            }
        }

        private void SessionManager_SessionAddedToList(object? sender, AudioSession e)
        {
            Sessions.Add(new AudioSessionVM(e));
        }
        private void SessionManager_SessionRemovedFromList(object? sender, AudioSession e)
        {
            var vm = Sessions.First(svm => svm.AudioSession.Equals(e));
            Sessions.Remove(vm);
            vm.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        public AudioDevice AudioDevice { get; }
        private IconPair IconPair
        {
            get => _iconPair;
            set
            {
                _iconPair = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Icon));
            }
        }
        private IconPair _iconPair = null!;
        public ImageSource? Icon => IconPair.GetBestFitIcon(preferLarge: false);
        public string Name => AudioDevice.Name;
        public string DeviceFriendlyName => AudioDevice.MMDevice.DeviceFriendlyName;
        public ObservableImmutableList<AudioSessionVM> Sessions { get; }

        private IconPair GetIconPair()
        {
            try
            {
                return IconGetter.GetIcons(AudioDevice.MMDevice.IconPath);
            }
            catch (Exception)
            {
                return new();
            }
        }
        public void Dispose()
        {
            this.AudioDevice.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
