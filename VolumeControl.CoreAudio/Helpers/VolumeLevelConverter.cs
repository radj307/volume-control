namespace Audio.Helpers
{
    /// <summary>
    /// Exposes functions for converting between native volume level (<see cref="float"/>; 0.0-1.0) and regular volume level (<see cref="int"/>; 0-100) values.
    /// </summary>
    public static class VolumeLevelConverter
    {
        #region Functions
        /// <summary>
        /// Converts from a native volume level (float) to a regular volume level (int).
        /// </summary>
        /// <param name="nativeVolume">A native volume level value.</param>
        /// <returns><paramref name="nativeVolume"/> * 100</returns>
        public static int FromNativeVolume(float nativeVolume) => Convert.ToInt32(nativeVolume * 100f);
        /// <summary>
        /// Converts from a regular volume level (int) to a native volume level (float).
        /// </summary>
        /// <param name="volume">A volume level value.</param>
        /// <returns><paramref name="volume"/> / 100</returns>
        public static float ToNativeVolume(int volume) => Convert.ToSingle(volume) / 100f;
        #endregion Functions
    }
}