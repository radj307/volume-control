using AudioAPI.Implementations;
using AudioAPI.Interfaces;
using AudioAPI.WindowsAPI.Audio;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AudioAPI.Objects
{
    /// <summary>
    /// Wrapper object implementing <see cref="IAudioSession"/> that maintains a <see cref="AudioSessionControl2"/> object, a <see cref="SimpleAudioVolume"/> object, and forwards their most useful properties.
    /// </summary>
    /// <remarks>This object also implements <see cref="IDisposable"/> to clean up COM resources.</remarks>
    public class AudioSession : IAudioSession, IDisposable, INotifyPropertyChanged
    {
        #region Constructor
        /// <summary>
        /// <see cref="AudioSession"/> constructor.
        /// </summary>
        /// <param name="control">An <see cref="IAudioSessionControl2"/> interface.</param>
        /// <param name="defaultContext">Optional <see cref="Guid"/> that is used for initializing the <see cref="IAudioSessionControl2"/> and <see cref="ISimpleAudioVolume"/> interfaces.<br/>Passing <see cref="null"/> will result in a random guid being used. (<see cref="Guid.NewGuid()"/>)</param>
        public AudioSession(IAudioSessionControl2 control, Guid? defaultContext = null)
        {
            if (defaultContext is null)
                defaultContext = Guid.NewGuid();

            SessionControl = new(control, defaultContext);
            SessionVolume = SessionControl.GetSimpleAudioVolume(defaultContext);
            SessionProcess = SessionControl.GetProcess();
            SessionProcess.Disposed += delegate { Dispose(); };
        }
        #endregion Constructor

        #region Fields
        private bool disposedValue;

        public event PropertyChangedEventHandler? PropertyChanged = null;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Fields

        #region Properties
        /// <summary>
        /// Management object that implements <see cref="IAudioSessionControl2"/>, and allows some interaction with the session's properties.<br/>
        /// This is used to retrieve both the <see cref="SessionVolume"/> property and the <see cref="SessionProcess"/> property.
        /// </summary>
        internal AudioSessionControl2 SessionControl { get; }
        /// <summary>
        /// Management object that implements <see cref="ISimpleAudioVolume"/>, and allows controlling the attached session's volume and mute state.
        /// </summary>
        internal SimpleAudioVolume SessionVolume { get; }
        /// <summary>
        /// This is the <see cref="Process"/> that owns this audio session.
        /// </summary>
        internal Process SessionProcess { get; }

        /// <inheritdoc/>
        /// <remarks>This object is never virtual.</remarks>
        public bool Virtual => false;
        /// <inheritdoc/>
        public int Volume
        {
            get => SessionVolume.VolumeFullRange;
            set
            {
                SessionVolume.VolumeFullRange = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public float NativeVolume
        {
            get => SessionVolume.Volume;
            set
            {
                SessionVolume.Volume = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public bool Muted
        {
            get => SessionVolume.Muted;
            set
            {
                SessionVolume.Muted = value;
                NotifyPropertyChanged();
            }
        }
        /// <inheritdoc/>
        public string ProcessName => SessionProcess.ProcessName;
        /// <inheritdoc/>
        public int PID => SessionProcess.Id;

        public string ProcessIdentifier => $"{PID}:{ProcessName}";
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
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>
        public bool Equals(IProcess? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
        /// <inheritdoc/>
        public bool Equals(IAudioSession? other) => Virtual.Equals(other?.Virtual) && PID.Equals(other?.PID);
        #endregion Methods
    }
}
