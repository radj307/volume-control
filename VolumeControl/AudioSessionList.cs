using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VolumeControl
{
    internal class AudioSessionList
    {
        public AudioSessionList(IMMDevice device)
        {
            GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            Manager = (IAudioSessionManager2)o;

            UpdateSessions();
            UpdateProcessNames();
        }
        public AudioSessionList()
        {
            IMMDeviceEnumerator devEnum = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            devEnum.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);

            GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            Manager = (IAudioSessionManager2)o;

            UpdateSessions();
            UpdateProcessNames();
        }

        private readonly Guid GUID;
        private readonly IAudioSessionManager2 Manager;
        public List<IAudioSessionControl2> Sessions = new();
        public List<string> ProcessNames = new();

        public void UpdateSessions()
        {
            Manager.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);
            Sessions = enumerator.GetAllSessions();
        }

        public List<string> GetProcessNames()
        {
            UpdateSessions();

            List<string> l = new();

            foreach (IAudioSessionControl2 session in Sessions)
            {
                session.GetProcessId(out int pid);
                l.Add(Process.GetProcessById(pid).ProcessName);
            }

            return l;
        }

        public void UpdateProcessNames()
        {
            ProcessNames = GetProcessNames();
        }
    }
}
