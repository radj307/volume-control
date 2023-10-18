using System.Reflection;
using System.Windows;
using VolumeControl.Core.Attributes;
using VolumeControl.Log;

namespace VolumeControl.Core.Input.Actions.Settings
{
    /// <summary>
    /// Defines an action setting, and provides methods for creating action setting instances.
    /// </summary>
    /// <remarks>
    /// This class is created automatically when a hotkey action addon method is marked with <see cref="HotkeyActionSettingAttribute"/>.
    /// </remarks>
    public class ActionSettingDefinition
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ActionSettingDefinition"/> instance with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the action setting.</param>
        /// <param name="valueType">The type of value contained by the action setting.</param>
        /// <param name="defaultValue">The default value of the action setting.</param>
        /// <param name="dataTemplateProviderType">The type of WPF DataTemplate to use for displaying the action setting's value editor control.</param>
        /// <param name="description">The description of the action setting.</param>
        internal ActionSettingDefinition(string name, Type valueType, object? defaultValue, Type? dataTemplateProviderType, string? description)
        {
            Name = name;
            Description = description;
            ValueType = valueType;
            DefaultValue = defaultValue;

            if (dataTemplateProviderType != null && dataTemplateProviderType.IsSubclassOf(typeof(DataTemplateProvider)))
            { // create a DataTemplate instance using the provider:
                DataTemplateProvider? provider = null;
                try
                {
                    provider = (DataTemplateProvider)Activator.CreateInstance(dataTemplateProviderType)!;
                }
                catch (Exception ex) // DataTemplateProvider instantiation failed
                {
                    if (FLog.Log.FilterEventType(Log.Enum.EventType.ERROR))
                        FLog.Log.Error($"{dataTemplateProviderType.FullName} instantiation failed due to an exception:", ex);
                }
                if (provider == null) return;
                try
                {
                    DataTemplate = provider.ProvideDataTemplate();
                }
                catch (Exception ex) // ProvideDataTemplate() failed
                {
                    if (FLog.Log.FilterEventType(Log.Enum.EventType.ERROR))
                        FLog.Log.Error($"{dataTemplateProviderType.FullName}.{nameof(DataTemplateProvider.ProvideDataTemplate)}() failed due to an exception:", ex);
                }
            }
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
        #endregion Properties

        #region Operators
        /// <summary>
        /// Conversion operator for casting from <see cref="HotkeyActionSettingAttribute"/> to <see cref="ActionSettingDefinition"/>.
        /// </summary>
        /// <param name="actionSettingAttribute">An <see cref="HotkeyActionSettingAttribute"/> instance.</param>
        public static explicit operator ActionSettingDefinition(HotkeyActionSettingAttribute actionSettingAttribute)
            => new(actionSettingAttribute.Name, actionSettingAttribute.ValueType, actionSettingAttribute.DefaultValue, actionSettingAttribute.DataTemplateProviderType, actionSettingAttribute.Description);
        #endregion Operators

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
                    CreateValueInstance()
                },
                System.Globalization.CultureInfo.CurrentCulture)!;
        }
        /// <summary>
        /// Creates a new <see cref="ActionSettingInstance{T}"/> instance from this <see cref="ActionSettingDefinition"/> and the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A default value for the action setting instance.</param>
        /// <inheritdoc cref="CreateInstance()"/>
        public IActionSettingInstance CreateInstance(object? value)
        {
            return (IActionSettingInstance)Activator.CreateInstance(
                typeof(ActionSettingInstance<>).MakeGenericType(ValueType),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[]
                { // constructor parameters:
                    this,
                    value ?? CreateValueInstance()
                },
                System.Globalization.CultureInfo.CurrentCulture)!;
        }
        #endregion Methods
    }
}
