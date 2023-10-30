using System.Diagnostics;
using System.Reflection;
using System.Windows;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Input.Exceptions;
using VolumeControl.Log;

namespace VolumeControl.Core.Input.Actions.Settings
{
    /// <summary>
    /// Defines an action setting, and provides methods for creating action setting instances.
    /// </summary>
    /// <remarks>
    /// This class is created automatically when a hotkey action addon method is marked with <see cref="HotkeyActionSettingAttribute"/>.
    /// </remarks>
    [DebuggerDisplay("Name = {Name}, ValueType = {ValueType}")]
    public class ActionSettingDefinition
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ActionSettingDefinition"/> instance with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the action setting.</param>
        /// <param name="valueType">The type of value contained by the action setting.</param>
        /// <param name="defaultValue">The default value of the action setting.</param>
        /// <param name="description">The description of the action setting.</param>
        /// <param name="isToggleable">Whether the action setting can be toggled.</param>
        /// <param name="startsEnabled">Whether the action setting is enabled by default. Has no effect when IsToggleable is <see langword="false"/>.</param>
        /// <param name="dataTemplate">The DataTemplate to use for the value editor control(s).</param>
        internal ActionSettingDefinition(string name, Type valueType, object? defaultValue, string? description, bool isToggleable, bool startsEnabled, DataTemplate? dataTemplate)
        {
            Name = name;
            Description = description;
            ValueType = valueType;
            IsToggleable = isToggleable;
            StartsEnabled = startsEnabled;
            DataTemplate = dataTemplate;

            if (defaultValue?.GetType() is Type defaultValueType && !valueType.IsAssignableFrom(defaultValueType))
            {
                var invalidTypeException = new InvalidActionSettingValueTypeException(defaultValueType, valueType, $"The default value of action setting \"{name}\" is an invalid type! Expected a value of type \"{valueType}\", received \"{defaultValue}\" with type \"{defaultValueType}\".");
                FLog.Error(invalidTypeException);
#if DEBUG
                throw invalidTypeException;
#endif
            }
            else DefaultValue = defaultValue;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the name of the action setting.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the description string of the action setting.
        /// </summary>
        public string? Description { get; }
        /// <summary>
        /// Gets the type of value that the action setting contains.
        /// </summary>
        public Type ValueType { get; }
        /// <summary>
        /// Gets the default value of the action setting.
        /// </summary>
        public object? DefaultValue { get; }
        /// <summary>
        /// Gets the WPF DataTemplate to use for displaying the value editor control in the GUI.
        /// </summary>
        public DataTemplate? DataTemplate { get; }
        /// <summary>
        /// Gets whether the action setting can be toggled on/off.
        /// </summary>
        public bool IsToggleable { get; }
        /// <summary>
        /// Gets whether the action setting is enabled by default or not.
        /// </summary>
        /// <remarks>
        /// Has no effect when IsToggleable is <see langword="false"/>.
        /// </remarks>
        public bool StartsEnabled { get; }
        #endregion Properties

        #region Methods
        internal object? CreateValueInstance()
        {
            if (DefaultValue == null && ValueType == typeof(string))
                return string.Empty; //< string doesn't have a default constructor, prevent exception by returning an empty string
            return DefaultValue ?? Activator.CreateInstance(ValueType);
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingInstance{T}"/> instance from this <see cref="ActionSettingDefinition"/>.
        /// </summary>
        /// <returns>A new <see cref="IActionSettingInstance"/> instance.</returns>
        public IActionSettingInstance CreateInstance()
        {
            return (IActionSettingInstance)Activator.CreateInstance(
                typeof(ActionSettingInstance<>).MakeGenericType(ValueType),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[]
                { // constructor parameters:
                    this,
                    IsToggleable && StartsEnabled,
                    CreateValueInstance()
                },
                System.Globalization.CultureInfo.CurrentCulture)!;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingInstance{T}"/> instance from this <see cref="ActionSettingDefinition"/> and the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="enabled">Whether the action setting instance should start enabled. This does not have any effect if the setting is not toggleable.</param>
        /// <param name="value">A default value for the action setting instance.</param>
        /// <inheritdoc cref="CreateInstance()"/>
        public IActionSettingInstance CreateInstance(bool? enabled, object? value)
        {
            return (IActionSettingInstance)Activator.CreateInstance(
                typeof(ActionSettingInstance<>).MakeGenericType(ValueType),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[]
                { // constructor parameters:
                    this,
                    enabled,
                    value ?? CreateValueInstance()
                },
                System.Globalization.CultureInfo.CurrentCulture)!;
        }
        #endregion Methods
    }
}
