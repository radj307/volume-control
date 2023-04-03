using Newtonsoft.Json;
using PropertyChanged;
using VolumeControl.Core.Attributes;

namespace VolumeControl.Core.Input.Actions
{
    /// <summary>
    /// Small container for hotkey action setting data
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    [JsonObject]
    public class HotkeyActionSetting : IHotkeyActionSetting
    {
        #region Constructors
        /// <summary>
        /// Creates a new empty <see cref="HotkeyActionSetting"/> instance.
        /// </summary>
        public HotkeyActionSetting() { }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> instance by copying a <see cref="HotkeyActionSettingAttribute"/> instance.
        /// </summary>
        /// <param name="attr">A <see cref="HotkeyActionSettingAttribute"/> instance.</param>
        public HotkeyActionSetting(HotkeyActionSettingAttribute attr)
        {
            Label = attr.Label;
            ValueType = attr.ValueType;
            Value = ValueType is null
                ? null
                : (ValueType.Equals(typeof(string)) ? string.Empty : Activator.CreateInstance(ValueType));
            Description = attr.Description;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionSetting"/> instance by copying another <see cref="HotkeyActionSetting"/> instance.
        /// </summary>
        /// <param name="copyInstance">An already-created instance of <see cref="HotkeyActionSetting"/> to copy.</param>
        public HotkeyActionSetting(HotkeyActionSetting copyInstance)
        {
            Label = copyInstance.Label;
            ValueType = copyInstance.ValueType;
            Value = copyInstance.Value;
            Description = copyInstance.Description;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the name of this setting
        /// </summary>
        [JsonIgnore]
        public string Label { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the <see cref="ValueType"/> of <see cref="Value"/> accepted by this setting.<br/>
        /// Setting this to <see langword="null"/> will allow <see cref="Value"/> to be set to any type.
        /// </summary>
        [JsonIgnore]
        public Type? ValueType { get; set; }
        /// <summary>
        /// Gets or sets the value of this setting
        /// </summary>
        public object? Value { get; set; }
        /// <summary>
        /// Gets or sets the description of this setting, which is shown in a tooltip in the action settings window.
        /// </summary>
        [JsonIgnore]
        public string? Description { get; set; }
        #endregion Properties
    }
}
