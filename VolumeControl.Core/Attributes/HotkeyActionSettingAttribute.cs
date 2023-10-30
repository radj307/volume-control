namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Specifies an action setting for the associated hotkey action method.
    /// </summary>
    /// <remarks>
    /// This attribute can only be used on methods that are also marked with <see cref="HotkeyActionAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class HotkeyActionSettingAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with the specified <paramref name="name"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="name">The name of this action setting.</param>
        /// <param name="valueType">The type of value that this action setting contains.</param>
        public HotkeyActionSettingAttribute(string name, Type valueType)
        {
            Name = name;
            ValueType = valueType;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with the specified <paramref name="name"/>, <paramref name="valueType"/>, and <paramref name="dataTemplateProviderType"/>.
        /// </summary>
        /// <param name="name">The name of this action setting.</param>
        /// <param name="valueType">The type of value that this action setting contains.</param>
        /// <param name="dataTemplateProviderType">The type of the WPF DataTemplate to use for providing an editor control for the GUI.<br/>See <see cref="DataTemplateProviderType"/> for more information.</param>
        public HotkeyActionSettingAttribute(string name, Type valueType, Type dataTemplateProviderType)
        {
            Name = name;
            ValueType = valueType;
            DataTemplateProviderType = dataTemplateProviderType;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with the specified <paramref name="name"/>, <paramref name="valueType"/>, and <paramref name="dataTemplateKey"/>.
        /// </summary>
        /// <param name="name">The name of this action setting.</param>
        /// <param name="valueType">The type of value that this action setting contains.</param>
        /// <param name="dataTemplateKey"><see cref="string"/> containing the key name of the target <see cref="ActionSettingDataTemplate"/> instance to use.</param>
        public HotkeyActionSettingAttribute(string name, Type valueType, string dataTemplateKey)
        {
            Name = name;
            ValueType = valueType;
            DataTemplateProviderKey = dataTemplateKey;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with the specified <paramref name="name"/>, <paramref name="valueType"/>, <paramref name="dataTemplateProviderType"/>, and <paramref name="dataTemplateKey"/>.
        /// </summary>
        /// <param name="name">The name of this action setting.</param>
        /// <param name="valueType">The type of value that this action setting contains.</param>
        /// <param name="dataTemplateProviderType">The type of the WPF DataTemplate to use for providing an editor control for the GUI.<br/>See <see cref="DataTemplateProviderType"/> for more information.</param>
        /// <param name="dataTemplateKey"><see cref="string"/> containing the key name of the target <see cref="ActionSettingDataTemplate"/> instance to use.</param>
        public HotkeyActionSettingAttribute(string name, Type valueType, Type dataTemplateProviderType, string dataTemplateKey)
        {
            Name = name;
            ValueType = valueType;
            DataTemplateProviderType = dataTemplateProviderType;
            DataTemplateProviderKey = dataTemplateKey;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the name of this action setting.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description string for this action setting.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the type of this action setting.
        /// </summary>
        public Type ValueType { get; set; }
        /// <summary>
        /// Gets or sets the default value of this action setting.
        /// </summary>
        public object? DefaultValue { get; set; }
        /// <summary>
        /// Gets or sets whether this action setting can be toggled on/off.
        /// </summary>
        /// <remarks>
        /// When <see langword="true"/> a checkbox control is shown in the UI to allow users to enable or disable the setting.<br/>
        /// You can determine whether an action setting instance is enabled or not by checking the <see cref="Input.Actions.Settings.IActionSettingInstance.IsEnabled"/> property.
        /// </remarks>
        public bool IsToggleable { get; set; }
        /// <summary>
        /// Gets or sets whether this action setting is enabled by default or not.
        /// </summary>
        /// <remarks>
        /// This has no effect when IsToggleable is set to <see langword="false"/>.
        /// </remarks>
        public bool StartsEnabled { get; set; }
        /// <summary>
        /// Gets or sets the type of the data template provider to use for this action setting.
        /// Only types that implement <see cref="ITemplateProvider"/> or <see cref="ITemplateDictionaryProvider"/> are allowed.
        /// </summary>
        /// <remarks>
        /// When this is <see langword="null"/>, the default DataTemplate for the specified ValueType is used instead, if one is available.<br/>
        /// See the documentation for <see cref="ITemplateProvider"/> &amp; <see cref="ITemplateDictionaryProvider"/> for more information.
        /// </remarks>
        public Type? DataTemplateProviderType { get; set; }
        /// <summary>
        /// Gets or sets the name of a specific provided data template to use.
        /// </summary>
        public string? DataTemplateProviderKey { get; set; }
        #endregion Properties
    }
}
