namespace VolumeControl.Core.Input.Exceptions
{
    /// <summary>
    /// Represents errors that occur when a hotkey action setting wasn't found in a <see cref="HotkeyActionPressedEventArgs"/> instance.
    /// </summary>
    public sealed class ActionSettingNotFoundException : Exception
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with the specified <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        internal ActionSettingNotFoundException(string actionSettingName, string message, Exception? innerException)
            : base(message: message, innerException: innerException)
        {
            ActionSettingName = actionSettingName;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        /// <param name="message">The message that describes the error.</param>
        internal ActionSettingNotFoundException(string actionSettingName, string message)
            : base(message: message)
        {
            ActionSettingName = actionSettingName;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with a message generated from the specified <paramref name="actionSettingName"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        internal ActionSettingNotFoundException(string actionSettingName)
            : base($"No setting by the name of '{actionSettingName}' was found!")
        {
            ActionSettingName = actionSettingName;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the name of the action setting that was not found.
        /// </summary>
        public string ActionSettingName { get; }
        #endregion Properties
    }
}
