using System.Collections;

namespace HotkeyLib
{
    /// <summary>
    /// A readonly list of Enum types that is filtered at construction time.
    /// </summary>
    /// <typeparam name="T">Enum Type</typeparam>
    public class FilteredEnumList<T> : ICollection<T>, IEnumerable<T>, IList<T> where T : Enum
    {
        #region Members

        private readonly List<T> _list;

        #endregion Members

        #region Properties

        public List<T> List => _list;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public int Count => ((ICollection<T>)_list).Count;

        public T this[int index] { get => ((IList<T>)_list)[index]; set => ((IList<T>)_list)[index] = value; }

        #endregion Properties

        #region Constructors

        public FilteredEnumList(List<T> l)
        {
            _list = l;
        }
        public FilteredEnumList(List<T> input, Func<T, bool> predicate)
        {
            _list = new();
            foreach (T it in input)
                if (predicate(it))
                    _list.Add(it);
        }

        #endregion Constructors

        #region Methods

        public void Add(T item)
        {
            ((ICollection<T>)_list).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)_list).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)_list).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)_list).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)_list).Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)_list).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)_list).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)_list).RemoveAt(index);
        }

        #endregion Methods
    }
}
