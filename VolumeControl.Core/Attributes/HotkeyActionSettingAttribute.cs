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
        /// <param name="dataTemplateProviderType">The type of the WPF DataTemplate to use for providing an editor control for the GUI.<br/>
        /// See <see cref="DataTemplateProviderType"/> for more information.</param>
        public HotkeyActionSettingAttribute(string name, Type valueType, Type dataTemplateProviderType)
        {
            Name = name;
            ValueType = valueType;
            DataTemplateProviderType = dataTemplateProviderType;
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
        /// Gets or sets the type of <see cref="DataTemplateProvider"/> used to provide a UI editor control specific to this action setting's ValueType.<br/>
        /// Only types that derive from <see cref="DataTemplateProvider"/> are allowed.
        /// </summary>
        /// <remarks>
        /// When this is <see langword="null"/>, the default DataTemplate for the specified ValueType is used instead, if one is available.<br/>
        /// See the documentation for <see cref="DataTemplateProvider"/> for more information.
        /// </remarks>
        /// <exception cref="ArgumentException">The specified type is not a subclass of <see cref="DataTemplateProvider"/>!</exception>
        public Type? DataTemplateProviderType
        {
            get => _dataTemplateProviderType;
            set
            {
                // check if the incoming type is derived from DataTemplateProvider:
                if (value != null && !value.IsSubclassOf(typeof(DataTemplateProvider)))
                    throw new ArgumentException($"{value.FullName} is not a valid {nameof(DataTemplateProviderType)} because it does not inherit from {typeof(DataTemplateProvider).FullName}!", nameof(value));

                _dataTemplateProviderType = value;
            }
        }
        private Type? _dataTemplateProviderType;
        #endregion Properties
    }
}
