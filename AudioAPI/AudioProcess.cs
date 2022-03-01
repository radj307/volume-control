using AudioAPI.Forms;
using AudioAPI.WindowsAPI.Audio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public class AudioProcess : IAudioProcess, IGridViewAudioProcess
    {
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

        private bool disposedValue;

        public Process Process { get; }
        public IAudioSessionControl2 SessionControl { get; }
        public int PID
        {
            get => Process.Id;
        }
        public string ProcessName
        {
            get => Process.ProcessName;
        }
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
    }
}
