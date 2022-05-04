using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;

namespace AudioAPI
{
    public class ProcessAudioControl : IDisposable
    {
        #region Constructor
        public ProcessAudioControl(IAudioSessionControl2 control, Guid? defaultContext = null)
        {
            SessionControl = new(control, defaultContext);
            SessionVolume = SessionControl.GetSimpleAudioVolume(defaultContext);
            SessionProcess = SessionControl.GetProcess();
            SessionProcess.Disposed += delegate { Dispose(); };
        }
        #endregion Constructor

        #region Fields
        internal readonly AudioSessionControl2 SessionControl;
        internal readonly SimpleAudioVolume SessionVolume;
        internal readonly Process SessionProcess;
        private bool disposedValue;
        #endregion Fields

        #region Properties
        public VolumeObject VolumeObject
        {
            get => VolumeObject.From(SessionVolume.Volume);
            set => SessionVolume.Volume = value.ToFloatVolume();
        }
        public decimal Volume
        {
            get => VolumeObject.Value;
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
        #endregion Methods
    }
}
