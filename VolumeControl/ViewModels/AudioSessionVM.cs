using radj307.IconExtractor;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.CoreAudio;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="CoreAudio.AudioSession"/> class.
    /// </summary>
    public sealed class AudioSessionVM : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        public AudioSessionVM(AudioDeviceManagerVM manager, AudioSession audioSession)
        {
            AudioDeviceManagerVM = manager;
            AudioSession = audioSession;

            Icon = GetIcon();

            AudioSession.IconPathChanged += (s, e) => Icon = GetIcon();

            // update bindings on the IsSelected property when this session is selected or deselected
            manager.AudioSessionMultiSelector.SessionSelected += this.AudioSessionMultiSelector_SessionSelected_SessionDeselected;
            manager.AudioSessionMultiSelector.SessionDeselected += this.AudioSessionMultiSelector_SessionSelected_SessionDeselected;
        }
        #endregion Constructor

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Properties
        AudioDeviceManagerVM AudioDeviceManagerVM { get; }
        public AudioSession AudioSession { get; }
        public ImageSource? Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }
        private ImageSource? _icon;
        public uint PID => AudioSession.PID;
        public string ProcessName => AudioSession.ProcessName;
        public string ProcessIdentifier => AudioSession.ProcessIdentifier;
        public string Name => AudioSession.Name;
        public bool IsSelected
        {
            get => AudioDeviceManagerVM.AudioSessionMultiSelector.GetSessionIsSelected(this.AudioSession);
            set
            {
                _isSelectedChanging = true;
                AudioDeviceManagerVM.AudioSessionMultiSelector.SetSessionIsSelected(this.AudioSession, value);
                _isSelectedChanging = false;
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
        private ImageSource? GetIcon()
        {
            if (PID == 0)
            { // if this is the system idle process (System Sounds), use the icon from the application's resources instead of trying (and failing) to get the icon from the DLL
                return IconExtractor.ExtractFromHandle(Properties.Resources.idle.Handle);
            }

            // try getting the icon from WASAPI
            var iconPath = AudioSession.AudioSessionControl.IconPath;
            if (iconPath.Length > 0 && IconExtractor.TryExtractFromPath(iconPath, out ImageSource wasapiImageSource))
            {
                wasapiImageSource.Freeze();
                return wasapiImageSource;
            }

            // try getting the icon from the process
            using var proc = AudioSession.GetProcess();

            if (proc == null) return null;
            try
            {
                if (proc.GetMainModulePath() is string path && IconExtractor.TryExtractFromPath(path, out ImageSource processImageSource))
                {
                    processImageSource.Freeze();
                    return processImageSource;
                }
            }
            catch (Exception ex)
            {
                FLog.Log.Error($"Failed to get an icon for session '{Name}' because of an exception:", ex);
            }

            return null;
        }
        #endregion Methods

        #region IDisposable Implementation
        public void Dispose()
        {
            ((IDisposable)this.AudioSession).Dispose();
            _icon = null;
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation

        private bool _isSelectedChanging;
        private void AudioSessionMultiSelector_SessionSelected_SessionDeselected(object? sender, AudioSession e)
        {
            if (_isSelectedChanging) return; //< don't update if the source is this object

            if (AudioSession.Equals(e))
            {
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
