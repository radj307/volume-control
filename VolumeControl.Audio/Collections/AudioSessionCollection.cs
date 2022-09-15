using NAudio.CoreAudioApi;
using PropertyChanged;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using VolumeControl.TypeExtensions;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Audio.Collections
{
    /// <summary>
    /// <see cref="AudioSession"/> management container type.<br/>
    /// This is implemented as a viewmodel in the <see cref="AudioAPI"/> class.
    /// </summary>
    /// <remarks>This object does not maintain ownership of <see cref="AudioSession"/> objects; rather, it utilizes <see cref="AudioDevice.Sessions"/> to create a single binding point for the UI.<br/><br/>
    /// <b>This object cannot be instantiated externally.</b></remarks>
    public class AudioSessionCollection : INotifyCollectionChanged, INotifyPropertyChanged, IList, ICollection, IEnumerable, IList<AudioSession>, IImmutableList<AudioSession>, ICollection<AudioSession>, IEnumerable<AudioSession>, IReadOnlyList<AudioSession>, IReadOnlyCollection<AudioSession>
    {
        #region Constructor
        internal AudioSessionCollection(AudioDeviceCollection devices)
        {
            _devices = devices;
            _devices.DeviceEnabledChanged += this.HandleDeviceEnabledChanged;
            _devices.DeviceSessionCreated += this.HandleDeviceSessionCreated;
            _devices.DeviceSessionRemoved += this.HandleDeviceSessionRemoved;
        }
        #endregion Constructor

        #region Fields
        private readonly AudioDeviceCollection _devices;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Container
        /// </summary>
        [SuppressPropertyChangedWarnings]
        public ObservableImmutableList<AudioSession> Items { get; } = new();
        /// <inheritdoc/>

        public int Count => this.Items.Count;
        /// <inheritdoc/>

        public bool IsSynchronized => this.Items.IsSynchronized;
        /// <inheritdoc/>

        public object SyncRoot => this.Items.SyncRoot;
        /// <inheritdoc/>

        public bool IsFixedSize => this.Items.IsFixedSize;
        /// <inheritdoc/>

        public bool IsReadOnly => this.Items.IsReadOnly;

        /// <inheritdoc/>
        [SuppressPropertyChangedWarnings]
        public AudioSession this[int index] => this.Items[index];

        /// <inheritdoc/>
        [SuppressPropertyChangedWarnings]
        AudioSession IList<AudioSession>.this[int index] { get => ((IList<AudioSession>)this.Items)[index]; set => ((IList<AudioSession>)this.Items)[index] = value; }
        /// <inheritdoc/>
        [SuppressPropertyChangedWarnings]
        object? IList.this[int index] { get => ((IList)this.Items)[index]; set => ((IList)this.Items)[index] = value; }
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => this.Items.CollectionChanged += value;
            remove => this.Items.CollectionChanged -= value;
        }
#       pragma warning disable CS0067 // The event 'AudioSessionCollection.PropertyChanged' is never used
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
#       pragma warning restore CS0067 // The event 'AudioSessionCollection.PropertyChanged' is never used
        #endregion Events

        #region EventHandlers
        private void HandleDeviceSessionCreated(object? sender, AudioSession session)
        {
            if (sender is AudioDevice device)
            {
                if (device.Enabled)
                    _ = this.AddIfUnique(session);
            }
            else
            {
                throw new InvalidOperationException($"{nameof(HandleDeviceSessionCreated)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
            }
        }
        private void HandleDeviceSessionRemoved(object? sender, long pid) => _ = sender is AudioDevice device
                ? this.Remove(pid)
                : throw new InvalidOperationException($"{nameof(HandleDeviceSessionRemoved)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        private void HandleDeviceEnabledChanged(object? sender, bool state)
        {
            if (sender is AudioDevice device)
            {
                if (state) this.AddRangeIfUnique(device.Sessions);
                else this.RemoveAll(session => !session.PID.Equals(0) && device.Sessions.Contains(session));
                return;
            }
            throw new InvalidOperationException($"{nameof(HandleDeviceEnabledChanged)} received a sender of type '{sender?.GetType().FullName}'; expected '{typeof(AudioDevice).FullName}'");
        }
        #endregion EventHandlers

        #region Methods
        /// <summary>Performs a selective-update of the session list.</summary>
        /// <param name="devices">Any number of <see cref="AudioDevice"/> classes.</param>
        internal void RefreshFromDevices(params AudioDevice[] devices)
        {
            List<AudioSession> l = new();
            foreach (AudioDevice? device in devices)
            {
                if (device.Enabled && device.State.Equals(DeviceState.Active))
                    _ = l.AddRangeIfUnique(device.Sessions);
            }

            _ = this.RemoveAll(s => !l.Contains(s));
            _ = this.AddRangeIfUnique(l.AsEnumerable());
        }
        /// <inheritdoc cref="RefreshFromDevices(AudioDevice[])"/>
        /// <remarks>This method uses the internal reference to <see cref="AudioDeviceCollection"/> instead of given devices.</remarks>
        internal void RefreshFromDevices() => this.RefreshFromDevices(_devices.ToArray());
        /// <summary>
        /// Removes any sessions with the same process ID number as <paramref name="pid"/>.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns><see langword="true"/> when a session was successfully removed from the list; otherwise <see langword="false"/>.</returns>
        public bool Remove(long pid)
        {
            for (int i = this.Count - 1; i >= 0; --i)
            {
                if (this[i] is AudioSession session && session.PID.Equals(pid))
                {
                    this.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => this.Items.CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator GetEnumerator() => this.Items.GetEnumerator();
        /// <inheritdoc/>
        public int Add(object? value) => this.Items.Add(value);
        /// <inheritdoc/>
        public void Clear() => this.Items.Clear();
        /// <inheritdoc/>
        public bool Contains(object? value) => this.Items.Contains(value);
        /// <inheritdoc/>
        public int IndexOf(object? value) => this.Items.IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => this.Items.Insert(index, value);
        /// <inheritdoc/>
        public void Remove(object? value) => this.Items.Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => this.Items.RemoveAt(index);
        /// <inheritdoc/>
        public int IndexOf(AudioSession item) => this.Items.IndexOf(item);
        /// <inheritdoc/>
        public void Insert(int index, AudioSession item) => this.Items.Insert(index, item);
        /// <inheritdoc/>
        public void Add(AudioSession item) => this.Items.Add(item);
        /// <inheritdoc/>
        public bool Contains(AudioSession item) => this.Items.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(AudioSession[] array, int arrayIndex) => this.Items.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(AudioSession item) => this.Items.Remove(item);
        /// <inheritdoc/>
        IEnumerator<AudioSession> IEnumerable<AudioSession>.GetEnumerator() => this.Items.GetEnumerator();
        /// <inheritdoc/>
        IImmutableList<AudioSession> IImmutableList<AudioSession>.Add(AudioSession value) => this.Items.Add(value);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> AddRange(IEnumerable<AudioSession> items) => this.Items.AddRange(items);
        /// <inheritdoc/>
        IImmutableList<AudioSession> IImmutableList<AudioSession>.Clear() => this.Items.Clear();
        /// <inheritdoc/>
        public int IndexOf(AudioSession item, int index, int count, IEqualityComparer<AudioSession>? equalityComparer) => this.Items.IndexOf(item, index, count, equalityComparer);
        /// <inheritdoc/>
        IImmutableList<AudioSession> IImmutableList<AudioSession>.Insert(int index, AudioSession element) => this.Items.Insert(index, element);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> InsertRange(int index, IEnumerable<AudioSession> items) => this.Items.InsertRange(index, items);
        /// <inheritdoc/>
        public int LastIndexOf(AudioSession item, int index, int count, IEqualityComparer<AudioSession>? equalityComparer) => this.Items.LastIndexOf(item, index, count, equalityComparer);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> Remove(AudioSession value, IEqualityComparer<AudioSession>? equalityComparer) => this.Items.Remove(value, equalityComparer);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> RemoveAll(Predicate<AudioSession> match) => this.Items.RemoveAll(match);
        /// <inheritdoc/>
        IImmutableList<AudioSession> IImmutableList<AudioSession>.RemoveAt(int index) => this.Items.RemoveAt(index);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> RemoveRange(IEnumerable<AudioSession> items, IEqualityComparer<AudioSession>? equalityComparer) => this.Items.RemoveRange(items, equalityComparer);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> RemoveRange(int index, int count) => this.Items.RemoveRange(index, count);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> Replace(AudioSession oldValue, AudioSession newValue, IEqualityComparer<AudioSession>? equalityComparer) => this.Items.Replace(oldValue, newValue, equalityComparer);
        /// <inheritdoc/>
        public IImmutableList<AudioSession> SetItem(int index, AudioSession value) => this.Items.SetItem(index, value);
        #endregion Methods
    }
}
