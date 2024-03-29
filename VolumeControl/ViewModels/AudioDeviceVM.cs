﻿using radj307.IconExtractor;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.CoreAudio;

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
            Icon?.Freeze(); //< prevents WPF exceptions in some cases
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
        #endregion Properties

        #region IDisposable Implementation
        ~AudioDeviceVM() => Dispose();
        public void Dispose()
        {
            this.AudioDevice.Dispose();
            _icon = null;
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
