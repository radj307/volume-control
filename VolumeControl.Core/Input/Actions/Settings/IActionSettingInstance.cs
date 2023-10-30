using System.ComponentModel;

namespace VolumeControl.Core.Input.Actions.Settings
{
    /// <summary>
    /// Represents an action setting instance.
    /// </summary>
    public interface IActionSettingInstance : INotifyPropertyChanged
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
        /// <summary>
        /// Gets the default value of this action setting instance.
        /// </summary>
        object? DefaultValue { get; }
        /// <summary>
        /// Gets whether this action setting instance can be toggled on/off.
        /// </summary>
        bool IsToggleable { get; }
        /// <summary>
        /// Gets or sets whether this action setting instance is enabled.
        /// </summary>
        /// <remarks>
        /// When IsToggleable is <see langword="false"/>, this always returns <see langword="true"/> and cannot be changed.
        /// </remarks>
        /// <returns><see langword="true"/> when enabled or not toggleable; <see langword="false"/> when disabled.</returns>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Deconstructs the action setting into a tuple that includes whether the setting is enabled or not, and its value.
        /// </summary>
        /// <param name="isEnabled"><see langword="true"/> when the setting is enabled (or isn't toggleable); otherwise <see langword="false"/>.</param>
        /// <param name="value">The value of the setting.</param>
        void Deconstruct(out bool isEnabled, out object? value);
    }
    /// <summary>
    /// Represents a strongly-typed action setting instance.
    /// </summary>
    /// <typeparam name="T">The value type of this action setting.</typeparam>
    public interface IActionSettingInstance<T> : INotifyPropertyChanged
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
        /// <summary>
        /// Gets the default value of this action setting instance.
        /// </summary>
        T? DefaultValue { get; }
        /// <summary>
        /// Gets whether this action setting instance can be toggled on/off.
        /// </summary>
        bool IsToggleable { get; }
        /// <summary>
        /// Gets or sets whether this action setting instance is enabled.
        /// </summary>
        /// <returns><see langword="true"/> when enabled; <see langword="false"/> when disabled; <see langword="null"/> when IsToggleable is <see langword="false"/>.</returns>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Deconstructs the action setting into a tuple that includes whether the setting is enabled or not, and its value.
        /// </summary>
        /// <param name="isEnabled"><see langword="true"/> when the setting is enabled (or isn't toggleable); otherwise <see langword="false"/>.</param>
        /// <param name="value">The value of the setting.</param>
        void Deconstruct(out bool isEnabled, out T? value);
    }
}
