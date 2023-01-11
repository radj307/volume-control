using VolumeControl.Core.Input.Actions;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// Attribute that specifies a hotkey action setting. This can be applied to methods, or directly to parameters to override automatic label/type deduction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HotkeyActionSettingAttribute : Attribute
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with an explicit type.
        /// </summary>
        /// <param name="label">The label to show next to the setting. Defaults to the name of the parameter.</param>
        /// <param name="valueType">The <see cref="Type"/> of value used by this setting.</param>
        public HotkeyActionSettingAttribute(string label, Type? valueType = null)
        {
            SettingLabel = label;
            SettingType = valueType;
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets or sets the label shown in the action settings menu.
        /// </summary>
        public string SettingLabel { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of value that this setting holds.
        /// </summary>
        public Type? SettingType { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> using this <see cref="HotkeyActionSettingAttribute"/>'s properties.
        /// </summary>
        /// <returns>A new <see cref="HotkeyActionSetting"/> instance.</returns>
        public HotkeyActionSetting ToHotkeyActionSetting() => new(SettingLabel, SettingType);
        #endregion Methods

        #region Operators
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> using this <see cref="HotkeyActionSettingAttribute"/>'s properties.
        /// </summary>
        /// <param name="attribute">A <see cref="HotkeyActionSettingAttribute"/> instance.</param>
        public static explicit operator HotkeyActionSetting(HotkeyActionSettingAttribute attribute) =>
            new(
                label: attribute.SettingLabel,
                valueType: attribute.SettingType
            );
        #endregion Operators
    }
}
