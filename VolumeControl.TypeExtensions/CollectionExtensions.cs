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
    }
}
