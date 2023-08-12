using Audio;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Log;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// ViewModel for the <see cref="Audio.AudioSession"/> class.
    /// </summary>
    public class AudioSessionVM : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        public AudioSessionVM(AudioSession audioSession)
        {
            AudioSession = audioSession;

            IconPair = GetIconPair();

            AudioSession.IconPathChanged += (s, e) => IconPair = GetIconPair();
        }
        #endregion Constructor

        #region Properties
        public AudioSession AudioSession { get; }
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
        public uint PID => AudioSession.PID;
        public string ProcessName => AudioSession.ProcessName;
        public string Name
        {
            get => AudioSession.Name;
            set => AudioSession.Name = value;
        }
        public string ProcessIdentifier => AudioSession.ProcessIdentifier;
        public int Volume
        {
            get => AudioSession.Volume;
            set => AudioSession.Volume = value;
        }
        public bool Mute
        {
            get => AudioSession.Mute;
            set => AudioSession.Mute = value;
        }
        public float PeakMeterValue => AudioSession.PeakMeterValue;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        private IconPair GetIconPair()
        {
            IconPair icons = null!;
            var iconPath = AudioSession.AudioSessionControl.IconPath;
            if (iconPath.Length > 0)
            {
                icons = IconGetter.GetIcons(iconPath);
                if (!icons.IsNull)
                    return icons;
            }
            using Process? proc = AudioSession.GetProcess();
            try
            {
                if (proc?.GetMainModulePath() is string path)
                    return IconGetter.GetIcons(path);
            }
            catch (Exception ex)
            {
                FLog.Log.Error($"Failed to query information for process {proc?.Id}", ex);
            }
            return icons;
        }

        public void Dispose()
        {
            ((IDisposable)this.AudioSession).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
