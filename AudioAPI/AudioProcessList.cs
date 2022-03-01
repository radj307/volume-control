using AudioAPI.WindowsAPI.Audio;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI.Enum;
using System.Collections;
using System.Runtime.InteropServices;

namespace AudioAPI
{
    public class AudioProcessList : ICollection<AudioProcess>, IEnumerable<AudioProcess>, IEnumerable, IList<AudioProcess>, IReadOnlyCollection<AudioProcess>, IReadOnlyList<AudioProcess>, ICollection, IList, IDisposable
    {
        public AudioProcessList(List<AudioProcess> list)
        {
            _audioProcesses = list;
        }
        public AudioProcessList()
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

        public void Add(AudioProcess item)
        {
            ((ICollection<AudioProcess>)_audioProcesses).Add(item);
        }

        public void Clear()
        {
            ((ICollection<AudioProcess>)_audioProcesses).Clear();
        }

        public bool Contains(AudioProcess item)
        {
            return ((ICollection<AudioProcess>)_audioProcesses).Contains(item);
        }

        public void CopyTo(AudioProcess[] array, int arrayIndex)
        {
            ((ICollection<AudioProcess>)_audioProcesses).CopyTo(array, arrayIndex);
        }

        public bool Remove(AudioProcess item)
        {
            return ((ICollection<AudioProcess>)_audioProcesses).Remove(item);
        }

        public IEnumerator<AudioProcess> GetEnumerator()
        {
            return ((IEnumerable<AudioProcess>)_audioProcesses).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_audioProcesses).GetEnumerator();
        }

        public int IndexOf(AudioProcess item)
        {
            return ((IList<AudioProcess>)_audioProcesses).IndexOf(item);
        }

        public void Insert(int index, AudioProcess item)
        {
            ((IList<AudioProcess>)_audioProcesses).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<AudioProcess>)_audioProcesses).RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_audioProcesses).CopyTo(array, index);
        }

        public int Add(object? value)
        {
            return ((IList)_audioProcesses).Add(value);
        }

        public bool Contains(object? value)
        {
            return ((IList)_audioProcesses).Contains(value);
        }

        public int IndexOf(object? value)
        {
            return ((IList)_audioProcesses).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_audioProcesses).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)_audioProcesses).Remove(value);
        }

        private readonly List<AudioProcess> _audioProcesses;
        private bool disposedValue;

        public int Count => ((ICollection<AudioProcess>)_audioProcesses).Count;

        public bool IsReadOnly => ((ICollection<AudioProcess>)_audioProcesses).IsReadOnly;

        public bool IsSynchronized => ((ICollection)_audioProcesses).IsSynchronized;

        public object SyncRoot => ((ICollection)_audioProcesses).SyncRoot;

        public bool IsFixedSize => ((IList)_audioProcesses).IsFixedSize;

        object? IList.this[int index] { get => ((IList)_audioProcesses)[index]; set => ((IList)_audioProcesses)[index] = value; }
        public AudioProcess this[int index] { get => ((IList<AudioProcess>)_audioProcesses)[index]; set => ((IList<AudioProcess>)_audioProcesses)[index] = value; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (AudioProcess process in _audioProcesses)
                    {
                        process.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        ~AudioProcessList()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
