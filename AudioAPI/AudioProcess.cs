using AudioAPI.Forms;
using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public class AudioProcess : IAudioProcess, IGridViewAudioProcess
    {
        #region Constructor

        public AudioProcess(IAudioSessionControl2 session)
        {
            session.GetProcessId(out int pid);
            Process = Process.GetProcessById(pid);
            SessionControl = session;
            AudioControl = (ISimpleAudioVolume)SessionControl;
        }
        ~AudioProcess()
        {
            Dispose(disposing: false);
        }

        #endregion Constructor

        #region Member

        private bool disposedValue;

        #endregion Member

        #region Properties

        /// <summary>
        /// A handle to the attached process.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Audio session controller for the attached process.
        /// </summary>
        public IAudioSessionControl2 SessionControl { get; }

        /// <summary>
        /// The process ID number of the attached process.
        /// </summary>
        public int PID
        {
            get => Process.Id;
        }

        /// <summary>
        /// The process name of the attached process.
        /// </summary>
        public string ProcessName
        {
            get => Process.ProcessName;
        }

        /// <summary>
        /// The display name of the attached process.
        /// This is often blank.
        /// </summary>
        public string DisplayName
        {
            get
            {
                SessionControl.GetDisplayName(out string n);
                return n;
            }
            set
            {
                Guid guid = Guid.NewGuid();
                SessionControl.SetDisplayName(value, guid);
            }
        }

        public ISimpleAudioVolume AudioControl { get; }

        /// <summary>
        /// Property that controls the volume of the attached process.
        /// Valid values for this property range from (0.0 - 1.0).
        /// </summary>
        public float Volume
        {
            get
            {
                AudioControl.GetMasterVolume(out float v);
                return v;
            }
            set
            {
                Guid guid = Guid.Empty;
                AudioControl.SetMasterVolume(value, ref guid);
            }
        }

        /// <summary>
        /// Property that controls the volume of the attached process.
        /// Valid values for this property range from (0 - 100).
        /// </summary>
        public decimal VolumeFullRange
        {
            get => Convert.ToDecimal(Volume) * 100m;
            set => Volume = (float)(value / 100m);
        }

        /// <summary>
        /// Retrieve or set the application's volume using a string that ends with an optional percent character.
        /// </summary>
        public string VolumePercent
        {
            get => VolumeFullRange.ToString() + '%';
            set
            {
                if (value != null)
                {
                    string match = RegularExpr.ContiguousDigits.Match(value).Value;
                    if (match.Length > 0 && decimal.TryParse(match, out decimal dval))
                        VolumeFullRange = dval;
                }
            }
        }

        /// <summary>
        /// Property that controls the mute state of the attached process.
        /// </summary>
        public bool Muted
        {
            get
            {
                AudioControl.GetMute(out bool m);
                return m;
            }
            set
            {
                Guid guid = Guid.NewGuid();
                AudioControl.SetMute(value, ref guid);
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Process.Dispose();
                    Marshal.ReleaseComObject(SessionControl);
                    Marshal.ReleaseComObject(AudioControl);
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
