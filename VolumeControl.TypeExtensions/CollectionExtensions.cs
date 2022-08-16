using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void AddIfUnique<T>(this ICollection<T> collection, T obj)
        {
            if (!collection.Contains(obj))
                collection.Add(obj);
        }
        /// <summary>
        /// Calls <see cref="AddIfUnique{T}(ICollection{T}, T)"/> on the given <paramref name="range"/> of objects.
        /// </summary>
        /// <typeparam name="T">The type of objects contained within the <paramref name="collection"/>.</typeparam>
        /// <param name="collection"><see cref="ICollection{T}"/></param>
        /// <param name="range">A range of objects of type <typeparamref name="T"/> to add to the list. Each object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        public static void AddRangeIfUnique<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (T item in range)
                collection.AddIfUnique(item);
        }
    }
}
