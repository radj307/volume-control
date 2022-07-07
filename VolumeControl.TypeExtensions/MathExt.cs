namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Methods for various mathematics operations not covered by <see cref="Math"/>
    /// </summary>
    public static class MathExt
    {
        #region Clamp
        /// <summary>Clamps <paramref name="value"/> within the range ( <paramref name="min"/> - <paramref name="max"/> )</summary>
        /// <remarks>The <paramref name="min"/> and <paramref name="max"/> boundary values are <b>inclusive</b>.</remarks>
        /// <typeparam name="T">Any numerical type.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum allowable value.<br/>Must be less than <paramref name="max"/>, or an exception will be thrown.</param>
        /// <param name="max">The maximum allowable value.<br/>Must be greater than <paramref name="min"/>, or an exception will be thrown.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        /// <returns><paramref name="value"/>  between <paramref name="min"/> and <paramref name="max"/>.</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable, IComparable<T>, IConvertible, IEquatable<T>, ISpanFormattable, IFormattable
        {
            if (min.CompareTo(max) > 0) // min exceeds max
                throw new ArgumentOutOfRangeException($"{nameof(Clamp)}<{typeof(T).Name}>({nameof(value)}: {value}, {nameof(min)}: {min}, {nameof(max)}: {max}) Failed:  {nameof(min)} cannot be greater than {nameof(max)}!");
            if (value.CompareTo(min) < 0) // value preceeds min
                value = min;
            else if (value.CompareTo(max) > 0) // value exceeds max
                value = max;
            return value;
        }
        #endregion Clamp
        #region Scale
        #region ScaleInteger
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRangeMin"/> - <paramref name="inRangeMax"/> ), and translates it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> ).
        /// </summary>
        /// <param name="input">The input number</param>
        /// <param name="inRangeMin">The minimum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="inRangeMax">The maximum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="outRangeMin">The minimum boundary of the output range.</param>
        /// <param name="outRangeMax">The minimum boundary of the output range.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> )</returns>
        public static int Scale(int input, int inRangeMin, int inRangeMax, int outRangeMin, int outRangeMax)
            => outRangeMin + ((input - inRangeMin) * (outRangeMax - outRangeMin) / (inRangeMax - inRangeMin));
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(int, int, int, int, int)"/> for an overload that doesn't use tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 )</returns>
        public static int Scale(int input, (int, int) inRange, (int, int) outRange) => Scale(input, inRange.Item1, inRange.Item2, outRange.Item1, outRange.Item2);
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRangeMin"/> - <paramref name="inRangeMax"/> ), and translates it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> ).
        /// </summary>
        /// <param name="input">The input number</param>
        /// <param name="inRangeMin">The minimum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="inRangeMax">The maximum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="outRangeMin">The minimum boundary of the output range.</param>
        /// <param name="outRangeMax">The minimum boundary of the output range.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> )</returns>
        public static long Scale(long input, long inRangeMin, long inRangeMax, long outRangeMin, long outRangeMax)
            => outRangeMin + ((input - inRangeMin) * (outRangeMax - outRangeMin) / (inRangeMax - inRangeMin));
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(long, long, long, long, long)"/> for an overload that doesn't use tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 )</returns>
        public static long Scale(long input, (long, long) inRange, (long, long) outRange) => Scale(input, inRange.Item1, inRange.Item2, outRange.Item1, outRange.Item2);
        #endregion ScaleInteger
        #region ScaleFloatingPoint
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRangeMin"/> - <paramref name="inRangeMax"/> ), and translates it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> ).
        /// </summary>
        /// <param name="input">The input number</param>
        /// <param name="inRangeMin">The minimum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="inRangeMax">The maximum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="outRangeMin">The minimum boundary of the output range.</param>
        /// <param name="outRangeMax">The minimum boundary of the output range.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> )</returns>
        public static float Scale(float input, float inRangeMin, float inRangeMax, float outRangeMin, float outRangeMax)
            => outRangeMin + ((input - inRangeMin) * (outRangeMax - outRangeMin) / (inRangeMax - inRangeMin));
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(float,float,float,float,float)"/> for an overload that doesn't use tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 )</returns>
        public static float Scale(float input, (float, float) inRange, (float, float) outRange) => Scale(input, inRange.Item1, inRange.Item2, outRange.Item1, outRange.Item2);
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRangeMin"/> - <paramref name="inRangeMax"/> ), and translates it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> ).
        /// </summary>
        /// <param name="input">The input number</param>
        /// <param name="inRangeMin">The minimum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="inRangeMax">The maximum boundary of the range that <paramref name="input"/> is within.</param>
        /// <param name="outRangeMin">The minimum boundary of the output range.</param>
        /// <param name="outRangeMax">The minimum boundary of the output range.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRangeMin"/> - <paramref name="outRangeMax"/> )</returns>
        public static double Scale(double input, double inRangeMin, double inRangeMax, double outRangeMin, double outRangeMax)
            => outRangeMin + ((input - inRangeMin) * (outRangeMax - outRangeMin) / (inRangeMax - inRangeMin));
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(double,double,double,double,double)"/> for an overload that doesn't use tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns>Equivalent value of <paramref name="input"/> were it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 )</returns>
        public static double Scale(double input, (double, double) inRange, (double, double) outRange) => Scale(input, inRange.Item1, inRange.Item2, outRange.Item1, outRange.Item2);
        #endregion ScaleFloatingPoint
        #endregion Scale
    }
}
