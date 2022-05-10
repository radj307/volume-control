using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace HotkeyLib
{
    public class KeysList : ICollection<Keys>, IEnumerable<Keys>, IEnumerable, IList<Keys>, IReadOnlyCollection<Keys>, IReadOnlyList<Keys>, ICollection, IList
    {
        /// <inheritdoc/>
        public Keys this[int index] { get => ((IList<Keys>)Keys)[index]; set => ((IList<Keys>)Keys)[index] = value; }
        /// <inheritdoc/>
        object? IList.this[int index] { get => ((IList)Keys)[index]; set => ((IList)Keys)[index] = value; }

        public List<Keys> Keys { get; set; } = KeysBlacklist.GetWhitelistedKeys();

        /// <inheritdoc/>
        public int Count => ((ICollection<Keys>)Keys).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<Keys>)Keys).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)Keys).IsSynchronized;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)Keys).SyncRoot;

        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)Keys).IsFixedSize;

        /// <inheritdoc/>
        public void Add(Keys item) => ((ICollection<Keys>)Keys).Add(item);
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)Keys).Add(value);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<Keys>)Keys).Clear();
        /// <inheritdoc/>
        public bool Contains(Keys item) => ((ICollection<Keys>)Keys).Contains(item);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)Keys).Contains(value);
        /// <inheritdoc/>
        public void CopyTo(Keys[] array, int arrayIndex) => ((ICollection<Keys>)Keys).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)Keys).CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator<Keys> GetEnumerator() => ((IEnumerable<Keys>)Keys).GetEnumerator();
        /// <inheritdoc/>
        public int IndexOf(Keys item) => ((IList<Keys>)Keys).IndexOf(item);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)Keys).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, Keys item) => ((IList<Keys>)Keys).Insert(index, item);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)Keys).Insert(index, value);
        /// <inheritdoc/>
        public bool Remove(Keys item) => ((ICollection<Keys>)Keys).Remove(item);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)Keys).Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<Keys>)Keys).RemoveAt(index);
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();
    }
}
