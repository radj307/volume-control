using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace VolumeControl.Helpers
{
    public class KeyOptions : ICollection<Key>, IEnumerable<Key>, IEnumerable, IList<Key>, IReadOnlyCollection<Key>, IReadOnlyList<Key>, ICollection, IList
    {
        public List<Key> Keys = Enum.GetValues<Key>().ToList();

        public Key this[int index] { get => ((IList<Key>)Keys)[index]; set => ((IList<Key>)Keys)[index] = value; }
        object? IList.this[int index] { get => ((IList)Keys)[index]; set => ((IList)Keys)[index] = value; }

        public int Count => ((ICollection<Key>)Keys).Count;

        public bool IsReadOnly => ((ICollection<Key>)Keys).IsReadOnly;

        public bool IsSynchronized => ((ICollection)Keys).IsSynchronized;

        public object SyncRoot => ((ICollection)Keys).SyncRoot;

        public bool IsFixedSize => ((IList)Keys).IsFixedSize;

        public void Add(Key item) => ((ICollection<Key>)Keys).Add(item);
        public int Add(object? value) => ((IList)Keys).Add(value);
        public void Clear() => ((ICollection<Key>)Keys).Clear();
        public bool Contains(Key item) => ((ICollection<Key>)Keys).Contains(item);
        public bool Contains(object? value) => ((IList)Keys).Contains(value);
        public void CopyTo(Key[] array, int arrayIndex) => ((ICollection<Key>)Keys).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)Keys).CopyTo(array, index);
        public IEnumerator<Key> GetEnumerator() => ((IEnumerable<Key>)Keys).GetEnumerator();
        public int IndexOf(Key item) => ((IList<Key>)Keys).IndexOf(item);
        public int IndexOf(object? value) => ((IList)Keys).IndexOf(value);
        public void Insert(int index, Key item) => ((IList<Key>)Keys).Insert(index, item);
        public void Insert(int index, object? value) => ((IList)Keys).Insert(index, value);
        public bool Remove(Key item) => ((ICollection<Key>)Keys).Remove(item);
        public void Remove(object? value) => ((IList)Keys).Remove(value);
        public void RemoveAt(int index) => ((IList<Key>)Keys).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();
    }
}
