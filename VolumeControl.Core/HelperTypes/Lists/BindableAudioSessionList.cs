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
        /// <inheritdoc/>
        public AudioSession this[int index] { get => ((IList<AudioSession>)List)[index]; set => ((IList<AudioSession>)List)[index] = value; }
        /// <inheritdoc/>
        object? IList.this[int index] { get => ((IList)List)[index]; set => ((IList)List)[index] = value; }

        /// <inheritdoc/>
        public int Count => ((ICollection<AudioSession>)List).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<AudioSession>)List).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)List).IsSynchronized;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)List).SyncRoot;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Add(AudioSession item) => ((ICollection<AudioSession>)List).Add(item);
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)List).Add(value);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<AudioSession>)List).Clear();
        /// <inheritdoc/>
        public bool Contains(AudioSession item) => ((ICollection<AudioSession>)List).Contains(item);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)List).Contains(value);
        /// <inheritdoc/>
        public void CopyTo(AudioSession[] array, int arrayIndex) => ((ICollection<AudioSession>)List).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)List).CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator<AudioSession> GetEnumerator() => ((IEnumerable<AudioSession>)List).GetEnumerator();
        /// <inheritdoc/>
        public int IndexOf(AudioSession item) => ((IList<AudioSession>)List).IndexOf(item);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)List).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, AudioSession item) => ((IList<AudioSession>)List).Insert(index, item);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)List).Insert(index, value);
        /// <inheritdoc/>
        public bool Remove(AudioSession item) => ((ICollection<AudioSession>)List).Remove(item);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)List).Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<AudioSession>)List).RemoveAt(index);
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)List).GetEnumerator();
        #endregion InterfaceImplementation
    }
}
