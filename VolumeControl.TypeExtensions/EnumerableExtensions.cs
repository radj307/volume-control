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
        /// <summary>
        /// Adds <paramref name="obj"/> to the <paramref name="list"/> if it isn't a duplicate of any existing elements.
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="obj">Object to add to the list. The object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        public static void AddIfUnique(this IList list, object obj)
        {
            if (!list.Contains(obj))
                list.Add(obj);
        }
        /// <summary>
        /// Calls <see cref="AddIfUnique(IList, object)"/> on the given <paramref name="range"/> of objects.
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="range">A range of objects to add to the list. Each object is only added if it isn't a duplicate of an existing element, as determined by <see cref="object.Equals(object?)"/>.</param>
        public static void AddRangeIfUnique(this IList list, IEnumerable range)
        {
            foreach (var item in range)
                list.AddIfUnique(item);
        }
        /// <summary>Performs the specified <paramref name="action"/> on each <paramref name="enumerable"/> element.</summary>
        public static void ForEach(this IEnumerable enumerable, Action<object?> action)
        {
            foreach (var item in enumerable)
                action(item);
        }
        /// <summary>Performs the specified <paramref name="action"/> on each <paramref name="enumerable"/> element.</summary>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
        /// <summary>Performs the specified <paramref name="action"/> on each <paramref name="enumerable"/> element.</summary>
        /// <returns><paramref name="enumerable"/>, allowing this method to be used in a pipeline.</returns>
        public static IEnumerable<T> ForwardForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (var item in enumerable)
                action(item);
            return enumerable;
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
            foreach (var item in enumerable)
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
        public static List<TOut> ConvertEach<TOut, TIn>(this IEnumerable<TIn> enumerable, Func<TIn, TOut> converter)
        {
            List<TOut> l = new();
            foreach (var item in enumerable)
                l.Add(converter(item));
            return l;
        }
    }
}
