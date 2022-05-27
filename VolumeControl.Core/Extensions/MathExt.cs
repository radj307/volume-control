namespace VolumeControl.Core.Extensions
{
    /// <summary>
    /// Methods for various mathematics operations not covered by <see cref="Math"/>
    /// </summary>
    public static class MathExt
    {
        /// <summary>Clamps the given value between <paramref name="min"/> and <paramref name="max"/>.</summary>
        /// <remarks>The <paramref name="min"/> and <paramref name="max"/> boundary values are <b>inclusive</b>.</remarks>
        /// <typeparam name="T">Any numerical type.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowable value.</param>
        /// <param name="max">The maximum allowable value.</param>
        /// <returns><paramref name="value"/> clamped between <paramref name="min"/> and <paramref name="max"/>.</returns>
        public static T ClampValue<T>(T value, T min, T max) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, ISpanFormattable, IFormattable
        {
            if (value.CompareTo(min) < 0)
                value = min;
            else if (value.CompareTo(max) > 0)
                value = max;
            return value;
        }
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRangeMin"/> - <paramref name="inRangeMax"/> ), and translates it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> ).
        /// </summary>
        /// <param name="input">The input number</param>
        /// <param name="inRangeMin">The minimum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="inRangeMax">The maximum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="outRangeMin">The minimum boundary of the output range.</param>
        /// <param name="outRangeMax">The minimum boundary of the output range.</param>
        /// <returns><paramref name="input"/> scaled to be within the range <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/></returns>
        public static int Scale(int input, int inRangeMin, int inRangeMax, int outRangeMin, int outRangeMax)
            => outRangeMin + (input - inRangeMin) * (outRangeMax - outRangeMin) / (inRangeMax - inRangeMin);
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(int, int, int, int, int)"/> if you're having trouble with the tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns><paramref name="input"/> scaled to be within the range <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2</returns>
        public static int Scale(int input, (int, int) inRange, (int, int) outRange) => Scale(input, inRange.Item1, inRange.Item2, outRange.Item1, outRange.Item2);
    }
}
