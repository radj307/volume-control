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
        public static bool EqualsAny<T>(this T e, IEnumerable<T> other) where T : struct, Enum
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
        public static bool EqualsAny<T>(this T e, params T[] other) where T : struct, Enum => e.EqualsAny(other.AsEnumerable());
        /// <summary>
        /// Returns true if <paramref name="e"/> contains any <see langword="enum"/> value present in <paramref name="other"/>.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="e">The enum instance.</param>
        /// <param name="other">Any enumerable of type <typeparamref name="T"/>.</param>
        /// <returns>True when <paramref name="e"/> is equal to at least one value from <paramref name="other"/></returns>
        public static bool HasAnyFlag<T>(this T e, IEnumerable<T> other) where T : struct, Enum
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
        public static bool HasAnyFlag<T>(this T e, params T[] other) where T : struct, Enum => e.HasAnyFlag(other.AsEnumerable());
        /// <summary>
        /// Returns the enum value with the specified <paramref name="flag"/> changed.
        /// </summary>
        /// <typeparam name="T">The type of enum being operated on.</typeparam>
        /// <param name="e">(implicit) Enum value to operate on.</param>
        /// <param name="flag">The enum value of the flag to set or unset.</param>
        /// <param name="state">The specified <paramref name="flag"/> is set when <see langword="true"/> and unset when <see langword="false"/>.</param>
        /// <returns>The enum value with the specified <paramref name="flag"/> set to the specified <paramref name="state"/>.</returns>
        public static T SetFlagState<T>(this T e, T flag, bool state) where T : struct, Enum
        {
            var e_v = Convert.ToInt64(e);

            if (state)
            {
                e_v |= Convert.ToInt64(flag);
            }
            else
            {
                e_v &= ~Convert.ToInt64(flag);
            }

            return (T)Enum.ToObject(typeof(T), e_v);
        }
        /// <summary>
        /// Returns the enum value with any number of flags changed.
        /// </summary>
        /// <typeparam name="T">The type of enum being operated on.</typeparam>
        /// <param name="e">(implicit) Enum value to operate on.</param>
        /// <param name="flagStates">Any number of flag-state pairs. When state is <see langword="true"/>, the associated flag is set; otherwise when state is <see langword="false"/>, the associated flag is unset.</param>
        /// <returns>The enum value with the specified <paramref name="flagStates"/> changed.</returns>
        public static T SetFlagStates<T>(this T e, params (T flag, bool state)[] flagStates) where T : struct, Enum
        {
            var e_v = Convert.ToInt64(e);

            foreach ((T flag, bool state) in flagStates)
            {
                if (state)
                {
                    e_v |= Convert.ToInt64(flag);
                }
                else
                {
                    e_v &= ~Convert.ToInt64(flag);
                }
            }

            return (T)Enum.ToObject(typeof(T), e_v);
        }
        /// <summary>
        /// Performs an XOR bitwise operation on the enum and the specified <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// Intended for use when the actual enum type is not known, such as within a generic method.
        /// </remarks>
        /// <typeparam name="T">The type of enum being operated on.</typeparam>
        /// <param name="e">(implicit) Enum value to operate on.</param>
        /// <param name="value">Enum value to operate on.</param>
        /// <returns>The result of the bitwise XOR operation.</returns>
        public static T Xor<T>(this T e, T value) where T : struct, Enum
        {
            return (T)Enum.ToObject(typeof(T), Convert.ToInt64(e) ^ Convert.ToInt64(value));
        }
        /// <summary>
        /// Gets a single merged value containing all of the flags in the <paramref name="enumerable"/>.
        /// </summary>
        /// <typeparam name="T">The type of enum being operated on.</typeparam>
        /// <param name="enumerable">(implicit) Enumerable containing enum values of type <typeparamref name="T"/>.</param>
        /// <returns>A single enum value containing all of the values in the <paramref name="enumerable"/>.</returns>
        public static T GetSingleValue<T>(this IEnumerable<T> enumerable) where T : struct, Enum
        {
            var result = Convert.ToInt64(default(T));

            foreach (var value in enumerable)
            {
                result |= Convert.ToInt64(value);
            }

            return (T)Enum.ToObject(typeof(T), result);
        }
        /// <summary>
        /// Checks if the enum value has only 1 bit set.
        /// </summary>
        /// <typeparam name="T">The type of enum being operated on.</typeparam>
        /// <param name="e">(implicit) Enum value to operate on.</param>
        /// <returns><see langword="true"/> when the enum value is a power of 2; <see langword="false"/>.</returns>
        public static bool IsSingleValue<T>(this T e) where T : struct, Enum
        {
            var e_v = Convert.ToInt64(e);
            
            return e_v == 0 || (e_v & (e_v - 1)) == 0;
        }
    }
}
