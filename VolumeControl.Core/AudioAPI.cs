using AudioAPI;
using AudioAPI.Interfaces;
using AudioAPI.WindowsAPI.Audio.MMDeviceAPI;
using System.Collections;

namespace VolumeControl.Core
{
    public class BindableAudioSessionList : ICollection<AudioSession>, IEnumerable<AudioSession>, IEnumerable, IList<AudioSession>, IReadOnlyCollection<AudioSession>, IReadOnlyList<AudioSession>, ICollection, IList
    {
        public List<AudioSession> List = new();

        public AudioSession this[int index] { get => ((IList<AudioSession>)List)[index]; set => ((IList<AudioSession>)List)[index] = value; }
        object? IList.this[int index] { get => ((IList)List)[index]; set => ((IList)List)[index] = value; }

        public int Count => ((ICollection<AudioSession>)List).Count;

        public bool IsReadOnly => ((ICollection<AudioSession>)List).IsReadOnly;

        public bool IsSynchronized => ((ICollection)List).IsSynchronized;

        public object SyncRoot => ((ICollection)List).SyncRoot;

        public bool IsFixedSize => ((IList)List).IsFixedSize;

        public void Add(AudioSession item) => ((ICollection<AudioSession>)List).Add(item);
        public int Add(object? value) => ((IList)List).Add(value);
        public void Clear() => ((ICollection<AudioSession>)List).Clear();
        public bool Contains(AudioSession item) => ((ICollection<AudioSession>)List).Contains(item);
        public bool Contains(object? value) => ((IList)List).Contains(value);
        public void CopyTo(AudioSession[] array, int arrayIndex) => ((ICollection<AudioSession>)List).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);
        public IEnumerator<AudioSession> GetEnumerator() => ((IEnumerable<AudioSession>)List).GetEnumerator();
        public int IndexOf(AudioSession item) => ((IList<AudioSession>)List).IndexOf(item);
        public int IndexOf(object? value) => ((IList)List).IndexOf(value);
        public void Insert(int index, AudioSession item) => ((IList<AudioSession>)List).Insert(index, item);
        public void Insert(int index, object? value) => ((IList)List).Insert(index, value);
        public bool Remove(AudioSession item) => ((ICollection<AudioSession>)List).Remove(item);
        public void Remove(object? value) => ((IList)List).Remove(value);
        public void RemoveAt(int index) => ((IList<AudioSession>)List).RemoveAt(index);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable)List).GetEnumerator();
    }
    public class AudioAPI
    {
        public AudioAPI()
        {
            Devices = new();
            Sessions = new();
            _selectedDevice = NullDevice;
            SelectedSession = NullSession;

            RefreshDevices();
            _selectedDevice = DefaultDevice;
            RefreshSessions();
        }

        public static readonly VirtualAudioDevice NullDevice = new();
        public static readonly VirtualAudioProcess NullSession = new();
        public static readonly AudioDevice DefaultDevice = new(Volume.GetDefaultDevice());


        public readonly List<AudioDevice> Devices;
        private IAudioDevice _selectedDevice;
        public IAudioDevice SelectedDevice
        {
            get => _selectedDevice;
            private set
            {
                Sessions.Clear();
                _selectedDevice = value;
                RefreshSessions();
            }
        }


        public readonly BindableAudioSessionList Sessions;
        public IProcess SelectedSession { get; private set; }

        public void RefreshDevices()
        {
            var devices = Volume.GetAllDevices();
            var sel = SelectedDevice;

            // remove all devices that aren't in the new list. (exited/stopped)
            Devices.RemoveAll(dev => !devices.Any(d => d.Equals(dev)));
            // add all devices that aren't in the current list. (new)
            Devices.AddRange(devices.Where(dev => !Devices.Any(d => d.Equals(dev))));

            if (sel != null && sel != NullDevice && !Devices.Any(d => d.Equals(sel)))
            {
                SelectedDevice = NullDevice;
            }
        }

        public void RefreshSessions()
        {
            var sessions = Volume.GetAllSessions();
            var sel = SelectedSession;

            // remove all sessions that aren't in the new list. (exited/stopped)
            Sessions.List.RemoveAll(session => !sessions.Any(s => s.Equals(session)));
            // add all sessions that aren't in the current list. (new)
            Sessions.List.AddRange(sessions.Where(session => !Sessions.Any(s => s.Equals(session))));

            if (sel != NullSession && !Sessions.Contains(sel))
            {
                SelectedSession = NullSession;
            }
        }
    }
}
