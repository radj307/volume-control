using VolumeControl.Core.Input.Actions.Settings;

namespace VolumeControl.Core.Input.Exceptions
{
    /// <summary>
    /// Represents type errors that occur from the <see cref="ActionSettingInstance{T}"/> class.
    /// </summary>
    public sealed class InvalidActionSettingValueTypeException : Exception
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="InvalidActionSettingValueTypeException"/> instance with the specified <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="actualType">The invalid type that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        internal InvalidActionSettingValueTypeException(Type? actualType, Type? expectedType, string message, Exception? innerException) : base(message: message, innerException: innerException)
        {
            ActualType = actualType;
            ExpectedType = expectedType;
        }
        /// <summary>
        /// Creates a new <see cref="InvalidActionSettingValueTypeException"/> instance with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="actualType">The invalid type that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        internal InvalidActionSettingValueTypeException(Type? actualType, Type? expectedType, string message) : base(message: message)
        {
            ActualType = actualType;
            ExpectedType = expectedType;
        }
        /// <summary>
        /// Creates a new <see cref="InvalidActionSettingValueTypeException"/> instance with a message generated from the given types.
        /// </summary>
        /// <param name="actualType">The invalid type that was received.</param>
        /// <param name="expectedType">The valid type that was expected.</param>
        internal InvalidActionSettingValueTypeException(Type? actualType, Type? expectedType)
            : base($"Value of type {(actualType == null ? "(null)" : $"'{actualType.FullName}'")} is invalid here; this action setting expects values of type {(expectedType == null ? "(null)" : $"'{expectedType.FullName}'")}!")
        {
            ActualType = actualType;
            ExpectedType = expectedType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the invalid type that caused this exception to be thrown.
        /// </summary>
        public Type? ActualType { get; }
        public Type? ExpectedType { get; }
        #endregion Properties
    }
}
