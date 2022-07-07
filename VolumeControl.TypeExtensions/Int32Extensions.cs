namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extensions for the <see cref="int"/> integral type.
    /// </summary>
    public static class Int32Extensions
    {
        /// <inheritdoc cref="MathExt.Scale(int, int, int, int, int)"/>
        public static int Scale(this int input, int inRangeMin, int inRangeMax, int outRangeMin, int outRangeMax) => MathExt.Scale(input, inRangeMin, inRangeMax, outRangeMin, outRangeMax);
        /// <summary>
        /// Accepts <paramref name="input"/>, a number in the range ( <paramref name="inRange"/>.Item1 - <paramref name="inRange"/>.Item2 ), and translates it to be within the range ( <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2 ).
        /// </summary>
        /// <remarks>See <see cref="Scale(int, int, int, int, int)"/> if you're having trouble with the tuples.</remarks>
        /// <param name="input">The input number</param>
        /// <param name="inRange">The range that <paramref name="input"/> is within.</param>
        /// <param name="outRange">The range to translate <paramref name="input"/> to.</param>
        /// <returns><paramref name="input"/> scaled to be within the range <paramref name="outRange"/>.Item1 - <paramref name="outRange"/>.Item2</returns>
        public static int Scale(this int input, (int, int) inRange, (int, int) outRange) => MathExt.Scale(input, inRange, outRange);
    }
}
