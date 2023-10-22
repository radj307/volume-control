using System.ComponentModel;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.Core.Input.Exceptions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Event arguments for hotkey action events.
    /// </summary>
    public class HotkeyActionPressedEventArgs : HandledEventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance without any action settings.
        /// </summary>
        public HotkeyActionPressedEventArgs()
        {
            Settings = Array.Empty<IActionSettingInstance>();
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance without any action settings.
        /// </summary>
        /// <param name="defaultHandledValue">The default value for the <see cref="HandledEventArgs.Handled"/> property.</param>
        public HotkeyActionPressedEventArgs(bool defaultHandledValue) : base(defaultHandledValue)
        {
            Settings = Array.Empty<IActionSettingInstance>();
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance with the specified <paramref name="actionSettings"/>.
        /// </summary>
        /// <param name="actionSettings">The array of settings to send to the action method.</param>
        public HotkeyActionPressedEventArgs(IActionSettingInstance[] actionSettings)
        {
            Settings = actionSettings;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyActionPressedEventArgs"/> instance with the specified <paramref name="actionSettings"/>.
        /// </summary>
        /// <param name="actionSettings">The array of settings to send to the action method.</param>
        /// <param name="defaultHandledValue">The default value for the <see cref="HandledEventArgs.Handled"/> property.</param>
        public HotkeyActionPressedEventArgs(IActionSettingInstance[] actionSettings, bool defaultHandledValue) : base(defaultHandledValue)
        {
            Settings = actionSettings;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the array of ActionSettings configured for the hotkey that triggered the event.
        /// </summary>
        public IActionSettingInstance[] Settings { get; }
        #endregion Properties

        #region Methods

        #region GetSetting
        /// <summary>
        /// Gets the action setting with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <returns><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/>.</exception>
        public IActionSettingInstance GetSetting(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var setting = Settings.FirstOrDefault(setting => setting.Name.Equals(name, stringComparison));

            if (setting == null)
                throw new ActionSettingNotFoundException(name);

            return setting;
        }
        /// <summary>
        /// Gets the action setting with the specified <paramref name="name"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="valueType">The value type of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <returns><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</exception>
        public IActionSettingInstance GetSetting(string name, Type valueType, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var setting = Settings.FirstOrDefault(setting => setting.ValueType.Equals(valueType) && setting.Name.Equals(name, stringComparison));

            if (setting == null)
                throw new ActionSettingNotFoundException(name);

            return setting;
        }
        /// <summary>
        /// Gets the action setting with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="T">The value type of the setting to get.</typeparam>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <returns><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/> and value type.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> and value type.</exception>
        public IActionSettingInstance<T> GetSetting<T>(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var type = typeof(T);
            var setting = (IActionSettingInstance<T>?)Settings.FirstOrDefault(setting => setting.ValueType.Equals(type) && setting.Name.Equals(name, stringComparison));

            if (setting == null)
                throw new ActionSettingNotFoundException(name, type);

            return setting;
        }
        #endregion GetSetting

        #region TryGetSetting
        /// <summary>
        /// Attempts to get the action setting with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <param name="setting"><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/>.</param>
        /// <returns><see langword="true"/> when the setting was found and isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetSetting(string name, StringComparison stringComparison, out IActionSettingInstance setting)
        {
            try
            {
                setting = GetSetting(name, stringComparison);
                return setting != null;
            }
            catch
            {
                setting = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryGetSetting(string, StringComparison, out IActionSettingInstance)"/>
        public bool TryGetSetting(string name, out IActionSettingInstance setting)
            => TryGetSetting(name, StringComparison.Ordinal, out setting);
        /// <summary>
        /// Attempts to get the action setting with the specified <paramref name="name"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="valueType">The value type of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <param name="setting"><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</param>
        /// <returns><see langword="true"/> when the setting was found and isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetSetting(string name, Type valueType, StringComparison stringComparison, out IActionSettingInstance setting)
        {
            try
            {
                setting = GetSetting(name, valueType, stringComparison);
                return setting != null;
            }
            catch
            {
                setting = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryGetSetting(string, Type, StringComparison, out IActionSettingInstance)"/>
        public bool TryGetSetting(string name, Type valueType, out IActionSettingInstance setting)
            => TryGetSetting(name, valueType, StringComparison.Ordinal, out setting);
        /// <summary>
        /// Gets the action setting with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="T">The value type of the setting to get.</typeparam>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <param name="setting"><see cref="IActionSettingInstance"/> with the specified <paramref name="name"/> and value type.</param>
        /// <returns><see langword="true"/> when the setting was found and isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetSetting<T>(string name, StringComparison stringComparison, out IActionSettingInstance<T> setting)
        {
            try
            {
                setting = GetSetting<T>(name, stringComparison);
                return setting != null;
            }
            catch
            {
                setting = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryGetSetting{T}(string, StringComparison, out IActionSettingInstance{T})"/>
        public bool TryGetSetting<T>(string name, out IActionSettingInstance<T> setting)
            => TryGetSetting(name, out setting);
        #endregion TryGetSetting

        #region GetValue
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>Value of the setting with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/>.</exception>
        public object GetValue(string name, StringComparison stringComparison = StringComparison.Ordinal)
            => GetSetting(name, stringComparison)?.Value!;
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="valueType">The value type of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <returns>Value of the setting with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</exception>
        public object GetValue(string name, Type valueType, StringComparison stringComparison = StringComparison.Ordinal)
            => GetSetting(name, valueType, stringComparison)?.Value!;
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="T">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>Value of the setting with the specified <paramref name="name"/> and value type.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> and value type.</exception>
        public T GetValue<T>(string name, StringComparison stringComparison = StringComparison.Ordinal)
            => GetSetting<T>(name, stringComparison).Value!;
        #endregion GetValue

        #region TryGetValue
        /// <summary>
        /// Attempts to get the value of the action setting with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="value">Value of the setting with the specified <paramref name="name"/>.</param>
        /// <returns><see langword="true"/> when the setting was found and the value isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetValue(string name, StringComparison stringComparison, out object value)
        {
            try
            {
                value = GetValue(name, stringComparison)!;
                return value != null;
            }
            catch
            {
                value = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryGetValue(string, StringComparison, out object)"/>
        public bool TryGetValue(string name, out object value)
            => TryGetValue(name, StringComparison.Ordinal, out value);
        /// <summary>
        /// Attempts to get the value of the action setting with the specified <paramref name="name"/> and <paramref name="valueType"/>.
        /// </summary>
        /// <param name="name">The name of the setting to get.</param>
        /// <param name="valueType">The value type of the setting to get.</param>
        /// <param name="stringComparison">The comparison type to use when performing string comparisons.</param>
        /// <param name="value">Value of the setting with the specified <paramref name="name"/> &amp; <paramref name="valueType"/>.</param>
        /// <returns><see langword="true"/> when the setting was found and the value isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetValue(string name, Type valueType, StringComparison stringComparison, out object value)
        {
            try
            {
                value = GetValue(name, valueType, stringComparison)!;
                return value != null;
            }
            catch
            {
                value = null!;
                return false;
            }
        }
        /// <inheritdoc cref="TryGetValue(string, Type, StringComparison, out object)"/>
        public bool TryGetValue(string name, Type valueType, out object value)
            => TryGetValue(name, valueType, StringComparison.Ordinal, out value);
        /// <summary>
        /// Attempts to get the value of the action setting with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="T">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="value">Value of the setting with the specified <paramref name="name"/> and value type.</param>
        /// <returns><see langword="true"/> when the setting was found and the value isn't <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetValue<T>(string name, StringComparison stringComparison, out T value)
        {
            try
            {
                value = GetValue<T>(name, stringComparison)!;
                return value != null;
            }
            catch
            {
                value = default!;
                return false;
            }
        }
        #endregion TryGetValue

        #region GetValueOrDefault
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/> if it isn't <see langword="null"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="defaultValue">The value to return if the setting wasn't found, or if its value is <see langword="null"/>.</param>
        /// <returns>The value of the setting with the specified <paramref name="name"/> if it exists and isn't <see langword="null"/>; otherwise <paramref name="defaultValue"/>.</returns>
        public object GetValueOrDefault(string name, StringComparison stringComparison, object defaultValue = default!)
        {
            if (TryGetSetting(name, stringComparison, out var setting))
                return setting.Value ?? defaultValue;
            return defaultValue;
        }
        /// <inheritdoc cref="GetValueOrDefault(string, StringComparison, object)"/>
        public object GetValueOrDefault(string name, object defaultValue = default!)
            => GetValueOrDefault(name, StringComparison.Ordinal, defaultValue);
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/> and <paramref name="valueType"/> if it isn't <see langword="null"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="valueType">The value type of the setting to get.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="defaultValue">The value to return if the setting wasn't found, or if its value is <see langword="null"/>.</param>
        /// <returns>The value of the setting with the specified <paramref name="name"/> and <paramref name="valueType"/> if it exists and isn't <see langword="null"/>; otherwise <paramref name="defaultValue"/>.</returns>
        public object GetValueOrDefault(string name, Type valueType, StringComparison stringComparison, object defaultValue = default!)
        {
            if (TryGetSetting(name, valueType, stringComparison, out var setting))
                return setting.Value ?? defaultValue;
            return defaultValue;
        }
        /// <inheritdoc cref="GetValueOrDefault(string, Type, StringComparison, object)"/>
        public object GetValueOrDefault(string name, Type valueType, object defaultValue = default!)
            => GetValueOrDefault(name, valueType, StringComparison.Ordinal, defaultValue);
        /// <summary>
        /// Gets the value of the action setting with the specified <paramref name="name"/> and value type if it isn't <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="defaultValue">The value to return if the setting wasn't found, or if its value is <see langword="null"/>.</param>
        /// <returns>The value of the setting with the specified <paramref name="name"/> and value type if it exists and isn't <see langword="null"/>; otherwise <paramref name="defaultValue"/>.</returns>
        public T GetValueOrDefault<T>(string name, StringComparison stringComparison, T defaultValue = default!)
        {
            if (TryGetSetting<T>(name, stringComparison, out var setting))
                return setting.Value ?? defaultValue;
            return defaultValue;
        }
        /// <inheritdoc cref="GetValueOrDefault{T}(string, StringComparison, T)"/>
        public T GetValueOrDefault<T>(string name, T defaultValue = default!)
            => GetValueOrDefault(name, StringComparison.Ordinal, defaultValue);
        #endregion GetValueOrDefault

        #endregion Methods
    }
    /// <summary>
    /// Event handler type for hotkey action events.
    /// </summary>
    /// <param name="sender">The <see cref="Hotkey"/> that was pressed.</param>
    /// <param name="e">The event arguments and action settings for the event.</param>
    public delegate void HotkeyActionPressedEventHandler(object? sender, HotkeyActionPressedEventArgs e);
}
