using System.Collections;

namespace VolumeControl.TypeExtensions
{
    /// <summary>Extensions for the <see cref="IEnumerable"/> interface type.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Similar to the regular Select Linq method except that it only includes selected values that are unique.
        /// </summary>
        /// <typeparam name="TSource">Input Type</typeparam>
        /// <typeparam name="TResult">Output Type</typeparam>
        /// <param name="source">Any enumerable <typeparamref name="TSource"/> type.</param>
        /// <param name="selector">A selector method.</param>
        /// <returns><see cref="IEnumerable"/> of type <typeparamref name="TResult"/>, where each item is unique as determined by the generic Equals() method.</returns>
        public static IEnumerable<TResult> SelectIfUnique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            HashSet<TResult> processedItems = new();
            foreach (TSource item in source)
            {
                if (selector(item) is TResult selected && !processedItems.Contains(selected))
                {
                    processedItems.Add(selected);
                    yield return selected;
                }
            }
        }
        /// <summary>
        /// Performs the specified <paramref name="action"/> on each <paramref name="source"/> element.
        /// </summary>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
        /// <summary>
        /// Performs the specified <paramref name="func"/> on each <paramref name="source"/> element, and discards the result.
        /// </summary>
        /// <typeparam name="TSource">The type of object contained by the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TDiscardedResult">The type that is returned by the specified <paramref name="func"/>. Returned values are ignored.</typeparam>
        public static void ForEach<TSource, TDiscardedResult>(this IEnumerable<TSource> source, Func<TSource, TDiscardedResult> func)
        {
            foreach (var item in source)
            {
                func(item);
            }
        }
        /// <summary>
        /// Does the same thing as the standard Linq Select() method, except this one uses a <paramref name="predicate"/> and applies the <paramref name="selector"/> only when the predicate returns <see langword="true"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="predicate">A predicate to apply to each item in <paramref name="source"/>.<br/>
        /// Return values are handled as follows:
        /// <list type="table">
        /// <item><term><see langword="true"/></term><description>Applies <paramref name="selector"/> to the item, adding it to the returned enumerable.</description></item>
        /// <item><term><see langword="false"/></term><description>Skips the item, and does not apply the <paramref name="selector"/>.</description></item>
        /// </list>
        /// </param>
        /// <returns>Enumerable with values selected in the items of <paramref name="source"/> by the <paramref name="selector"/>, excluding those filtered out by <paramref name="predicate"/>.</returns>
        public static IEnumerable<TResult> SelectIf<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Func<TSource, bool> predicate)
        {
            foreach (TSource item in source)
            {
                if (predicate(item))
                {
                    yield return selector(item);
                }
            }
        }
        /// <summary>
        /// Selects non-<see langword="null"/> values from the enumerable using the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of element in the enumerable.</typeparam>
        /// <typeparam name="TResult">The type of element in the resulting enumerable.</typeparam>
        /// <param name="source">The enumerable to apply the <paramref name="selector"/> to.</param>
        /// <param name="selector">A function that accepts a parameter of type <typeparamref name="TSource"/> and returns a <typeparamref name="TResult"/> instance, or <see langword="null"/>. When this returns <see langword="null"/> the item does not appear in the resulting enumeration.</param>
        /// <returns>The resulting elements of type <typeparamref name="TResult"/> that aren't <see langword="null"/>.</returns>
        public static IEnumerable<TResult> SelectValue<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector)
        {
            foreach (var item in source)
            {
                if (selector(item) is TResult value)
                {
                    yield return value;
                }
            }
        }
        /// <summary>
        /// Applies the specified <paramref name="selector"/> to each element and returns the resulting values.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the specified <paramref name="selector"/>.</typeparam>
        /// <param name="source">An untyped enumerable instance to use as the source.</param>
        /// <param name="selector">A selector method that accepts an untyped object and returns a <typeparamref name="TResult"/> instance.</param>
        /// <returns>The enumeration after applying the <paramref name="selector"/> to each element.</returns>
        public static IEnumerable<TResult> Select<TResult>(this IEnumerable source, Func<object, TResult> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            foreach (var obj in source)
            {
                yield return selector(obj);
            }
        }
        /// <summary>
        /// Calls <see cref="IDisposable.Dispose"/> on all of the elements in the collection.
        /// </summary>
        /// <typeparam name="T">Enumerable element type that implements <see cref="IDisposable"/>.</typeparam>
        /// <param name="source">(implicit) The enumerable collection to dispose of.</param>
        public static void DisposeAll<T>(this IEnumerable<T> source) where T : IDisposable
        {
            ArgumentNullException.ThrowIfNull(source);
            foreach (var item in source)
            {
                item?.Dispose();
            }
        }
    }
}
