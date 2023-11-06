namespace VolumeControl.TypeExtensions
{
    /// <summary>
    /// Extension methods for the <see cref="bool"/> type.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts the <see cref="bool"/> value to <see cref="int"/>.
        /// </summary>
        /// <param name="value">(implicit) A boolean value.</param>
        /// <returns>1 when <paramref name="value"/> was <see langword="true"/>; 0 when <paramref name="value"/> was <see langword="false"/>.</returns>
        public static int ToInt32(this bool value) => value ? 1 : 0;
        /// <summary>
        /// Converts the <see cref="bool"/> value to <see cref="uint"/>.
        /// </summary>
        /// <param name="value">(implicit) A boolean value.</param>
        /// <returns>1 when <paramref name="value"/> was <see langword="true"/>; 0 when <paramref name="value"/> was <see langword="false"/>.</returns>
        public static uint ToUInt32(this bool value) => value ? 1u : 0u;
    }
}
