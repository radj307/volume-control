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
            for (int i = 0, max = list.Count; i < max; ++i)
            {
                action(list[i]);
            }
            return list;
        }
        /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
        /// <returns>The resulting <paramref name="list"/>; this can safely be ignored when using this method outside of a pipeline.</returns>
        public static IList ForEach(this IList list, Action<object?> action)
        {
            for (int i = 0, max = list.Count; i < max; ++i)
            {
                action(list[i]);
            }
            return list;
        }
        /// <summary>
        /// Adds a range of items to the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">(implicit) The list to add the items to.</param>
        /// <param name="enumerable">An <see cref="IEnumerable"/> of items to add.</param>
        public static void AddRange(this IList list, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                list.Add(item);
            }
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
        /// <summary>
        /// Converts each element in the given <paramref name="enumerable"/> from type <typeparamref name="TIn"/> to type <typeparamref name="TOut"/>, and returns the converted objects as a new list of type <typeparamref name="TList"/>.
        /// </summary>
        /// <typeparam name="TList">The type of list to return.</typeparam>
        /// <typeparam name="TOut">The type to convert each item to.</typeparam>
        /// <typeparam name="TIn">The type to convert each item from.</typeparam>
        /// <param name="enumerable">Any type implementing <see cref="IEnumerable{T}"/>.</param>
        /// <param name="converter">A converter method that accepts <typeparamref name="TIn"/> and returns <typeparamref name="TOut"/>.</param>
        /// <returns>The converted list of items.</returns>
        public static TList ConvertEach<TList, TOut, TIn>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> converter) where TList : IList, IEnumerable, IList<TOut>, IEnumerable<TOut>, ICollection, ICollection<TOut>, new()
        {
            TList l = new();
            foreach (TIn? item in enumerable)
                l.Add(converter(item));
            return l;
        }
        /// <summary>
        /// Converts each element in the given <paramref name="enumerable"/> from type <typeparamref name="TIn"/> to type <typeparamref name="TOut"/>, and returns the converted objects as a new <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type to convert each item to.</typeparam>
        /// <typeparam name="TIn">The type to convert each item from.</typeparam>
        /// <param name="enumerable">Any type implementing <see cref="IEnumerable{T}"/>.</param>
        /// <param name="converter">A converter method that accepts <typeparamref name="TIn"/> and returns <typeparamref name="TOut"/>.</param>
        /// <returns>The converted list of items.</returns>
        public static IList<TOut> ConvertEach<TOut, TIn>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> converter)
        {
            List<TOut> l = new();
            foreach (TIn? item in enumerable)
                l.Add(converter(item));
            return l;
        }
        /// <summary>
        /// Gets the index of the first occurrence of <paramref name="item"/> in this <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of object in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="list">(implicit) The <see cref="IList{T}"/> to search.</param>
        /// <param name="item">The object to locate in the <see cref="IList{T}"/>.</param>
        /// <param name="index">The index of <paramref name="item"/> if found in the list; otherwise -1.</param>
        /// <returns><see langword="true"/> when <paramref name="item"/> was found in the list; otherwise <see langword="false"/>.</returns>
        public static bool IndexOf<T>(this IList<T> list, T item, out int index)
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
