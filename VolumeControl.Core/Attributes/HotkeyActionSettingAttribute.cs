using VolumeControl.Core.Input.Actions;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Attribute that specifies a hotkey action setting. This can be applied to methods, or directly to parameters to override automatic label/type deduction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class HotkeyActionSettingAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with an explicit type.
        /// </summary>
        /// <param name="label">The label to show next to the setting. Defaults to the name of the parameter.</param>
        /// <param name="valueType">The <see cref="Type"/> of value used by this setting.</param>
        public HotkeyActionSettingAttribute(string label, Type? valueType = null)
        {
            Label = label;
            ValueType = valueType;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with an explicit type.
        /// </summary>
        /// <param name="label">The label to show next to the setting. Defaults to the name of the parameter.</param>
        /// <param name="valueType">The <see cref="Type"/> of value used by this setting.</param>
        /// <param name="description">A brief description of this setting to show in a tooltip.</param>
        public HotkeyActionSettingAttribute(string label, Type? valueType, string description)
        {
            Label = label;
            ValueType = valueType;
            Description = description;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the label shown in the action settings menu.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of value that this setting holds.
        /// </summary>
        public Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the description that is shown in a tooltip in the action settings menu.
        /// </summary>
        public string? Description { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> using this <see cref="HotkeyActionSettingAttribute"/>'s properties.
        /// </summary>
        /// <returns>A new <see cref="HotkeyActionSetting"/> instance.</returns>
        public HotkeyActionSetting ToHotkeyActionSetting() => new(this);
        #endregion Methods
    }
}
