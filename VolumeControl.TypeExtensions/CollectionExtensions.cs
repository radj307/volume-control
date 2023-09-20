using System.Collections;

namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods for <see cref="ICollection{T}"/>
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds <paramref name="obj"/> to the <paramref name="collection"/> if it isn't a duplicate of any existing elements.
        /// </summary>
        /// <typeparam name="T">The type of objects contained within the <paramref name="collection"/>.</typeparam>
        /// <param name="collection"><see cref="ICollection"/></param>
        /// <param name="obj">Object to add to the list. The object is only added if it isn't a duplicate of an existing element, as determined by the default equality comparison operator for type <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> when <paramref name="obj"/> was added to <paramref name="collection"/>; otherwise <see langword="false"/>.</returns>
        public static bool AddIfUnique<T>(this ICollection<T> collection, T obj)
        {
            if (!collection.Contains(obj))
            {
                collection.Add(obj);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Calls <see cref="AddIfUnique{T}(ICollection{T}, T)"/> on the given <paramref name="range"/> of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects contained within the <paramref name="collection"/>.</typeparam>
        /// <param name="collection"><see cref="ICollection{T}"/></param>
        /// <param name="range">A range of objects of type <typeparamref name="T"/> to add to the list. Each object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        /// <returns>The number of items from <paramref name="range"/> that were added to <paramref name="collection"/>.</returns>
        public static int AddRangeIfUnique<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            int count = 0;
            foreach (T item in range)
            {
                count += collection.AddIfUnique(item) ? 1 : 0;
            }
            return count;
        }
        /// <summary>
        /// Determines whether the <paramref name="collection"/> contains at least one of the specified <paramref name="items"/>.
        /// </summary>
        /// <typeparam name="T">Item type contained within the collection.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="items">The items to search for in the collection.</param>
        /// <returns><see langword="true"/> when at least one of the specified <paramref name="items"/> exists in the <paramref name="collection"/>; otherwise <see langword="false"/>.</returns>
        public static bool ContainsAny<T>(this ICollection<T> collection, params T[] items)
        {
            switch (items.Length)
            {
            case 0: // no search items specified:
                return collection.Count > 0;
            case 1: // one search item specified:
                return collection.Contains(items[0]);
            default: // multiple search items specified:
                {
                    bool itemsContainsNull = false;
                    for (int i = 0; i < items.Length; ++i)
                    {
                        if (items[i] == null) itemsContainsNull = true;
                    }

                    foreach (var item in collection)
                    {
                        if (item == null)
                        {
                            if (itemsContainsNull) return true;
                            else continue;
                        }

                        for (int i = 0; i < items.Length; ++i)
                        {
                            if (item.Equals(items[i]))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
        }
    }
}
