using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using System.Runtime.InteropServices;
using System.Collections;

namespace AudioAPI
{
    public class AudioProcess2List : ICollection<AudioProcess2>, IEnumerable<AudioProcess2>, IEnumerable, IList<AudioProcess2>, IReadOnlyCollection<AudioProcess2>, IReadOnlyList<AudioProcess2>, ICollection, IList
    {
        private readonly List<AudioProcess2> _list;

        public AudioProcess2List(List<AudioProcess2> list)
        {
            _list = list;
        }
        public AudioProcess2List()
        {
            _list = new();
            Reload();
        }

        public void Reload()
        {
            if (_list == null) throw new ArgumentNullException(nameof(_list));

            _list.Clear();

            IMMDeviceEnumerator devEnum = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            devEnum.GetDefaultAudioEndpoint(EDataFlow.ERender, ERole.EMultimedia, out IMMDevice device);

            Guid GUID = typeof(IAudioSessionManager2).GUID;
            device.Activate(ref GUID, 0, IntPtr.Zero, out object o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            mgr.GetSessionEnumerator(out IAudioSessionEnumerator enumerator);
            List<IAudioSessionControl2> Sessions = enumerator.GetAllSessions();

            foreach (IAudioSessionControl2 session in Sessions)
            {
                if (session.GetProcessId(out int pid) == 0)
                {
                    _list.Add(new AudioProcess2(pid));
                }
            }

            Marshal.ReleaseComObject(enumerator);
            Marshal.ReleaseComObject(devEnum);
            Marshal.ReleaseComObject(mgr);
        }


        public AudioProcess2 this[int index] => ((IReadOnlyList<AudioProcess2>)_list)[index];

        object? IList.this[int index] { get => ((IList)_list)[index]; set => ((IList)_list)[index] = value; }

        public int Count => ((ICollection<AudioProcess2>)_list).Count;

        public bool IsReadOnly => ((ICollection<AudioProcess2>)_list).IsReadOnly;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        AudioProcess2 IList<AudioProcess2>.this[int index] { get => ((IList<AudioProcess2>)_list)[index]; set => ((IList<AudioProcess2>)_list)[index] = value; }

        public void Add(AudioProcess2 item)
        {
            ((ICollection<AudioProcess2>)_list).Add(item);
        }

        public int Add(object? value)
        {
            return ((IList)_list).Add(value);
        }

        public void Clear()
        {
            ((ICollection<AudioProcess2>)_list).Clear();
        }

        public bool Contains(AudioProcess2 item)
        {
            return ((ICollection<AudioProcess2>)_list).Contains(item);
        }

        public bool Contains(object? value)
        {
            return ((IList)_list).Contains(value);
        }

        public void CopyTo(AudioProcess2[] array, int arrayIndex)
        {
            ((ICollection<AudioProcess2>)_list).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public IEnumerator<AudioProcess2> GetEnumerator()
        {
            return ((IEnumerable<AudioProcess2>)_list).GetEnumerator();
        }

        public int IndexOf(object? value)
        {
            return ((IList)_list).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_list).Insert(index, value);
        }

        public bool Remove(AudioProcess2 item)
        {
            return ((ICollection<AudioProcess2>)_list).Remove(item);
        }

        public void Remove(object? value)
        {
            ((IList)_list).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)_list).RemoveAt(index);
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
            return ((global::System.Collections.IEnumerable)_list).GetEnumerator();
        }

        public int IndexOf(AudioProcess2 item)
        {
            return ((IList<AudioProcess2>)_list).IndexOf(item);
        }

        public void Insert(int index, AudioProcess2 item)
        {
            ((IList<AudioProcess2>)_list).Insert(index, item);
        }
    }
}
