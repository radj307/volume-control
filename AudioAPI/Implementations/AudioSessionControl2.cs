using AudioAPI.WindowsAPI;
using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioAPI.Implementations
{
    /// <summary>
    /// Wrapper object for the <see cref="IAudioSessionControl2"/> audio interface that automatically handles memory.
    /// Exposes properties that allow controlling and identifying the audio session and its associated process.
    /// </summary>
    public class AudioSessionControl2 : IAudioSessionControl2, IDisposable
    {
        #region Constructor
        public AudioSessionControl2(IAudioSessionControl2 control, Guid? defaultContext = null)
        {
            _control = control;
            GetProcessId(out _pid);
            _defaultContext = defaultContext ?? Guid.NewGuid();
        }
        public AudioSessionControl2(SimpleAudioVolume session, Guid? defaultContext = null)
        {
            if (!session.IsValid) throw new InvalidOperationException("Cannot construct AudioSessionControl2 instance using a disposed-of SimpleAudioVolume object!");
            _control = (IAudioSessionControl2)session.Interface;
            GetProcessId(out _pid);
            _defaultContext = defaultContext ?? Guid.NewGuid();
        }
        #endregion Constructor

        #region Disposal
        ~AudioSessionControl2() => Dispose(disposing: false);
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                Marshal.ReleaseComObject(_control);
                disposedValue = true;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Disposal

        #region Members
        private readonly IAudioSessionControl2 _control;
        private bool disposedValue;
        private readonly Guid _defaultContext;

        private readonly int _pid;
        #endregion Members

        #region Interface
        public int NotImpl0() => _control.NotImpl0();
        public int NotImpl1() => _control.NotImpl1();
        public int NotImpl2() => _control.NotImpl2();
        public int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal) => _control.GetDisplayName(out pRetVal);
        public int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string value, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext) => _control.SetDisplayName(value, eventContext);
        public int GetGroupingParam(out Guid pRetVal) => _control.GetGroupingParam(out pRetVal);
        public int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext) => _control.SetGroupingParam(Override, eventContext);
        public int GetProcessId(out int pRetVal) => _control.GetProcessId(out pRetVal);
        public int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal) => _control.GetSessionIdentifier(out pRetVal);
        public int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal) => _control.GetSessionInstanceIdentifier(out pRetVal);
        public int IsSystemSoundsSession() => _control.IsSystemSoundsSession();
        public int SetDuckingPreference(bool optOut) => _control.SetDuckingPreference(optOut);
        public int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal) => _control.GetIconPath(out pRetVal);
        public int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string value, [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext) => _control.SetIconPath(value, eventContext);
        #endregion Interface

        #region Methods
        /// <summary>
        /// Returns a new System.Diagnostics.Process component, given the identifier of a process on the local computer.
        /// </summary>
        /// <remarks>This is just a wrapper for <code language="cs">System.Diagnostics.Process.GetProcessById(this.PID);</code></remarks>
        /// <returns>The <see cref="Process"/> associated with this audio session.</returns>
        /// <exception cref="ArgumentException">The process specified by the processId parameter is not running. The identifier might be expired.</exception>
        /// <exception cref="InvalidOperationException">The process was not started by this object.</exception>
        public Process GetProcess() => Process.GetProcessById(PID);
        public SimpleAudioVolume GetSimpleAudioVolume(Guid? defaultContext = null) => new((ISimpleAudioVolume)_control, defaultContext);
        #endregion Methods

        #region Properties
        /// <summary>
        /// Get the underlying interface object for this session.
        /// </summary>
        internal IAudioSessionControl2 Interface => _control;
        /// <summary>
        /// Checks if this object has been invalidated and disposed of.
        /// </summary>
        public bool IsValid => !disposedValue;
        /// <summary>
        /// This is guaranteed not to throw, event after the process has exited.
        /// </summary>
        public int PID => _pid;
        /// <summary>
        /// Gets or sets the display name associated with this session.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (GetDisplayName(out string name).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
                return name;
            }
            set
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (SetDisplayName(value, _defaultContext).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
            }
        }
        /// <summary>
        /// Gets or sets the grouping parameter associated with this session.
        /// </summary>
        public Guid GroupingParam
        {
            get
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (GetGroupingParam(out Guid guid).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
                return guid;
            }
            set
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (SetGroupingParam(value, _defaultContext).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
            }
        }
        /// <summary>
        /// Gets or sets the icon path associated with this session.
        /// </summary>
        public string IconPath
        {
            get
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (GetIconPath(out string path).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
                return path;
            }
            set
            {
                if (!IsValid) throw new InvalidOperationException("Cannot perform operations on a disposed-of object!");
                if (SetIconPath(value, _defaultContext).Equals(ReturnCodes.AUDCLNT_E_DEVICE_INVALIDATED))
                    Dispose();
            }
        }
        #endregion Properties
    }
}
