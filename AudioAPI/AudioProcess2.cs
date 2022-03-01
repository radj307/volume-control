using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public class AudioProcess2 : IDisposable
    {
        private readonly Process _process;
        private ISimpleAudioVolume? _controller;
        private bool _valid;

        public AudioProcess2(Process process)
        {
            _process = process;

            InitializeProcess();
        }
        public AudioProcess2(ref IAudioSessionControl2 session)
        {
            session.GetProcessId(out int pid);
            _process = Process.GetProcessById(pid);

            InitializeProcess();
        }
        public AudioProcess2(int PID)
        {
            _process = Process.GetProcessById(PID);

            InitializeProcess();
        }

        /// <summary>
        /// Registers event handlers for the current process.
        /// </summary>
        private void InitializeProcess()
        {
            if (_process == null)
                throw new Exception("Cannot initialize process event handler: Process is null!");
            _valid = true;
            _process.Exited += delegate { Dispose(); };
        }

        private void ReloadController()
        {
            if (_valid)
            {
                if (_controller != null)
                    Marshal.ReleaseComObject(_controller);
                _controller = null;

                // Get the default output device
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice speakers);

                speakers.GetId(out string defaultDeviceId);

                ISimpleAudioVolume volumeControl = GetVolumeObject(PID, speakers);
                Marshal.ReleaseComObject(speakers);

                if (volumeControl == null) // Controller is using a different device
                {
                    // Enumerate all audio devices
                    deviceEnumerator.EnumAudioEndpoints(EDataFlow.ERender, EDeviceState.Active, out IMMDeviceCollection deviceCollection);
                    deviceCollection.GetCount(out int count);
                    for (int i = 0; i < count; i++)
                    {
                        deviceCollection.Item(i, out IMMDevice device);
                        device.GetId(out string deviceId);
                        try
                        {
                            if (deviceId == defaultDeviceId) // this session is using the default device, skip it
                                continue;

                            volumeControl = GetVolumeObject(PID, device);
                            if (volumeControl != null)
                                break;
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(device);
                        }
                    }
                }

                Marshal.ReleaseComObject(deviceEnumerator);

                if (volumeControl == null)
                    Dispose();
                else
                    _controller = volumeControl;
            }
        }
        private static ISimpleAudioVolume GetVolumeObject(int pid, IMMDevice device)
        {
            // Activate the session manager. we need the enumerator
            Guid iidIAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref iidIAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // Enumerate sessions for this device
            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int count);

            // Search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume? volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                sessionEnumerator.GetSession(i, out IAudioSessionControl2 ctl);
                ctl.GetProcessId(out int cpid);

                if (cpid == pid)
                {
                    volumeControl = (ISimpleAudioVolume)ctl;
                    break;
                }

                Marshal.ReleaseComObject(ctl);
            }

            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
#           pragma warning disable CS8603 // Possible null reference return.
            return volumeControl;
#           pragma warning restore CS8603 // Possible null reference return.
        }

        private static Guid GetSetterGuid()
            => Guid.NewGuid();

        public string ProcessName { get => _process.ProcessName; }
        public int PID { get => _process.Id; }
        public bool Running
        {
            get => _valid;
            set
            {
                if (!value)
                {
                    if (_valid)
                        _process.Kill();
                }
                else if (!_valid) // value is true, valid is false: (re)start the process
                    _process.Start();
            }
        }

        public float Volume
        {
            get
            {
                if (_controller == null && _valid)
                    ReloadController();
                if (_controller != null)
                {
                    _controller.GetMasterVolume(out float vol);
                    return vol;
                }
                else return 0f;
            }
            set
            {
                if (_controller == null && _valid)
                    ReloadController();
                if (_controller != null)
                {
                    Guid guid = GetSetterGuid();
                    _controller.SetMasterVolume(value, ref guid);
                }
            }
        }
        public decimal VolumeFullRange
        {
            get => Convert.ToDecimal(Volume) * 100m;
            set => Volume = (float)(value / 100m);
        }
        public string VolumePercent
        {
            get => Volume.ToString() + '%';
            set
            {
                if (decimal.TryParse(RegularExpr.ValidNumber.Match(value).Value, out decimal vol))
                    VolumeFullRange = vol;
            }
        }
        public bool Muted
        {
            get
            {
                if (_controller == null && _valid)
                    ReloadController();
                if (_controller != null)
                {
                    _controller.GetMute(out bool m);
                    return m;
                }
                return false;
            }
            set
            {
                if (_controller == null && _valid)
                    ReloadController();
                if (_controller != null)
                {
                    Guid guid = GetSetterGuid();
                    _controller.SetMute(value, ref guid);
                }
            }
        }

        ~AudioProcess2() => Dispose(disposing: false);

        protected virtual void Dispose(bool disposing)
        {
            if (_valid)
            {
                if (disposing)
                {
                    _process.Dispose();
                    if (_controller != null)
                        Marshal.ReleaseComObject(_controller);
                }

                _valid = false;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
