using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VolumeControl.Audio.Interfaces;

namespace VolumeControl.Audio
{
    public class AudioDevice : IDevice, INotifyPropertyChanged, IDisposable
    {
        #region Constructors
        public AudioDevice(MMDevice device)
        {
            _device = device;
            _deviceID = _device.ID;
        }
        #endregion Constructors

        #region Fields
        private readonly string _deviceID;
        private MMDevice _device;
        private (ImageSource?, ImageSource?)? _icons = null;
        #endregion Fields

        #region Properties
        /// <inheritdoc/>
        public string Name => FriendlyName;
        /// <inheritdoc/>
        public string DeviceID => _deviceID;
        public string InstanceID => _device.InstanceId;
        public string IconPath => _device.IconPath;
        public ImageSource? SmallIcon => (_icons ??= this.GetIcons())?.Item1;
        public ImageSource? LargeIcon => (_icons ??= this.GetIcons())?.Item2;
        public ImageSource? Icon => SmallIcon ?? LargeIcon;
        public string DeviceFriendlyName => _device.DeviceFriendlyName;
        public string FriendlyName => _device.FriendlyName;
        public PropertyStore Properties => _device.Properties;
        public DeviceState State => _device.State;
        public AudioSessionManager SessionManager => _device.AudioSessionManager;
        public AudioEndpointVolume EndpointVolumeObject => _device.AudioEndpointVolume;
        /// <inheritdoc/>
        public float NativeEndpointVolume
        {
            get => EndpointVolumeObject.MasterVolumeLevel;
            set
            {
                if (value > 1f)
                    value = 1f;
                else if (value < 0f)
                    value = 0f;
                EndpointVolumeObject.MasterVolumeLevel = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public int EndpointVolume
        {
            get => Convert.ToInt32(NativeEndpointVolume * 100f);
            set
            {
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                NativeEndpointVolume = (float)(Convert.ToDouble(value) / 100.0);
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool EndpointMuted
        {
            get => EndpointVolumeObject.Mute;
            set
            {
                EndpointVolumeObject.Mute = value;
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        protected virtual void Reload() => _device = new MMDeviceEnumerator().GetDevice(_deviceID);
        public List<AudioSession> GetAudioSessions()
        {
            Reload();
            SessionCollection? sessions = _device.AudioSessionManager.Sessions;
            List<AudioSession> l = new();
            for (int i = 0; i < sessions.Count; ++i)
            {
                l.Add(new AudioSession(sessions[i]));
            }
            return l;
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            _device.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
