using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Collections;
using System.Runtime.InteropServices;

namespace AudioAPI.Forms
{
    public class InterfaceAudioProcessList : ICollection<IAudioProcess>, IEnumerable<IAudioProcess>, IEnumerable, IList<IAudioProcess>, IReadOnlyCollection<IAudioProcess>, IReadOnlyList<IAudioProcess>, ICollection, IList
    {
        public InterfaceAudioProcessList(List<IAudioProcess> list)
        {
            _audioProcesses = list;
        }
        public InterfaceAudioProcessList()
        {
            _audioProcesses = new();
            Reload();
        }

        public void Reload()
        {
            if (_audioProcesses == null) throw new ArgumentNullException(nameof(_audioProcesses));

            _audioProcesses.Clear();

            IMMDeviceEnumerator devEnum = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            devEnum.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);

            Guid GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);
            List<IAudioSessionControl2> Sessions = enumerator.GetAllSessions();

            foreach (IAudioSessionControl2 session in Sessions)
            {
                _audioProcesses.Add(new AudioProcess(session));
            }

            Marshal.ReleaseComObject(enumerator);
            Marshal.ReleaseComObject(devEnum);
            Marshal.ReleaseComObject(mgr);
        }

        private readonly List<IAudioProcess> _audioProcesses;

        public IAudioProcess this[int index] { get => ((IList<IAudioProcess>)_audioProcesses)[index]; set => ((IList<IAudioProcess>)_audioProcesses)[index] = value; }
        object? IList.this[int index] { get => ((IList)_audioProcesses)[index]; set => ((IList)_audioProcesses)[index] = value; }

        public int Count => ((ICollection<IAudioProcess>)_audioProcesses).Count;

        public bool IsReadOnly => ((ICollection<IAudioProcess>)_audioProcesses).IsReadOnly;

        public bool IsSynchronized => ((ICollection)_audioProcesses).IsSynchronized;

        public object SyncRoot => ((ICollection)_audioProcesses).SyncRoot;

        public bool IsFixedSize => ((IList)_audioProcesses).IsFixedSize;

        public void Add(IAudioProcess item)
        {
            ((ICollection<IAudioProcess>)_audioProcesses).Add(item);
        }

        public int Add(object? value)
        {
            return ((IList)_audioProcesses).Add(value);
        }

        public void Clear()
        {
            ((ICollection<IAudioProcess>)_audioProcesses).Clear();
        }

        public bool Contains(IAudioProcess item)
        {
            return ((ICollection<IAudioProcess>)_audioProcesses).Contains(item);
        }

        public bool Contains(object? value)
        {
            return ((IList)_audioProcesses).Contains(value);
        }

        public void CopyTo(IAudioProcess[] array, int arrayIndex)
        {
            ((ICollection<IAudioProcess>)_audioProcesses).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_audioProcesses).CopyTo(array, index);
        }

        public IEnumerator<IAudioProcess> GetEnumerator()
        {
            return ((IEnumerable<IAudioProcess>)_audioProcesses).GetEnumerator();
        }

        public int IndexOf(IAudioProcess item)
        {
            return ((IList<IAudioProcess>)_audioProcesses).IndexOf(item);
        }

        public int IndexOf(object? value)
        {
            return ((IList)_audioProcesses).IndexOf(value);
        }

        public void Insert(int index, IAudioProcess item)
        {
            ((IList<IAudioProcess>)_audioProcesses).Insert(index, item);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_audioProcesses).Insert(index, value);
        }

        public bool Remove(IAudioProcess item)
        {
            return ((ICollection<IAudioProcess>)_audioProcesses).Remove(item);
        }

        public void Remove(object? value)
        {
            ((IList)_audioProcesses).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList<IAudioProcess>)_audioProcesses).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_audioProcesses).GetEnumerator();
        }
    }
}
