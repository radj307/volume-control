using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace VolumeControl
{
    internal static class AudioSessionList
    {
        public static BindingList<string> GetProcessNames()
        {

            IMMDeviceEnumerator devEnum = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            devEnum.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);

            Guid GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);
            List<IAudioSessionControl2> Sessions = enumerator.GetAllSessions();

            BindingList<string> l = new();

            foreach (IAudioSessionControl2 session in Sessions)
            {
                session.GetProcessId(out int pid);
                l.Add(Process.GetProcessById(pid).ProcessName);
            }

            return l;
        }
        public static BindingList<string> GetProcessNames(IMMDevice device)
        {
            Guid GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);
            List<IAudioSessionControl2> Sessions = enumerator.GetAllSessions();

            BindingList<string> l = new();

            foreach (IAudioSessionControl2 session in Sessions)
            {
                session.GetProcessId(out int pid);
                l.Add(Process.GetProcessById(pid).ProcessName);
            }

            return l;
        }
    }
}
