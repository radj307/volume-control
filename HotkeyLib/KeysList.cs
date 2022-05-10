using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace HotkeyLib
{
    public class KeysList : ICollection<Keys>, IEnumerable<Keys>, IEnumerable, IList<Keys>, IReadOnlyCollection<Keys>, IReadOnlyList<Keys>, ICollection, IList
    {
        public Keys this[int index] { get => ((IList<Keys>)Keys)[index]; set => ((IList<Keys>)Keys)[index] = value; }
        object? IList.this[int index] { get => ((IList)Keys)[index]; set => ((IList)Keys)[index] = value; }

        public List<Keys> Keys { get; set; } = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();

        public int Count => ((ICollection<Keys>)Keys).Count;

        public bool IsReadOnly => ((ICollection<Keys>)Keys).IsReadOnly;

        public bool IsSynchronized => ((ICollection)Keys).IsSynchronized;

        public object SyncRoot => ((ICollection)Keys).SyncRoot;

        public bool IsFixedSize => ((IList)Keys).IsFixedSize;

        public void Add(Keys item) => ((ICollection<Keys>)Keys).Add(item);
        public int Add(object? value) => ((IList)Keys).Add(value);
        public void Clear() => ((ICollection<Keys>)Keys).Clear();
        public bool Contains(Keys item) => ((ICollection<Keys>)Keys).Contains(item);
        public bool Contains(object? value) => ((IList)Keys).Contains(value);
        public void CopyTo(Keys[] array, int arrayIndex) => ((ICollection<Keys>)Keys).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)Keys).CopyTo(array, index);
        public IEnumerator<Keys> GetEnumerator() => ((IEnumerable<Keys>)Keys).GetEnumerator();
        public int IndexOf(Keys item) => ((IList<Keys>)Keys).IndexOf(item);
        public int IndexOf(object? value) => ((IList)Keys).IndexOf(value);
        public void Insert(int index, Keys item) => ((IList<Keys>)Keys).Insert(index, item);
        public void Insert(int index, object? value) => ((IList)Keys).Insert(index, value);
        public bool Remove(Keys item) => ((ICollection<Keys>)Keys).Remove(item);
        public void Remove(object? value) => ((IList)Keys).Remove(value);
        public void RemoveAt(int index) => ((IList<Keys>)Keys).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();
    }
}
