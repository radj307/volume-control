using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VolumeControl.Core.Input.Enums;

namespace VolumeControl.Helpers
{
    /// <summary>
    /// Container for <see cref="EFriendlyKey"/> enum values that are shown in the UI.
    /// </summary>
    public class KeyOptions : ICollection<EFriendlyKey>, IEnumerable<EFriendlyKey>, IEnumerable, IList<EFriendlyKey>, IReadOnlyCollection<EFriendlyKey>, IReadOnlyList<EFriendlyKey>, ICollection, IList
    {
        public KeyOptions() => Keys = Enum.GetValues<EFriendlyKey>().ToList();

        public List<EFriendlyKey> Keys;

        /// <inheritdoc/>
        public EFriendlyKey this[int index] { get => ((IList<EFriendlyKey>)Keys)[index]; set => ((IList<EFriendlyKey>)Keys)[index] = value; }
        /// <inheritdoc/>
        object? IList.this[int index] { get => ((IList)Keys)[index]; set => ((IList)Keys)[index] = value; }

        /// <inheritdoc/>
        public int Count => ((ICollection<EFriendlyKey>)Keys).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<EFriendlyKey>)Keys).IsReadOnly;

        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)Keys).IsSynchronized;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)Keys).SyncRoot;

        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)Keys).IsFixedSize;

        /// <inheritdoc/>
        public void Add(EFriendlyKey item) => ((ICollection<EFriendlyKey>)Keys).Add(item);
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)Keys).Add(value);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<EFriendlyKey>)Keys).Clear();
        /// <inheritdoc/>
        public bool Contains(EFriendlyKey item) => ((ICollection<EFriendlyKey>)Keys).Contains(item);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)Keys).Contains(value);
        /// <inheritdoc/>
        public void CopyTo(EFriendlyKey[] array, int arrayIndex) => ((ICollection<EFriendlyKey>)Keys).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)Keys).CopyTo(array, index);
        /// <inheritdoc/>
        public IEnumerator<EFriendlyKey> GetEnumerator() => ((IEnumerable<EFriendlyKey>)Keys).GetEnumerator();
        /// <inheritdoc/>
        public int IndexOf(EFriendlyKey item) => ((IList<EFriendlyKey>)Keys).IndexOf(item);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)Keys).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, EFriendlyKey item) => ((IList<EFriendlyKey>)Keys).Insert(index, item);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)Keys).Insert(index, value);
        /// <inheritdoc/>
        public bool Remove(EFriendlyKey item) => ((ICollection<EFriendlyKey>)Keys).Remove(item);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)Keys).Remove(value);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<EFriendlyKey>)Keys).RemoveAt(index);
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();
    }
}
