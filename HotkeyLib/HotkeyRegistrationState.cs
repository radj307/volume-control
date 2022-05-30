namespace HotkeyLib
{
    /// <summary>
    /// Enum containing the various states that a hotkey registration may be in.
    /// </summary>
    public enum HotkeyRegistrationState : byte
    {
        /// <summary>
        /// Hotkey is not registered.
        /// </summary>
        UNREGISTERED,
        /// <summary>
        /// Hotkey is registered.
        /// </summary>
        REGISTERED,
        /// <summary>
        /// The previous attempt to register the hotkey failed, and it is now in a failed state.<br/>
        /// This can usually be cleared by simply re-registering it.
        /// </summary>
        FAILED,
    }
}
