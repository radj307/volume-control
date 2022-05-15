using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Log.Enum;

namespace VolumeControl.WPF
{
    public class BindableEventTypeEnumeration : INotifyPropertyChanged
    {
        public BindableEventTypeEnumeration(BindableEventType parent, EventType ev)
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
    public class BindableEventType : ICollection<BindableEventTypeEnumeration>, IEnumerable<BindableEventTypeEnumeration>, IEnumerable, IList<BindableEventTypeEnumeration>, IReadOnlyCollection<BindableEventTypeEnumeration>, IReadOnlyList<BindableEventTypeEnumeration>, ICollection, IList, INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));

        public BindableEventTypeEnumeration this[int index] { get => ((IList<BindableEventTypeEnumeration>)Options)[index]; set => ((IList<BindableEventTypeEnumeration>)Options)[index] = value; }
        object? IList.this[int index] { get => ((IList)Options)[index]; set => ((IList)Options)[index] = value; }

        private EventType _value;
        public EventType Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }
        public List<BindableEventTypeEnumeration> Options { get; } = new();

        public int Count => ((ICollection<BindableEventTypeEnumeration>)Options).Count;

        public bool IsReadOnly => ((ICollection<BindableEventTypeEnumeration>)Options).IsReadOnly;

        public bool IsSynchronized => ((ICollection)Options).IsSynchronized;

        public object SyncRoot => ((ICollection)Options).SyncRoot;

        public bool IsFixedSize => ((IList)Options).IsFixedSize;

        public void Add(BindableEventTypeEnumeration item) => ((ICollection<BindableEventTypeEnumeration>)Options).Add(item);
        public int Add(object? value) => ((IList)Options).Add(value);
        public void Clear() => ((ICollection<BindableEventTypeEnumeration>)Options).Clear();
        public bool Contains(BindableEventTypeEnumeration item) => ((ICollection<BindableEventTypeEnumeration>)Options).Contains(item);
        public bool Contains(object? value) => ((IList)Options).Contains(value);
        public void CopyTo(BindableEventTypeEnumeration[] array, int arrayIndex) => ((ICollection<BindableEventTypeEnumeration>)Options).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)Options).CopyTo(array, index);
        public IEnumerator<BindableEventTypeEnumeration> GetEnumerator() => ((IEnumerable<BindableEventTypeEnumeration>)Options).GetEnumerator();
        public int IndexOf(BindableEventTypeEnumeration item) => ((IList<BindableEventTypeEnumeration>)Options).IndexOf(item);
        public int IndexOf(object? value) => ((IList)Options).IndexOf(value);
        public void Insert(int index, BindableEventTypeEnumeration item) => ((IList<BindableEventTypeEnumeration>)Options).Insert(index, item);
        public void Insert(int index, object? value) => ((IList)Options).Insert(index, value);
        public bool Remove(BindableEventTypeEnumeration item) => ((ICollection<BindableEventTypeEnumeration>)Options).Remove(item);
        public void Remove(object? value) => ((IList)Options).Remove(value);
        public void RemoveAt(int index) => ((IList<BindableEventTypeEnumeration>)Options).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Options).GetEnumerator();

        private void InitializeOptions()
        {
            foreach (EventType e in (EventType[])System.Enum.GetValues(typeof(EventType)))
            {
                if (e != EventType.NONE && e != EventType.ALL && e != EventType.ALL_EXCEPT_DEBUG)
                    Options.Add(new(this, e));
            }
        }
    }
}
