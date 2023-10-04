using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.CoreAudio;
using VolumeControl.WPF;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="CoreAudio.AudioDevice"/> class.
    /// </summary>
    public sealed class AudioDeviceVM : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        public AudioDeviceVM(AudioDevice audioDevice)
        {
            AudioDevice = audioDevice;

            Icon = IconExtractor.TryExtractFromPath(AudioDevice.IconPath, out ImageSource icon) ? icon : null;
            Sessions = new();

            // attach events to add and remove audio sessions from the Sessions list
            AudioDevice.SessionManager.SessionAddedToList += this.SessionManager_SessionAddedToList;
            AudioDevice.SessionManager.SessionRemovedFromList += this.SessionManager_SessionRemovedFromList;

            // initialize Sessions list
            foreach (var session in AudioDevice.SessionManager.Sessions)
            {
                Sessions.Add(new AudioSessionVM(session));
            }
        }
        #endregion Constructor

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Properties
        public AudioDevice AudioDevice { get; }
        public ImageSource? Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }
        private ImageSource? _icon = null;
        public string Name => AudioDevice.Name;
        public string DeviceFriendlyName => AudioDevice.FullName;
        public ObservableImmutableList<AudioSessionVM> Sessions { get; }
        #endregion Properties

        #region IDisposable Implementation
        public void Dispose()
        {
            this.AudioDevice.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation

        #region EventHandlers

        #region SessionManager
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
        #endregion SessionManager

        #endregion EventHandlers
    }
}
