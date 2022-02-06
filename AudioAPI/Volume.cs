using System.Runtime.InteropServices;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio;

namespace AudioAPI
{
    public static class Volume
    {
        #region Static Members

        public static ISimpleAudioVolume GetVolumeObject(int pid)
        {
            // Get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice speakers);

            speakers.GetId(out string defaultDeviceId);

            ISimpleAudioVolume volumeControl = GetVolumeObject(pid, speakers);
            Marshal.ReleaseComObject(speakers);

            if (volumeControl == null)
            {
                // If volumeControl is null, then the process's volume object might be on a different device.
                // This happens if the process doesn't use the default device.
                // 
                // As far as Spotify is concerned, if using the "--enable-audio-graph" command line argument,
                // a new option becomes available in the Settings that makes it possible to change the playback device.

                deviceEnumerator.EnumAudioEndpoints(EDataFlow.ERender, EDeviceState.Active, out IMMDeviceCollection deviceCollection);

                deviceCollection.GetCount(out int count);
                for (int i = 0; i < count; i++)
                {
                    deviceCollection.Item(i, out IMMDevice device);
                    device.GetId(out string deviceId);

                    try
                    {
                        if (deviceId == defaultDeviceId)
                            continue;

                        volumeControl = GetVolumeObject(pid, device);
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
#           pragma warning disable CS8603 // Possible null reference return.
            return volumeControl;
#           pragma warning restore CS8603 // Possible null reference return.
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

        #endregion
    }
}