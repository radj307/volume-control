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
        /// <param name="actionSettingType">The type of the action setting that wasn't found, or <see langword="null"/> if no type was specified.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        internal ActionSettingNotFoundException(string actionSettingName, Type? actionSettingType, string? message, Exception? innerException)
            : base(message: message, innerException: innerException)
        {
            ActionSettingName = actionSettingName;
            ActionSettingType = actionSettingType;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        /// <param name="actionSettingType">The type of the action setting that wasn't found, or <see langword="null"/> if no type was specified.</param>
        /// <param name="message">The message that describes the error.</param>
        internal ActionSettingNotFoundException(string actionSettingName, Type? actionSettingType, string? message)
            : base(message: message)
        {
            ActionSettingName = actionSettingName;
            ActionSettingType = actionSettingType;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with a message generated from the specified <paramref name="actionSettingName"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        internal ActionSettingNotFoundException(string actionSettingName)
            : base($"Couldn't find an action setting with name \"{actionSettingName}\"!")
        {
            ActionSettingName = actionSettingName;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingNotFoundException"/> instance with a message generated from the specified <paramref name="actionSettingName"/> and <paramref name="actionSettingType"/>.
        /// </summary>
        /// <param name="actionSettingName">The name of the action setting that wasn't found.</param>
        /// <param name="actionSettingType">The type of the action setting that wasn't found, or <see langword="null"/> if no type was specified.</param>
        internal ActionSettingNotFoundException(string actionSettingName, Type? actionSettingType)
            : base($"Couldn't find an action setting with name \"{actionSettingName}\" and type \"{actionSettingType.FullName}\"!")
        {
            ActionSettingName = actionSettingName;
            ActionSettingType = actionSettingType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the name of the requested setting that wasn't found.
        /// </summary>
        public string ActionSettingName { get; }
        /// <summary>
        /// Gets the type of the requested setting that wasn't found.
        /// </summary>
        public Type? ActionSettingType { get; }
        #endregion Properties
    }
}
