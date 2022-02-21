using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;

namespace AudioAPI
{
    internal struct ReturnCode
    {
        internal static int S_OK = 0;
    }

    public static class API
    {
        public static IAudioSessionControl2[] GetVolumeObjectsForDevice(IMMDevice? dev = null)
        {
            if (dev == null)
            {
                IMMDeviceEnumerator devEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                devEnumerator.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out dev);
                Marshal.ReleaseComObject(devEnumerator);
            }

            Guid iidAudioSessionManager = typeof(IAudioSessionManager2).GUID;
            dev.Activate(ref iidAudioSessionManager, 0, IntPtr.Zero, out object oi);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)oi;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);

            IAudioSessionControl2[] objects = enumerator.GetAllSessions().ToArray();

            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(enumerator);

            return objects;
        }
    }
    public class AudioSession : IDisposable
    {
        #region Members

        private readonly ISimpleAudioVolume volInterface;
        private readonly Process proc;
        private bool disposedValue;

        #endregion Members

        #region Properties

        /// <summary>
        /// Get or set the volume of this session, using a range of (0.0-1.0).
        /// </summary>
        public float Volume
        {
            get
            {
                int rc = volInterface.GetMasterVolume(out float volume);
                if (rc != ReturnCode.S_OK)
                    throw new COMException($"Failed to retrieve current volume of process [{proc.Id}]: \"{proc.ProcessName}\"!", new COMException($"ISimpleAudioVolume.GetMasterVolume() returned {rc}."));
                return volume;
            }
            set
            {
                if (value < 0f || value > 1f)
                    throw new ArgumentException($"Invalid Volume: {value}, must be within (0.0 - 1.0)");

                Guid guid = Guid.Empty;
                int rc = volInterface.SetMasterVolume(value, ref guid);
                if (rc != ReturnCode.S_OK)
                    throw new COMException($"Failed to set volume of process [{proc.Id}]: \"{proc.ProcessName}\"!", new COMException($"ISimpleAudioVolume.SetMasterVolume({value}) returned {rc}."));
            }
        }
        /// <summary>
        /// Get or set the volume of this session, using a range of (0.0-100.0).
        /// </summary>
        public float VolumeFullRange
        {
            get => Volume * 100;
            set
            {
                if (value < 0f || value > 100f)
                    throw new ArgumentException($"Invalid Volume: {value}, must be within (0.0 - 100.0)");
                Volume = value / 100;
            }
        }
        /// <summary>
        /// Get or set the mute state of this session.
        /// </summary>
        public bool Mute
        {
            get
            {
                int rc = volInterface.GetMute(out bool mute);
                if (rc != ReturnCode.S_OK)
                    throw new COMException($"Failed to retrieve current mute status of process [{proc.Id}]: \"{proc.ProcessName}\"!", new COMException($"ISimpleAudioVolume.GetMute() returned {rc}."));
                return mute;
            }
            set
            {
                Guid guid = Guid.Empty;
                int rc = volInterface.SetMute(value, ref guid);
                if (rc != ReturnCode.S_OK)
                    throw new COMException($"Failed to set mute status of process [{proc.Id}]: \"{proc.ProcessName}\"!", new COMException($"ISimpleAudioVolume.SetMute({value}) returned {rc}."));
            }
        }
        public string ProcessName { get => proc.ProcessName; }
        public int PID { get => proc.Id; }

        #endregion Properties

        #region Methods

        private float ModVolume(float amount)
        {
            float
                current = Volume,
                final = current + amount;

            if (final < 0f)
                final = 0f;
            else if (final > 1f)
                final = 1f;

            return Volume = final;
        }

        public float VolumeUp(float amount, bool fullRange = true) => ModVolume((fullRange ? amount / 100 : amount));
        public float VolumeDown(float amount, bool fullRange = true) => ModVolume(-(fullRange ? amount / 100 : amount));

        #endregion Methods

        #region ObjectInternals
        public AudioSession(ISimpleAudioVolume audioInterface, Process p)
        {
            volInterface = audioInterface;
            proc = p;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    proc.Dispose();
                    Marshal.ReleaseComObject(volInterface);
                }
                disposedValue = true;
            }
        }

        ~AudioSession()
        {
            Dispose(disposing: false);
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion ObjectInternals
    }
    public class AudioSessionList : IList<AudioSession>
    {
        /// <summary>
        /// Static function that retrieves a list of all currently active AudioSession instances.
        /// You can optionally include an IMMDevice to retrieve audio sessions for that device specifically.
        /// </summary>
        /// <param name="dev">Optional IMMDevice to retrieve sessions for.</param>
        /// <returns>
        /// AudioSession List
        /// </returns>
        private static List<AudioSession> GetAllAudioSessions(IMMDevice? dev = null)
        {
            List<AudioSession> l = new();
            foreach (IAudioSessionControl2 session in API.GetVolumeObjectsForDevice(dev))
            {
                if (session == null)
                    continue;

                if (session.GetProcessId(out int pid) == ReturnCode.S_OK)
                {
                    l.Add(new AudioSession((ISimpleAudioVolume)session, Process.GetProcessById(pid)));
                }
                else
                {

                }

                // cleanup the orphaned session
                Marshal.ReleaseComObject(session);
            }
            return l;
        }

        public AudioSessionList()
        {
            sessions = GetAllAudioSessions();
        }

        private List<AudioSession> sessions;

        public int Count => ((ICollection<AudioSession>)sessions).Count;

        public bool IsReadOnly => ((ICollection<AudioSession>)sessions).IsReadOnly;

        public AudioSession this[int index] { get => ((IList<AudioSession>)sessions)[index]; set => ((IList<AudioSession>)sessions)[index] = value; }

        /// <summary>
        /// Refreshes the sessions list.
        /// This function should be called before using other AudioSessionList functions to ensure that there aren't any unassociated processes in the list.
        /// </summary>
        public void UpdateSessions()
        {
            sessions = GetAllAudioSessions();
        }

        /// <summary>
        /// Retrieve an audio session by its name
        /// Make sure you call the UpdateSessions() function first.
        /// </summary>
        /// <param name="name">The name of the target process.</param>
        /// <param name="strcomp">The type of string comparison to perform.</param>
        /// <returns>AudioSession?</returns>
        public AudioSession? GetSession(string name, StringComparison strcomp = StringComparison.Ordinal) => sessions.FirstOrDefault(s => s != null && s.ProcessName.Equals(name, strcomp), null);
        /// <summary>
        /// Retrieve an audio session by its PID number.
        /// Make sure you call the UpdateSessions() function first.
        /// </summary>
        /// <param name="id">The PID number of the target process.</param>
        /// <returns>AudioSession?</returns>
        public AudioSession? GetSession(int id) => sessions.FirstOrDefault(s => s != null && s.PID == id, null);

        /// <summary>
        /// Retrieve a list of the process names of all currently active audio sessions.
        /// Make sure you call the UpdateSessions() function first.
        /// </summary>
        /// <returns>string[]</returns>
        public string[] GetAllSessionNames()
        {
            List<string> l = new();
            foreach (AudioSession s in sessions)
                l.Add(s.ProcessName);
            return l.ToArray();
        }
        /// <summary>
        /// Retrieve a list of the PID numbers of all currently active audio sessions.
        /// Make sure you call the UpdateSessions() function first.
        /// </summary>
        /// <returns>int[]</returns>
        public int[] GetAllSessionPIDs()
        {
            List<int> l = new();
            foreach (AudioSession s in sessions)
                l.Add(s.PID);
            return l.ToArray();
        }

        #region IList
        public int IndexOf(AudioSession item)
        {
            return ((IList<AudioSession>)sessions).IndexOf(item);
        }

        public void Insert(int index, AudioSession item)
        {
            ((IList<AudioSession>)sessions).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<AudioSession>)sessions).RemoveAt(index);
        }

        public void Add(AudioSession item)
        {
            ((ICollection<AudioSession>)sessions).Add(item);
        }

        public void Clear()
        {
            ((ICollection<AudioSession>)sessions).Clear();
        }

        public bool Contains(AudioSession item)
        {
            return ((ICollection<AudioSession>)sessions).Contains(item);
        }

        public void CopyTo(AudioSession[] array, int arrayIndex)
        {
            ((ICollection<AudioSession>)sessions).CopyTo(array, arrayIndex);
        }

        public bool Remove(AudioSession item)
        {
            return ((ICollection<AudioSession>)sessions).Remove(item);
        }

        public IEnumerator<AudioSession> GetEnumerator()
        {
            return ((IEnumerable<AudioSession>)sessions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)sessions).GetEnumerator();
        }
        #endregion IList
    }
}
