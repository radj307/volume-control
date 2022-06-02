using System.Collections;

namespace VolumeControl.TypeExtensions
{
    /// <summary>Extensions for the <see cref="IEnumerable"/> interface type.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>Gets an enumerable list of all items of type <typeparamref name="T"/> from <paramref name="enumerable"/></summary>
        /// <typeparam name="T">Target type to search for and to return.</typeparam>
        /// <param name="enumerable">Any enumerable object.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> list.</returns>
        public static IEnumerable<T> WithType<T>(this IEnumerable enumerable)
        {
            List<T> l = new();
            foreach (var item in enumerable)
                if (item is T match) l.Add(match);
            return l.AsEnumerable();
        }
        /// <summary>Similar to the regular Select Linq method except that it only includes selected values that are unique.</summary>
        /// <typeparam name="TIn">Input Type</typeparam>
        /// <typeparam name="TResult">Output Type</typeparam>
        /// <param name="enumerable">Any enumerable <typeparamref name="TIn"/> type.</param>
        /// <param name="selector">A selector method.</param>
        /// <returns><see cref="IEnumerable"/> of type <typeparamref name="TResult"/>, where each item is unique as determined by the generic Equals() method.</returns>
        public static IEnumerable<TResult> SelectIfUnique<TIn, TResult>(this IEnumerable<TIn> enumerable, Func<TIn, TResult> selector)
        {
            List<TResult> l = new();
            foreach (TIn item in enumerable)
                if (selector(item) is TResult selected && !l.Contains(selected))
                    l.Add(selected);
            return l.AsEnumerable();
        }
        public static void AddIfUnique(this IList list, object obj)
        {
            if (!list.Contains(obj))
                list.Add(obj);
        }
        public static void AddRangeIfUnique(this IList list, IEnumerable range)
        {
            foreach (var item in range)
                list.AddIfUnique(item);
        }
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
        public static IEnumerable<T> ForwardForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (var item in enumerable)
                action(item);
            return enumerable;
        }
    }
}
