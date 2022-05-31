namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extensions for floating-point number types.
    /// </summary>
    public static class RealNumberExtensions
    {
        /// <summary>
        /// Compares two <see cref="double"/> types for equality using <see cref="double.Epsilon"/>.
        /// </summary>
        /// <param name="n">This number.</param>
        /// <param name="other">A number to compare this number to.</param>
        /// <param name="epsilon">The epsilon comparsion threshold value.</param>
        /// <returns>True when <paramref name="n"/> and <paramref name="other"/> are less than <paramref name="epsilon"/> apart.</returns>
        public static bool EqualsWithin(this double n, double other, double epsilon = double.Epsilon) => Math.Abs((double)(n - other)) <= epsilon;
        /// <summary>
        /// Compares two <see cref="float"/> types for equality using <see cref="float.Epsilon"/>.
        /// </summary>
        /// <param name="n">This number.</param>
        /// <param name="other">A number to compare this number to.</param>
        /// <param name="epsilon">The epsilon comparsion threshold value.</param>
        /// <returns>True when <paramref name="n"/> and <paramref name="other"/> are less than <paramref name="epsilon"/> apart.</returns>
        public static bool EqualsWithin(this float n, float other, float epsilon = float.Epsilon) => Math.Abs((float)(n - other)) <= epsilon;
    }
}
