using AudioAPI.Objects;
using AudioAPI.WindowsAPI;
using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Enum;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace AudioAPI.API
{
    /// <summary>
    /// Contains a wide array of static helper functions that are used by the <see cref="AudioAPI.Objects"/> namespace.<br/>
    /// This is public and can be used outside of <see cref="AudioAPI"/>, however it is hightly recommended that you use the objects rather than these raw API wrappers.
    /// </summary>
    public static class WrapperAPI
    {
        internal static bool WindowsVersionGreaterThan(int major, int minor)
        {
            int os_major = Environment.OSVersion.Version.Major, os_minor = Environment.OSVersion.Version.Minor;
            return os_major > major || (os_major == major && os_minor >= minor);
        }
        internal static object? GetManagementObject(string property)
        {
            object? obj = null;
            try
            {
                IEnumerable<ManagementObject> objects = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<ManagementObject>();
                obj = (from it in objects select it.GetPropertyValue(property)).FirstOrDefault();
            }
            catch (Exception) { }
            return obj;
        }

        #region CoreAudio
        public static List<IMMDevice> GetAllDevicesNative()
        {
            // Get the first (default) ERender device with a multimedia role
            var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
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
            var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.GetDevice(endpointID, out IMMDevice device);
            return device;
        }
        public static IMMDevice GetDefaultDevice()
        {
            var deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);
            return device;
        }

        public static IAudioSessionControl2? GetSessionObject(int pid, IMMDevice device)
        {
            Guid iidAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref iidAudioSessionManager2, 0, IntPtr.Zero, out object o);
            var mgr = (IAudioSessionManager2)o;

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
            var mgr = (IAudioSessionManager2)o;

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
            var mgr = (IAudioSessionManager2)o;

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
            var l = new List<AudioSession>();
            foreach (var session in GetAllSessionsNative(device))
            {
                l.Add(new AudioSession(session));
            }
            return l;
        }
        public static List<AudioSession> GetAllSessions()
            => GetAllSessions(GetDefaultDevice());
        #endregion CoreAudio


        #region StaticMembers
        #region ByProcessName
        public static void IncrementVolume(string proc_name, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_name);
                decimal newVolume = currentVolume + increment;
                SetVolume(proc_name, newVolume >= 100.0m ? 100.0m : newVolume);
            }
            catch
            {
                // send media volume up keypress globally
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_UP, 0, 1, IntPtr.Zero);
            }
        }

        public static void DecrementVolume(string proc_name, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_name);
                decimal newVolume = currentVolume - increment;
                SetVolume(proc_name, newVolume <= 0.0m ? 0.0m : newVolume);
            }
            catch
            {
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_DOWN, 0, 1, IntPtr.Zero);
            }
        }

        public static void ToggleMute(string proc_name)
        {
            try
            {
                if (!WindowsVersionGreaterThan(6, 1))
                    throw new NotSupportedException($"This application requires Windows 7 or newer!");

                SetMute(proc_name, !IsMuted(proc_name));
            }
            catch
            {
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_MUTE, 0, 1, IntPtr.Zero);
            }
        }

        public static decimal GetVolume(string proc_name)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException($"Volume object creation failed for target \"{proc_name}\"");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return (decimal)level * 100m;
        }

        public static bool TryGetVolume(string proc_name, out decimal volume)
        {
            volume = 0m;
            ISimpleAudioVolume? volObj = GetVolumeObject(proc_name);
            if (volObj == null)
                return false;
            volObj.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volObj);
            volume = Convert.ToDecimal(level);
            return true;
        }

        public static bool IsMuted(string proc_name)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        public static bool TryIsMuted(string proc_name, out bool muted)
        {
            muted = false;
            ISimpleAudioVolume? volObj = GetVolumeObject(proc_name);
            if (volObj == null)
                return false;
            volObj.GetMute(out muted);
            Marshal.ReleaseComObject(volObj);
            return true;
        }

        public static void SetVolume(string proc_name, decimal level)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume((float)(level / 100.0m), ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static void SetMute(string proc_name, bool mute)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_name);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static ISimpleAudioVolume? GetVolumeObject(string proc_name)
        {
            var audioVolumeObjects = from p in Process.GetProcessesByName(proc_name)
                                     let vol = GetVolumeObject(p.Id)
                                     where vol != null
                                     select vol;
            return audioVolumeObjects.FirstOrDefault();
        }
        #endregion ByProcessName
        #region ByProcessID
        public static void IncrementVolume(int proc_id, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_id);
                decimal newVolume = currentVolume + increment;
                SetVolume(proc_id, newVolume >= 100.0m ? 100.0m : newVolume);
            }
            catch
            {
                // send media volume up keypress globally
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_UP, 0, 1, IntPtr.Zero);
            }
        }

        public static void DecrementVolume(int proc_id, decimal increment)
        {
            try
            {
                decimal currentVolume = GetVolume(proc_id);
                decimal newVolume = currentVolume - increment;
                SetVolume(proc_id, newVolume <= 0.0m ? 0.0m : newVolume);
            }
            catch
            {
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_DOWN, 0, 1, IntPtr.Zero);
            }
        }

        public static void ToggleMute(int proc_id)
        {
            try
            {
                if (!WindowsVersionGreaterThan(6, 1))
                    throw new NotSupportedException("This feature is only available on Windows 7 or newer");

                SetMute(proc_id, !IsMuted(proc_id));
            }
            catch
            {
                User32.KeyboardEvent(EVirtualKeyCode.VK_VOLUME_MUTE, 0, 1, IntPtr.Zero);
            }
        }

        public static decimal GetVolume(int proc_id)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMasterVolume(out float level);
            Marshal.ReleaseComObject(volume);
            return Convert.ToDecimal(level) * 100m;
        }

        public static bool IsMuted(int proc_id)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            volume.GetMute(out bool mute);
            Marshal.ReleaseComObject(volume);
            return mute;
        }

        public static void SetVolume(int proc_id, decimal level)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMasterVolume((float)(level / 100.0m), ref guid);
            Marshal.ReleaseComObject(volume);
        }

        public static void SetMute(int proc_id, bool mute)
        {
            ISimpleAudioVolume? volume = GetVolumeObject(proc_id);
            if (volume == null)
                throw new COMException("Volume object creation failed");

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
            Marshal.ReleaseComObject(volume);
        }

        #endregion ByProcessID
        #endregion StaticMembers
    }
}