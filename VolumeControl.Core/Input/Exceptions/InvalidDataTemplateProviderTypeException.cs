namespace VolumeControl.Core.Input.Exceptions
{
    /// <summary>
    /// Represents errors that occur when the specified <see cref="Attributes.HotkeyActionSettingAttribute.DataTemplateProviderType"/> does not implement <see cref="ITemplateProvider"/>.
    /// </summary>
    public sealed class InvalidDataTemplateProviderTypeException : Exception
    {
        #region Constructors
        internal InvalidDataTemplateProviderTypeException(Type? invalidProviderType, string? message, Exception? innerException) : base(message, innerException)
        {
            InvalidProviderType = invalidProviderType;
        }
        internal InvalidDataTemplateProviderTypeException(Type? invalidProviderType, string? message) : base(message)
        {
            InvalidProviderType = invalidProviderType;
        }
        internal InvalidDataTemplateProviderTypeException(Type? invalidProviderType)
            : base($"Type \"{invalidProviderType}\" is not a valid data template provider because it does not implement \"{typeof(ITemplateProvider)}\"!")
        {
            InvalidProviderType = invalidProviderType;
        }
        internal InvalidDataTemplateProviderTypeException(Type? invalidProviderType, Exception? innerException)
            : base($"Type \"{invalidProviderType}\" is not a valid data template provider because it does not implement \"{typeof(ITemplateProvider)}\"!", innerException)
        {
            InvalidProviderType = invalidProviderType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the invalid <see cref="ITemplateProvider"/> type that caused the exception.
        /// </summary>
        public Type? InvalidProviderType { get; }
        #endregion Properties
    }
}
