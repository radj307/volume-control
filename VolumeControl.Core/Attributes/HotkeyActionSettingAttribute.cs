using System.Runtime.CompilerServices;

namespace VolumeControl.Core.Attributes
{
    /// <summary>
    /// <b>(Optional)</b> Allows you to override the automatically generated action setting data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public sealed class HotkeyActionSettingAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with automatic type deduction.
        /// </summary>
        public HotkeyActionSettingAttribute() { }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with an explicit type.
        /// </summary>
        /// <param name="settingType">The <see cref="Type"/> of value used by this setting.</param>
        /// <param name="settingLabel">The label to show next to the setting. Defaults to the name of the parameter.</param>
        public HotkeyActionSettingAttribute(Type? settingType, string? settingLabel = null)
        {
            SettingType = settingType;
            SettingLabel = settingLabel;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSettingAttribute"/> instance with an explicit type.
        /// </summary>
        /// <param name="settingLabel">The label to show next to the setting. Defaults to the name of the parameter.</param>
        /// <param name="settingType">The <see cref="Type"/> of value used by this setting.</param>
        public HotkeyActionSettingAttribute(string? settingLabel, Type? settingType = null)
        {
            SettingLabel = settingLabel;
            SettingType = settingType;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the label shown in the action settings menu.
        /// </summary>
        public string? SettingLabel { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of value that this setting holds.
        /// </summary>
        public Type? SettingType { get; set; }
        #endregion Properties
    }
}
