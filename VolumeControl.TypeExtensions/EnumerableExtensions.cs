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
            foreach (object? item in enumerable)
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
            {
                if (selector(item) is TResult selected && !l.Contains(selected))
                    l.Add(selected);
            }

            return l.AsEnumerable();
        }
        /// <summary>Performs the specified <paramref name="action"/> on each <paramref name="enumerable"/> element.</summary>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T? item in enumerable)
            {
                action(item);
            }
        }
        /// <summary>
        /// Performs the specified <paramref name="func"/> on each <paramref name="enumerable"/> element.
        /// </summary>
        /// <typeparam name="T">The type of object contained by the <paramref name="enumerable"/>.</typeparam>
        /// <typeparam name="TReturn">The type that is returned by the specified <paramref name="func"/>. Returned values are ignored.</typeparam>
        public static void ForEach<T, TReturn>(this IEnumerable<T> enumerable, Func<T, TReturn> func)
        {
            foreach (T? item in enumerable)
            {
                func(item);
            }
        }
        /// <summary>
        /// Performs the specified <paramref name="action"/> on each <paramref name="enumerable"/> element.
        /// </summary>
        /// <returns><paramref name="enumerable"/>, allowing this method to be used in a pipeline.</returns>
        public static IEnumerable<T> ForwardForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (T? item in enumerable)
                action(item);
            return enumerable;
        }
        /// <summary>
        /// Does the same thing as the standard Linq Select() method, except this one uses a <paramref name="predicate"/> and applies the <paramref name="keySelector"/> only when the predicate returns <see langword="true"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TVar"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="keySelector"></param>
        /// <param name="predicate">A predicate to apply to each item in <paramref name="enumerable"/>.<br/>
        /// Return values are handled as follows:
        /// <list type="table">
        /// <item><term><see langword="true"/></term><description>Applies <paramref name="keySelector"/> to the item, adding it to the returned enumerable.</description></item>
        /// <item><term><see langword="false"/></term><description>Skips the item, and does not apply the <paramref name="keySelector"/>.</description></item>
        /// </list>
        /// </param>
        /// <returns>Enumerable with values selected in the items of <paramref name="enumerable"/> by the <paramref name="keySelector"/>, excluding those filtered out by <paramref name="predicate"/>.</returns>
        public static IEnumerable<TVar> SelectIf<T, TVar>(this IEnumerable<T> enumerable, Func<T, TVar> keySelector, Predicate<T> predicate)
        {
            List<TVar> l = new();
            foreach (T item in enumerable)
            {
                if (predicate(item))
                    l.Add(keySelector(item));
            }

            return l.AsEnumerable();
        }
    }
}
