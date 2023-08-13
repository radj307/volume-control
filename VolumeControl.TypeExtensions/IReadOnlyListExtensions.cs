namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="IReadOnlyList{T}"/> interface.
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Returns the index of the first occurrence of <paramref name="item"/> within the given range in this list.
        /// The list is searched forwards from beginning to end.
        /// The elements of the list are compared to the given value using the <see cref="object.Equals(object?)"/> method.
        /// </summary>
        /// <typeparam name="T">The type of object in the <see cref="IReadOnlyList{T}"/>.</typeparam>
        /// <param name="list">(implicit) The <see cref="IReadOnlyList{T}"/> to search.</param>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="index">The starting index of the search.</param>
        /// <param name="count">The number of elements to search.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item, int index, int count)
        {
            if (index > list.Count)
                throw new ArgumentOutOfRangeException($"Index {index} is out of range for the given list with size {list.Count}!");
            if (item is not null)
            {
                for (int i = index, j = 0; j < count && i < list.Count; ++i, ++j)
                {
                    if (item.Equals(list[i]))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Returns the index of the first occurrence of <paramref name="item"/> within the given range in this list.
        /// The list is searched forwards from beginning to end.
        /// The elements of the list are compared to the given value using the <see cref="object.Equals(object?)"/> method.
        /// </summary>
        /// <typeparam name="T">The type of object in the <see cref="IReadOnlyList{T}"/>.</typeparam>
        /// <param name="list">(implicit) The <see cref="IReadOnlyList{T}"/> to search.</param>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="index">The starting index of the search.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item, int index)
        {
            if (index > list.Count)
                throw new ArgumentOutOfRangeException($"Index {index} is out of range for the given list with size {list.Count}!");
            if (item is not null)
            {
                for (int i = index; i < list.Count; ++i)
                {
                    if (item.Equals(list[i]))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Returns the index of the first occurrence of <paramref name="item"/> in this list.
        /// The list is searched forwards from beginning to end.
        /// The elements of the list are compared to the given value using the <see cref="object.Equals(object?)"/> method.
        /// </summary>
        /// <typeparam name="T">The type of object in the <see cref="IReadOnlyList{T}"/>.</typeparam>
        /// <param name="list">(implicit) The <see cref="IReadOnlyList{T}"/> to search.</param>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList{T}"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise -1.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            if (item is not null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (item.Equals(list[i]))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
