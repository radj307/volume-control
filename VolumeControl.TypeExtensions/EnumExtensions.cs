namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extensions for any <see cref="Enum"/> type.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns true if <paramref name="e"/> is equal to any <see langword="enum"/> value present in <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="e">The enum instance.</param>
        /// <param name="other">Any enumerable of type <typeparamref name="T"/>.</param>
        /// <returns>True when <paramref name="e"/> is equal to at least one value from <paramref name="other"/></returns>
        public static bool EqualsAny<T>(this T e, IEnumerable<T> other) where T : Enum
        {
            foreach (T? o in other)
            {
                if (e.Equals(o))
                    return true;
            }

            return false;
        }
        /// <param name="e">The enum instance.</param>
        /// <param name="other">Any number of other enums of type <typeparamref name="T"/>.</param>
        /// <inheritdoc cref="EqualsAny{T}(T, IEnumerable{T})"/>
        public static bool EqualsAny<T>(this T e, params T[] other) where T : Enum => e.EqualsAny(other.AsEnumerable());
        /// <summary>
        /// Returns true if <paramref name="e"/> contains any <see langword="enum"/> value present in <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="e">The enum instance.</param>
        /// <param name="other">Any enumerable of type <typeparamref name="T"/>.</param>
        /// <returns>True when <paramref name="e"/> is equal to at least one value from <paramref name="other"/></returns>
        public static bool HasAnyFlag<T>(this T e, IEnumerable<T> other) where T : Enum
        {
            foreach (T? o in other)
            {
                if (e.HasFlag(o))
                    return true;
            }

            return false;
        }
        /// <param name="e">The enum instance.</param>
        /// <param name="other">Any number of other enums of type <typeparamref name="T"/>.</param>
        /// <inheritdoc cref="EqualsAny{T}(T, IEnumerable{T})"/>
        public static bool HasAnyFlag<T>(this T e, params T[] other) where T : Enum => e.HasAnyFlag(other.AsEnumerable());
    }
}
