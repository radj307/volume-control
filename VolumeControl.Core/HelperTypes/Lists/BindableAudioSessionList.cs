using AudioAPI.Objects;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VolumeControl.Core.HelperTypes.Lists
{
    public class BindableAudioSessionList : ICollection<AudioSession>, IEnumerable<AudioSession>, IEnumerable, IList<AudioSession>, IReadOnlyCollection<AudioSession>, IReadOnlyList<AudioSession>, ICollection, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constructors
        public BindableAudioSessionList() { }
        public BindableAudioSessionList(List<AudioSession> sessions)
        {
            (List = new()).AddRange(sessions);
        }
        #endregion Constructors

        #region Fields
        public ObservableList<AudioSession> List = new();
        #endregion Fields

        #region InterfaceImplementation
        public AudioSession this[int index] { get => ((IList<AudioSession>)List)[index]; set => ((IList<AudioSession>)List)[index] = value; }
        object? IList.this[int index] { get => ((IList)List)[index]; set => ((IList)List)[index] = value; }

        public int Count => ((ICollection<AudioSession>)List).Count;

        public bool IsReadOnly => ((ICollection<AudioSession>)List).IsReadOnly;

        public bool IsSynchronized => ((ICollection)List).IsSynchronized;

        public object SyncRoot => ((ICollection)List).SyncRoot;

        public bool IsFixedSize => ((IList)List).IsFixedSize;

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add
            {
                ((INotifyCollectionChanged)List).CollectionChanged += value;
            }

            remove
            {
                ((INotifyCollectionChanged)List).CollectionChanged -= value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                ((INotifyPropertyChanged)List).PropertyChanged += value;
            }

            remove
            {
                ((INotifyPropertyChanged)List).PropertyChanged -= value;
            }
        }

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
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();
        #endregion InterfaceImplementation
    }
}
