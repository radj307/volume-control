namespace VolumeControl.TypeExtensions
{
    /// <summary>Extensions for generic array types.</summary>
    public static class ArrayExtensions
    {
        /// <summary>Performs the specified action on each element of the <see cref="Array"/>.</summary>
        /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
        public static T[] ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (T? item in array)
                action(item);
            return array;
        }
        /// <summary>Applies the specified function to each element of the <see cref="Array"/>.</summary>
        /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
        public static T[] ForEach<T>(this T[] array, Func<T, T> function)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = function(array[i]);
            return array;
        }
    }
}
