using AudioAPI.WindowsAPI;
using AudioAPI.WindowsAPI.Audio;
using System.Runtime.InteropServices;

namespace AudioAPI.Implementations
{
    /// <summary>
    /// Wrapper object for the <see cref="ISimpleAudioVolume"/> interface that handles memory cleanup. <br/>
    /// Exposes properties that allow controlling the volume and muted state of an application.
    /// </summary>
    public class SimpleAudioVolume : ISimpleAudioVolume, IDisposable
    {
        #region Constructor
        public SimpleAudioVolume(ISimpleAudioVolume control, Guid? defaultContext = null)
        {
            _control = control;
            _defaultContext = defaultContext ?? Guid.NewGuid();
        }
        public SimpleAudioVolume(AudioSessionControl2 session, Guid? defaultContext = null)
        {
            if (!session.IsValid) throw new InvalidOperationException("Cannot construct SimpleAudioVolume instance using a disposed-of AudioSessionControl2 object!");
            _control = (ISimpleAudioVolume)session.Interface;
            _defaultContext = defaultContext ?? Guid.NewGuid();
        }
        #endregion Constructor

        #region Disposal
        ~SimpleAudioVolume() => Dispose(disposing: false);
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                Marshal.ReleaseComObject(_control);
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Disposal

        #region Members
        /// <summary>
        /// This is used as the eventContext parameter when using properties.
        /// </summary>
        private Guid _defaultContext;
        private readonly ISimpleAudioVolume _control;
        private bool disposedValue;
        #endregion Members

        #region Interface
        public int SetMasterVolume(float fLevel, ref Guid eventContext) => _control.SetMasterVolume(fLevel, ref eventContext);
        public int GetMasterVolume(out float pfLevel) => _control.GetMasterVolume(out pfLevel);
        public int SetMute(bool bMute, ref Guid eventContext) => _control.SetMute(bMute, ref eventContext);
        public int GetMute(out bool pbMute) => _control.GetMute(out pbMute);
        #endregion Interface

        #region Methods
        /// <summary>
        /// Clamp an arbitrary numerical type between given minimum and maximum boundaries. <br/>
        /// Boundary values are inclusive, so the returned value can be equal to either min or max, or anywhere in between.
        /// </summary>
        /// <typeparam name="T">Any numerical type.</typeparam>
        /// <param name="value">Unclamped input value.</param>
        /// <param name="min">Minimum boundary.</param>
        /// <param name="max">Maximum boundary.</param>
        /// <returns>value clamped at or between min and max.</returns>
        private static T Clamp<T>(T value, T min, T max) where T : IComparable<T>, IEquatable<T>, IConvertible
        {
            if (value.CompareTo(min) < 0) // value preceeds min
                value = min; // clamp to min
            else if (value.CompareTo(max) > 0) // value follows max
                value = max; // clamp to max
            return value;
        }
        public AudioSessionControl2 GetAudioSessionControl2(Guid? defaultContext = null) => new((IAudioSessionControl2)_control, defaultContext);
        #endregion Methods

        #region Properties
        /// <summary>
        /// Get the underlying interface object for this session.
        /// </summary>
        internal ISimpleAudioVolume Interface => _control;
        /// <summary>
        /// Checks if this object has been invalidated and disposed of.
        /// </summary>
        public bool IsValid => !disposedValue;
        public float Volume
        {
            get
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (GetMasterVolume(out float level).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
                return level;
            }
            set
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (SetMasterVolume(Clamp(value, 0f, 1f), ref _defaultContext).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose(); // dispose of invalidated interface
            }
        }
        public int VolumeFullRange
        {
            get => Convert.ToInt32(Volume * 100f);
            set => Volume = (float)(Convert.ToDecimal(Clamp(value, 0, 100)) / 100m);
        }
        public bool Muted
        {
            get
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (GetMute(out bool muted).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
                return muted;
            }
            set
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (SetMute(value, ref _defaultContext).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
            }
        }
        #endregion Properties

        #region Methods

        #endregion Methods
    }
}
