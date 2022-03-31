using System.Collections;

namespace Core.Keyboard
{
    public class KeysList : ICollection<Keys>, IEnumerable<Keys>, IEnumerable, IList<Keys>, IReadOnlyCollection<Keys>, IReadOnlyList<Keys>, ICollection, IList
    {
        #region Constructors
        public KeysList(KeysList o)
        {
            _list = o._list;
        }
        public KeysList(List<Keys> list)
        {
            _list = list;
        }
        public KeysList()
        {
            _list = new();
        }
        #endregion Constructors

        #region Members
        private readonly List<Keys> _list;
        #endregion Members

        #region Properties
        public bool IsFixedSize => ((IList)_list).IsFixedSize;

        public bool IsReadOnly => ((IList)_list).IsReadOnly;

        public int Count => ((ICollection)_list).Count;

        public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

        public object SyncRoot => ((ICollection)_list).SyncRoot;

        Keys IList<Keys>.this[int index] { get => ((IList<Keys>)_list)[index]; set => ((IList<Keys>)_list)[index] = value; }

        Keys IReadOnlyList<Keys>.this[int index] => ((IReadOnlyList<Keys>)_list)[index];

        public object? this[int index] { get => ((IList)_list)[index]; set => ((IList)_list)[index] = value; }
        #endregion Properties

        #region Methods
        public int Add(object? value)
        {
            return ((IList)_list).Add(value);
        }

        public void Clear()
        {
            ((IList)_list).Clear();
        }

        public bool Contains(object? value)
        {
            return ((IList)_list).Contains(value);
        }

        public int IndexOf(object? value)
        {
            return ((IList)_list).IndexOf(value);
        }

        public void Insert(int index, object? value)
        {
            ((IList)_list).Insert(index, value);
        }

        public void Remove(object? value)
        {
            ((IList)_list).Remove(value);
        }

        public void RemoveAt(int index)
        {
            ((IList)_list).RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        IEnumerator<Keys> IEnumerable<Keys>.GetEnumerator()
        {
            return ((IEnumerable<Keys>)_list).GetEnumerator();
        }

        public int IndexOf(Keys item)
        {
            return ((IList<Keys>)_list).IndexOf(item);
        }

        public void Insert(int index, Keys item)
        {
            ((IList<Keys>)_list).Insert(index, item);
        }

        public void Add(Keys item)
        {
            ((ICollection<Keys>)_list).Add(item);
        }

        public bool Contains(Keys item)
        {
            return ((ICollection<Keys>)_list).Contains(item);
        }

        public void CopyTo(Keys[] array, int arrayIndex)
        {
            ((ICollection<Keys>)_list).CopyTo(array, arrayIndex);
        }

        public bool Remove(Keys item)
        {
            return ((ICollection<Keys>)_list).Remove(item);
        }
        #endregion Methods
    }
}
