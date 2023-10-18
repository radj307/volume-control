namespace VolumeControl.Core.Input.Actions.Settings
{
    /// <summary>
    /// Represents an action setting instance.
    /// </summary>
    public interface IActionSettingInstance
    {
        /// <summary>
        /// Gets the definition object for this action setting instance.
        /// </summary>
        ActionSettingDefinition ActionSettingDefinition { get; }
        /// <summary>
        /// Gets the name of this action setting instance.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the description of this action setting instance.
        /// </summary>
        string? Description { get; }
        /// <summary>
        /// Gets or sets the value of this action setting instance.
        /// </summary>
        object? Value { get; set; }
        /// <summary>
        /// Gets the value type of this action setting instance.
        /// </summary>
        Type ValueType { get; }
    }
    /// <summary>
    /// Represents a strongly-typed action setting instance.
    /// </summary>
    /// <typeparam name="T">The value type of this action setting.</typeparam>
    public interface IActionSettingInstance<T>
    {
        /// <summary>
        /// Gets the definition object for this action setting instance.
        /// </summary>
        ActionSettingDefinition ActionSettingDefinition { get; }
        /// <summary>
        /// Gets the name of this action setting instance.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the description of this action setting instance.
        /// </summary>
        string? Description { get; }
        /// <summary>
        /// Gets or sets the value of this action setting instance.
        /// </summary>
        T? Value { get; set; }
        /// <summary>
        /// Gets the value type of this action setting instance.
        /// </summary>
        Type ValueType { get; }
    }
}
