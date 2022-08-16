using System.Collections;

namespace VolumeControl.TypeExtensions
{
    /// <summary>Extensions for the generic <see cref="List{T}"/> object.</summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Similar to <see cref="List{T}.AddRange(IEnumerable{T})"/>, but designed for unique elements.<br/>
        /// Removes elements from <paramref name="l"/> that are <b>not present in <paramref name="other"/></b>.<br/>
        /// Adds elements from <paramref name="other"/> to <paramref name="l"/> that aren't already present somewhere in the list.<br/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l">The generic list on which to operate.</param>
        /// <param name="other">Any enumerable list of type <typeparamref name="T"/>.</param>
        public static void SelectiveUpdate<T>(this List<T> l, IEnumerable<T> other)
        {
            // remove entries that AREN'T present in the incoming list,
            //  and ARE present in the current list
            for (int i = l.Count - 1; i >= 0; --i)
            {
                if (!other.Any(incomingItem => incomingItem != null && incomingItem.Equals(l[i])))
                    l.RemoveAt(i);
            }
            // add entries that ARE present in the incoming list,
            //  and AREN'T present in the current list
            foreach (T incomingItem in other)
            {
                if (!l.Any(i => i != null && i.Equals(incomingItem)))
                    l.Add(incomingItem);
            }
        }
        /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
        /// <returns>The resulting <paramref name="list"/>; this can safely be ignored when using this method outside of a pipeline.</returns>
        public static IList<T> ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (T? item in list)
                action(item);
            return list;
        }
        /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
        /// <returns>The resulting <paramref name="list"/>; this can safely be ignored when using this method outside of a pipeline.</returns>
        public static IList ForEach(this IList list, Action<object?> action)
        {
            foreach (object? item in list)
                action(item);
            return list;
        }
        /// <summary>
        /// Adds <paramref name="obj"/> to the <paramref name="list"/> if it isn't a duplicate of any existing elements.
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="obj">Object to add to the list. The object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        public static void AddIfUnique(this IList list, object obj)
        {
            if (!list.Contains(obj))
                _ = list.Add(obj);
        }
        /// <summary>
        /// Calls <see cref="AddIfUnique(IList, object)"/> on the given <paramref name="range"/> of objects.
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="range">A range of objects to add to the list. Each object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        public static void AddRangeIfUnique(this IList list, IEnumerable range)
        {
            foreach (object? item in range)
                list.AddIfUnique(item);
        }
    }
}
