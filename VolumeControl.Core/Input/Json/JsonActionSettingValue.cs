using Newtonsoft.Json;
using VolumeControl.Core.Input.Actions.Settings;

namespace VolumeControl.Core.Input.Json
{
    /// <summary>
    /// JSON container for toggleable action settings.
    /// </summary>
    /// <remarks>
    /// This object is only serialized directly to JSON when <see cref="IActionSettingInstance.IsToggleable"/> is <see langword="true"/>; otherwise, only the <see cref="Value"/> is written instead.
    /// </remarks>
    [JsonConverter(typeof(JsonActionSettingValueConverter))]
    public struct JsonActionSettingValue
    {
        #region Constructors
        internal JsonActionSettingValue(IActionSettingInstance settingInstance)
        {
            Enabled = settingInstance.IsToggleable ? settingInstance.IsEnabled : null;
            Value = settingInstance.Value;
        }
        internal JsonActionSettingValue(bool? isEnabled, object? value)
        {
            Enabled = isEnabled;
            Value = value;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets whether the action setting value can be toggled on/off, and if it can, whether it is enabled or disabled.
        /// </summary>
        /// <returns><see langword="null"/> when not toggleable; <see langword="true"/> when toggleable and enabled; <see langword="false"/> when toggleable and disabled.</returns>
        public bool? Enabled { get; set; }
        /// <summary>
        /// Gets or sets the value of the action setting.
        /// </summary>
        public object? Value { get; set; }
        #endregion Properties
    }
}
