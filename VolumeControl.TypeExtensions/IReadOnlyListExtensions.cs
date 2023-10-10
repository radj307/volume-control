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
            if (item != null)
            {
                for (int i = index, max = list.Count, j = 0; j < count && i < max; ++i, ++j)
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
            if (item != null)
            {
                for (int i = index, max = list.Count; i < max; ++i)
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
            if (item != null)
            {
                for (int i = 0, max = list.Count; i < max; ++i)
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
        /// Gets the index of the first occurrence of <paramref name="item"/> in this <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of object in the <see cref="IReadOnlyList{T}"/>.</typeparam>
        /// <param name="list">(implicit) The <see cref="IReadOnlyList{T}"/> to search.</param>
        /// <param name="item">The object to locate in the <see cref="IReadOnlyList{T}"/>.</param>
        /// <param name="index">The index of <paramref name="item"/> if found in the list; otherwise -1.</param>
        /// <returns><see langword="true"/> when <paramref name="item"/> was found in the list; otherwise <see langword="false"/>.</returns>
        public static bool IndexOf<T>(this IReadOnlyList<T> list, T item, out int index)
        {
            if (item != null)
            {
                for (int i = 0, max = list.Count; i < max; ++i)
                {
                    if (item.Equals(list[i]))
                    {
                        index = i;
                        return true;
                    }
                }
            }
            index = -1;
            return false;
        }
    }
}
