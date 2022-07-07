namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extends nullable boolean and byte types with conversion methods.
    /// </summary>
    public static class NullableBoolExtensions
    {
        /// <summary>
        /// Gets the equivalent 3-state number value of <paramref name="b"/>.
        /// </summary>
        /// <param name="b">A 3-state boolean value. (<see langword="true"/>, <see langword="false"/>, or <see langword="null"/>)</param>
        /// <returns><list type="table">
        /// <item><term>0</term><description> <paramref name="b"/> was <see langword="false"/></description></item>
        /// <item><term>1</term><description> <paramref name="b"/> was <see langword="true"/></description></item>
        /// <item><term>2</term><description> <paramref name="b"/> was <see langword="null"/></description></item>
        /// </list></returns>
        public static byte ToThreeStateNumber(this bool? b) => b.HasValue ? Convert.ToByte(b.Value) : (byte)2;
        /// <summary>
        /// Converts the 3-state number value <paramref name="threeStateNumber"/> to its equivalent nullable boolean value.
        /// </summary>
        /// <param name="threeStateNumber">Must be a 0, 1, or 2; any other values will throw an exception.</param>
        /// <returns><list type="table">
        /// <item><term><see langword="false"/></term><description> <paramref name="threeStateNumber"/> was 0</description></item>
        /// <item><term><see langword="true"/></term><description> <paramref name="threeStateNumber"/> was 1</description></item>
        /// <item><term><see langword="null"/></term><description> <paramref name="threeStateNumber"/> was 2</description></item>
        /// </list></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="threeStateNumber"/> isn't a 0, 1, or 2.</exception>
        public static bool? ToBoolean(this byte threeStateNumber) => threeStateNumber switch
        {
            0 => false,
            1 => true,
            2 => null,
            _ => throw new ArgumentOutOfRangeException($"{nameof(threeStateNumber)} ({threeStateNumber}) isn't a valid 3-state boolean value! ( min: 0, max: 2 )"),
        };
    }
}
