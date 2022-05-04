using AudioAPI.Interfaces;
using AudioAPI.WindowsAPI.Audio;
using System.ComponentModel;
using System.Diagnostics;

namespace AudioAPI
{
    public class AudioSession : IAudioSession
    {
        #region Constructor
        public AudioSession(IAudioSessionControl2 control, Guid? defaultContext = null)
        {
            SessionControl = new(control, defaultContext);
            SessionVolume = SessionControl.GetSimpleAudioVolume(defaultContext);
            SessionProcess = SessionControl.GetProcess();
            SessionProcess.Disposed += delegate { Dispose(); };
        }
        #endregion Constructor

        #region Fields
        internal readonly Process SessionProcess;
        private bool disposedValue;
        #endregion Fields

        #region Properties
        public AudioSessionControl2 SessionControl { get; }
        public SimpleAudioVolume SessionVolume { get; }
        public bool Virtual => false;
        [Bindable(false)]
        public VolumeObject VolumeObject
        {
            get => VolumeObject.From(SessionVolume.Volume);
            set => SessionVolume.Volume = value.ToFloatVolume();
        }
        public decimal Volume
        {
            get => VolumeObject.Value * 100;
            set => VolumeObject = VolumeObject.From(value, 0m, 100m);
        }
        public bool Muted
        {
            get => SessionVolume.Muted;
            set => SessionVolume.Muted = value;
        }
        public string ProcessName => SessionProcess.ProcessName;
        public int PID => SessionProcess.Id;
        #endregion Properties

        #region Methods
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    SessionControl.Dispose();
                    SessionVolume.Dispose();
                    SessionProcess.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public bool Equals(IProcess? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
        public bool Equals(IAudioSession? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
        #endregion Methods
    }
}
