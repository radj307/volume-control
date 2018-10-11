namespace ToastifyAPI.Native.RawInputAPI.Enums
{
    /// <summary>
    ///     Enumeration containing the type device the raw input is coming from.
    /// </summary>
    public enum RawInputType
    {
        /// <summary>
        ///     Mouse input.
        /// </summary>
        Mouse = 0,

        /// <summary>
        ///     Keyboard input.
        /// </summary>
        Keyboard = 1,

        /// <summary>
        ///     Human interface device input.
        /// </summary>
        HID = 2,

        /// <summary>
        ///     Another device that is not the keyboard or the mouse.
        /// </summary>
        Other = 3
    }
}