using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public static class Volume
    {
        public static List<IMMDevice> GetAllDevicesNative()
        {
            // Get the first (default) ERender device with a multimedia role
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.EnumAudioEndpoints(EDataFlow.ERender, EDeviceState.Active, out IMMDeviceCollection deviceCollection);

            if (deviceCollection.GetCount(out int devCount) != 0)
                throw new Exception($"Failed to enumerate audio devices!");
            List<IMMDevice> devices = new();
            for (int i = 0; i < devCount; ++i)
            {
                if (deviceCollection.Item(i, out IMMDevice dev) == 0)
                    devices.Add(dev);
            }
            return devices;
        }
        public static List<AudioDevice> GetAllDevices()
        {
            List<AudioDevice> devices = new();
            foreach (var dev in GetAllDevicesNative())
            {
                devices.Add(new AudioDevice(dev));
            }
            return devices;
        }
        public static IMMDevice GetDevice(string endpointID)
        {
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.GetDevice(endpointID, out IMMDevice device);
            return device;
        }
        public static IMMDevice GetDefaultDevice()
        {
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);
            return device;
        }

        public static IAudioSessionControl2? GetSessionObject(int pid, IMMDevice device)
        {
            Guid iidAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref iidAudioSessionManager2, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator sessionEnumerator);
            sessionEnumerator.GetCount(out int sessionCount);

            IAudioSessionControl2? sessionControl = null;
            for (int i = 0; i < sessionCount; ++i)
            {
                sessionEnumerator.GetSession(i, out sessionControl);

                if (sessionControl == null)
                    continue;

                sessionControl.GetProcessId(out int sessionPID);

                if (sessionPID == pid)
                    break;
                // else
                Marshal.ReleaseComObject(sessionControl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(device);

            return sessionControl;
        }
        public static IAudioSessionControl2? GetSessionObject(int pid)
            => GetSessionObject(pid, GetDefaultDevice());

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
        public static ISimpleAudioVolume GetVolumeObject(int pid)
            => GetVolumeObject(pid, GetDefaultDevice());

        public static List<IAudioSessionControl2> GetAllSessionsNative(IMMDevice device)
        {
            Guid GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);

            List<IAudioSessionControl2> l = enumerator.GetAllSessions();

            Marshal.ReleaseComObject(enumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(device);

            return l;
        }
        public static List<IAudioSessionControl2> GetAllSessionsNative()
            => GetAllSessionsNative(GetDefaultDevice());
        public static List<AudioSession> GetAllSessions(IMMDevice device)
        {
            List<AudioSession> l = new List<AudioSession>();
            foreach (var session in GetAllSessionsNative(device))
            {
                l.Add(new AudioSession(session));
            }
            return l;
        }
        public static List<AudioSession> GetAllSessions()
            => GetAllSessions(GetDefaultDevice());
    }
}