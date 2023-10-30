namespace VolumeControl.Core.Input.Exceptions
{
    /// <summary>
    /// Represents errors that occur because a type that does not implement <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/> was used as a DataTemplate provider in an action setting.
    /// </summary>
    public sealed class InvalidProviderTypeException : Exception
    {
        #region Constructors
        internal InvalidProviderTypeException(Type invalidProviderType, string? message, Exception? innerException)
            : base(message, innerException)
        {
            InvalidProviderType = invalidProviderType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the invalid provider type that caused the exception.
        /// </summary>
        public Type InvalidProviderType { get; }
        #endregion Properties

        #region Static Methods
        internal static InvalidProviderTypeException DoesNotImplementAnyRequiredInterface(Type invalidProviderType, Exception? innerException = null)
            => new(invalidProviderType, $"Type \"{invalidProviderType}\" is not a valid provider type because it does not implement {typeof(ITemplateProvider)} or {typeof(ITemplateDictionaryProvider)}!", innerException);
        #endregion Static Methods
    }
}
