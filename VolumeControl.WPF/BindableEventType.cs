using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Log.Enum;

namespace VolumeControl.WPF
{
    /// <summary>
    /// Provides the source for a WPF list component's ItemsSource property to expose a readable and writable interface for enum types with <see cref="FlagsAttribute"/>.
    /// </summary>
    public class BindableEventType : ICollection<BindableEventType.BindableEventTypeFlag>, IEnumerable<BindableEventType.BindableEventTypeFlag>, IEnumerable, IList<BindableEventType.BindableEventTypeFlag>, IReadOnlyCollection<BindableEventType.BindableEventTypeFlag>, IReadOnlyList<BindableEventType.BindableEventTypeFlag>, ICollection, IList, INotifyPropertyChanged
    {
        #region SubObject
        /// <summary>
        /// This represents a single flag value of the <see cref="EventType"/> enum.
        /// </summary>
        /// <remarks>This is used by <see cref="BindableEventType"/> to provide a bindable <b>ItemsSource</b> list.</remarks>
        public class BindableEventTypeFlag : INotifyPropertyChanged
        {
            public BindableEventTypeFlag(BindableEventType parent, EventType ev)
            {
                _parent = parent;
                _ev = ev;
                Name = System.Enum.GetName(typeof(EventType), _ev) ?? string.Empty;
            }
            private readonly BindableEventType _parent;
            private readonly EventType _ev;

            public event PropertyChangedEventHandler? PropertyChanged;
            protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

            public string Name { get; }
            public bool IsSet
            {
                get => (_parent.Value & _ev) != 0;
                set
                {
                    if (value)
                        _parent.Value |= _ev;
                    else
                        _parent.Value &= ~_ev;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion SubObject

        #region Constructors
        public BindableEventType()
        {
            _value = EventType.NONE;
            InitializeOptions();
        }
        public BindableEventType(EventType ev)
        {
            _value = ev;
            InitializeOptions();
        }
        #endregion Constructors

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Indexers
        /// <inheritdoc/>
        public BindableEventTypeFlag this[int index] { get => ((IList<BindableEventTypeFlag>)Options)[index]; set => ((IList<BindableEventTypeFlag>)Options)[index] = value; }
        /// <inheritdoc/>
        object? IList.this[int index] { get => ((IList)Options)[index]; set => ((IList)Options)[index] = value; }
        #endregion Indexers

        #region Fields
        private EventType _value;
        #endregion Fields

        #region Properties
        public EventType Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }
        public List<BindableEventTypeFlag> Options { get; } = new();

        /// <inheritdoc/>
        public int Count => ((ICollection<BindableEventTypeFlag>)Options).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<BindableEventTypeFlag>)Options).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)Options).IsSynchronized;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)Options).SyncRoot;

        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)Options).IsFixedSize;
        #endregion Properties

        #region Methods
        /// <summary>
        /// This method is responsible for populating the <see cref="Options"/> list, as well as filtering out unwanted event types like <see cref="EventType.NONE"/>, <see cref="EventType.ALL_EXCEPT_DEBUG"/>, and <see cref="EventType.ALL"/>.
        /// </summary>
        private void InitializeOptions()
        {
            foreach (EventType e in (EventType[])System.Enum.GetValues(typeof(EventType)))
            {
                if (e != EventType.NONE && e != EventType.ALL && e != EventType.ALL_EXCEPT_DEBUG)
                    Options.Add(new(this, e));
            }
        }
        /// <inheritdoc/>
        public void Add(BindableEventTypeFlag item) => ((ICollection<BindableEventTypeFlag>)Options).Add(item);
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)Options).Add(value);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<BindableEventTypeFlag>)Options).Clear();
        /// <inheritdoc/>
        public bool Contains(BindableEventTypeFlag item) => ((ICollection<BindableEventTypeFlag>)Options).Contains(item);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)Options).Contains(value);
        /// <inheritdoc/>
        public void CopyTo(BindableEventTypeFlag[] array, int arrayIndex) => ((ICollection<BindableEventTypeFlag>)Options).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)Options).CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator<BindableEventTypeFlag> GetEnumerator() => ((IEnumerable<BindableEventTypeFlag>)Options).GetEnumerator();
        /// <inheritdoc/>
        public int IndexOf(BindableEventTypeFlag item) => ((IList<BindableEventTypeFlag>)Options).IndexOf(item);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)Options).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, BindableEventTypeFlag item) => ((IList<BindableEventTypeFlag>)Options).Insert(index, item);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)Options).Insert(index, value);
        /// <inheritdoc/>
        public bool Remove(BindableEventTypeFlag item) => ((ICollection<BindableEventTypeFlag>)Options).Remove(item);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)Options).Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<BindableEventTypeFlag>)Options).RemoveAt(index);
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Options).GetEnumerator();
        #endregion Methods
    }
}
