using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace VolumeControl.Helpers
{
    public class KeyOptions : ICollection<Key>, IEnumerable<Key>, IEnumerable, IList<Key>, IReadOnlyCollection<Key>, IReadOnlyList<Key>, ICollection, IList
    {
        /// <summary>
        /// Gets a list of all <see cref="Key"/> enumerations that do not appear in <see cref="Core.Config.KeyBlacklist"/>.
        /// </summary>
        /// <returns>List of <see cref="Key"/> enumerations.</returns>
        internal static List<Key> GetValidKeyList() => Enum.GetValues<Key>().Where(k => !Core.Config.KeyBlacklist.Contains(k)).ToList();

        public List<Key> Keys = GetValidKeyList();

        /// <inheritdoc/>
        public Key this[int index] { get => ((IList<Key>)Keys)[index]; set => ((IList<Key>)Keys)[index] = value; }
        /// <inheritdoc/>
        object? IList.this[int index] { get => ((IList)Keys)[index]; set => ((IList)Keys)[index] = value; }

        /// <inheritdoc/>
        public int Count => ((ICollection<Key>)Keys).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<Key>)Keys).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)Keys).IsSynchronized;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)Keys).SyncRoot;

        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)Keys).IsFixedSize;

        /// <inheritdoc/>
        public void Add(Key item) => ((ICollection<Key>)Keys).Add(item);
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)Keys).Add(value);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<Key>)Keys).Clear();
        /// <inheritdoc/>
        public bool Contains(Key item) => ((ICollection<Key>)Keys).Contains(item);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)Keys).Contains(value);
        /// <inheritdoc/>
        public void CopyTo(Key[] array, int arrayIndex) => ((ICollection<Key>)Keys).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)Keys).CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator<Key> GetEnumerator() => ((IEnumerable<Key>)Keys).GetEnumerator();
        /// <inheritdoc/>
        public int IndexOf(Key item) => ((IList<Key>)Keys).IndexOf(item);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)Keys).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, Key item) => ((IList<Key>)Keys).Insert(index, item);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)Keys).Insert(index, value);
        /// <inheritdoc/>
        public bool Remove(Key item) => ((ICollection<Key>)Keys).Remove(item);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)Keys).Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<Key>)Keys).RemoveAt(index);
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();
    }
}
