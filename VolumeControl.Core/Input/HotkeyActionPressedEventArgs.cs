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
        /// Gets the first <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>The first <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> when one was found; otherwise <see langword="null"/>.</returns>
        public IActionSettingInstance? GetSetting(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            for (int i = 0, max = Settings.Length; i < max; ++i)
            {
                var setting = Settings[i];
                if (setting.Name.Equals(name, stringComparison))
                    return setting;
            }
            return null;
        }
        /// <summary>
        /// Gets the <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="TValue">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>The <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and the same ValueType as <typeparamref name="TValue"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> and value type.</exception>
        public IActionSettingInstance<TValue> GetSetting<TValue>(string name, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var targetType = typeof(TValue);
            for (int i = 0, max = Settings.Length; i < max; ++i)
            {
                var setting = Settings[i];
                if (setting.ValueType.Equals(targetType) && setting.Name.Equals(name, stringComparison))
                    return (IActionSettingInstance<TValue>)setting;
            }
            throw new ActionSettingNotFoundException(name);
        }
        /// <summary>
        /// Attempts to get the <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="TValue">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="actionSetting">The <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and the same ValueType as <typeparamref name="TValue"/>.</param>
        /// <returns><see langword="true"/> when no errors occurred and <paramref name="actionSetting"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
        public bool TryGetSetting<TValue>(string name, StringComparison stringComparison, out IActionSettingInstance<TValue> actionSetting)
        {
            try
            {
                actionSetting = GetSetting<TValue>(name, stringComparison);
                return actionSetting != null;
            }
            catch
            {
                actionSetting = null!;
                return false;
            }
        }
        #endregion GetSetting

        #region GetValue
        /// <summary>
        /// Gets the value of the first action setting with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>The value of the first action setting with the specified <paramref name="name"/> when found; otherwise <see langword="null"/>.</returns>
        public object? GetValue(string name, StringComparison stringComparison = StringComparison.Ordinal)
            => GetSetting(name, stringComparison)?.Value;
        /// <summary>
        /// Gets the value of the <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and value type.
        /// </summary>
        /// <typeparam name="TValue">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <returns>The value of the action setting with the specified <paramref name="name"/> and the same ValueType as <typeparamref name="TValue"/>.</returns>
        /// <exception cref="ActionSettingNotFoundException">There is no action setting with the specified <paramref name="name"/> and value type.</exception>
        public TValue? GetValue<TValue>(string name, StringComparison stringComparison = StringComparison.Ordinal)
            => GetSetting<TValue>(name, stringComparison).Value;
        #endregion GetValue

        #region GetValueOrDefault
        /// <summary>
        /// Gets the value of the <see cref="IActionSettingInstance"/> instance with the specified <paramref name="name"/> and value type, or the <paramref name="defaultValue"/> if one wasn't found.
        /// </summary>
        /// <typeparam name="TValue">The value type of the action setting to get.</typeparam>
        /// <param name="name">The name of the action setting to get the value of.</param>
        /// <param name="stringComparison">The type of <see cref="StringComparison"/> to use when comparing strings.</param>
        /// <param name="defaultValue">A default <typeparamref name="TValue"/> instance to return if no settings with the specified <paramref name="name"/> were found.</param>
        /// <returns>The value of the action setting with the specified <paramref name="name"/> and the same ValueType as <typeparamref name="TValue"/> when found; otherwise <paramref name="defaultValue"/>.</returns>
        public TValue GetValueOrDefault<TValue>(string name, StringComparison stringComparison, TValue defaultValue = default!)
        {
            if (TryGetSetting<TValue>(name, stringComparison, out var actionSetting))
                return actionSetting.Value ?? defaultValue;
            return defaultValue;
        }
        /// <inheritdoc/>
        public TValue GetValueOrDefault<TValue>(string name, TValue defaultValue = default!)
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
