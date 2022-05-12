//using AudioAPI.API;
//using AudioAPI.Interfaces;
//using AudioAPI.Objects;
//using AudioAPI.Objects.Virtual;
//using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using NAudio.CoreAudioApi;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Interfaces;

namespace VolumeControl.Core.HelperTypes
{
    public class AudioDevice : IDevice, INotifyPropertyChanged, IDisposable
    {
        #region Constructors
        public AudioDevice(MMDevice device)
        {
            _device = device;
        }
        #endregion Constructors

        #region Fields
        private readonly MMDevice _device;
        #endregion Fields

        #region Properties
        public string Name => FriendlyName;
        public string DeviceID => _device.ID;
        public string InstanceID => _device.InstanceId;
        public string IconPath => _device.IconPath;
        public string DeviceFriendlyName => _device.DeviceFriendlyName;
        public string FriendlyName => _device.FriendlyName;
        public PropertyStore Properties => _device.Properties;
        public DeviceState State => _device.State;
        public AudioSessionManager SessionManager => _device.AudioSessionManager;
        public AudioEndpointVolume EndpointVolumeObject => _device.AudioEndpointVolume;
        public float EndpointNativeVolume
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
        public int EndpointVolume
        {
            get => Convert.ToInt32(EndpointNativeVolume * 100f);
            set
            {
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                EndpointNativeVolume = (float)(Convert.ToDouble(value) / 100.0);
                NotifyPropertyChanged();
            }
        }
        public bool EndpointMuted => EndpointVolumeObject.Mute;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods
        public List<AudioSession> GetAudioSessions()
        {
            var sessions = _device.AudioSessionManager.Sessions;
            List<AudioSession> l = new();
            for (int i = 0; i < sessions.Count; ++i)
            {
                l.Add(new AudioSession(sessions[i]));
            }
            return l;
        }

        public void Dispose()
        {
            ((IDisposable)_device).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion Methods
    }
}
