using AudioAPI;
using AudioAPI.WindowsAPI.Audio;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VolumeControl.Core.Audio
{
    public class AudioProcess : INotifyPropertyChanged, IDisposable, IAudioProcess
    {
        #region Constructor

        public AudioProcess(IAudioSessionControl2 session)
        {
            SessionControl = new(session, _defaultContext);
            AudioControl = SessionControl.GetSimpleAudioVolume(_defaultContext);
            Process = Process.GetProcessById(SessionControl.PID);
            _processName = Process.ProcessName;
        }
        public AudioProcess(Process proc)
        {
            Process = proc;
            var session = AudioAPI.Volume.GetSessionObject(proc.Id);
            if (session == null) throw new ArgumentException($"Process [{Process.Id}] '{Process.ProcessName}' doesn't have an audio session!");
            SessionControl = new(session, _defaultContext);
            AudioControl = SessionControl.GetSimpleAudioVolume(_defaultContext);
            _processName = Process.ProcessName;
        }

        #endregion Constructor

        #region Member
        private readonly Guid _defaultContext = Guid.NewGuid();
        private readonly string _processName;

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Member

        #region Properties

        /// <summary>
        /// The <see cref="System.Diagnostics.Process"/> associated with this audio session.
        /// </summary>
        public Process Process { get; }

        /// <summary>See <see cref="AudioSessionControl2"/> for documentation.</summary>
        public AudioSessionControl2 SessionControl { get; }

        /// <summary>See <see cref="SimpleAudioVolume"/> for documentation.</summary>
        public SimpleAudioVolume AudioControl { get; }

        /// <summary>
        /// The process ID number of the attached process.
        /// This is guaranteed to not throw.
        /// </summary>
        public int PID => SessionControl.PID;

        /// <summary>
        /// The process name of the attached process.
        /// This is guaranteed to not throw.
        /// </summary>
        public string ProcessName => _processName;

        /// <summary>
        /// The display name of the attached process.
        /// This is often blank.
        /// </summary>
        public string DisplayName
        {
            get => SessionControl.DisplayName;
            set => SessionControl.DisplayName = value;
        }

        /// <summary>
        /// Property that controls the volume of the attached process.
        /// Valid values for this property range from (0.0 - 1.0).
        /// </summary>
        public float Volume
        {
            get => AudioControl.Volume;
            set => AudioControl.Volume = value;
        }

        /// <summary>
        /// Property that controls the volume of the attached process.
        /// Valid values for this property range from (0 - 100).
        /// </summary>
        public decimal VolumeFullRange
        {
            get => Convert.ToDecimal(AudioControl.VolumeFullRange);
            set => AudioControl.VolumeFullRange = Convert.ToInt32(value);
        }

        /// <summary>
        /// Retrieve or set the application's volume using a string that ends with an optional percent character.
        /// </summary>
        public string VolumePercent
        {
            get
            {
                string s = VolumeFullRange.ToString();
                if (s.EndsWith('0'))
                {
                    int pos = s.IndexOf('.');
                    if (pos != -1)
                    {
                        s = s[..pos];
                    }
                }

                return s + '%';
            }
            set
            {
                if (value != null)
                {
                    string match = RegularExpr.ContiguousDigits.Match(value).Value;
                    if (match.Length > 0 && decimal.TryParse(match, out decimal dval))
                        VolumeFullRange = dval;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property that controls the mute state of the attached process.
        /// </summary>
        public bool Muted
        {
            get => AudioControl.Muted;
            set => AudioControl.Muted = value;
        }

        /// <summary>
        /// Checks if the process is still running.
        /// <br></br>
        /// This works by calling <see cref="Process.GetProcessesByName(string?)"/> and checking if the returned list size is greater than 0, and contains a process with the same ID.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                var list = Process.GetProcessesByName(ProcessName);
                return list.Length > 0 && list.Any(p => p.Id == PID);
            }
        }

        public bool Virtual => false;

        #endregion Properties

        #region Methods
        public bool ToggleMute() => Muted = !Muted;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Process.Dispose();
            AudioControl.Dispose();
            SessionControl.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion Methods
    }
}
