using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Actions.Settings;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Log;

namespace VolumeControl.Core.Input.Json
{
    /// <summary>
    /// Contains the JSON object representation of a <see cref="Hotkey"/> instance.
    /// </summary>
    public struct JsonHotkey
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="JsonHotkey"/> instance using the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> instance to copy the values of.</param>
        public JsonHotkey(IHotkey hotkey)
        {
            Name = hotkey.Name;
            Key = hotkey.Key;
            Modifiers = hotkey.Modifiers;
            IsRegistered = hotkey.IsRegistered;

            if (hotkey.Action != null)
            { // hotkey has an action configured
                ActionIdentifier = hotkey.Action.Identifier;
                ActionSettings = ActionSettingsArrayToDictionary(hotkey.Action.ActionSettings);
            }
        }
        /// <summary>
        /// Creates a new <see cref="JsonHotkey"/> instance with default values.
        /// </summary>
        public JsonHotkey() { }
        #endregion Constructors

        #region Properties
        /// <inheritdoc cref="IHotkey.Name"/>
        public string Name { get; set; } = string.Empty;
        /// <inheritdoc cref="IHotkey.Key"/>
        public EFriendlyKey Key { get; set; } = EFriendlyKey.None;
        /// <inheritdoc cref="IHotkey.Modifiers"/>
        public EModifierKey Modifiers { get; set; } = EModifierKey.None;
        /// <inheritdoc cref="IHotkey.IsRegistered"/>
        public bool IsRegistered { get; set; } = false;
        /// <summary>
        /// Gets or sets the Identifier string of this hotkey's action.
        /// </summary>
        public string? ActionIdentifier { get; set; } = null;
        /// <summary>
        /// Gets or sets the action settings dictionary containing the action settings for this hotkey's action.
        /// </summary>
        public Dictionary<string, JsonActionSettingValue>? ActionSettings { get; set; } = null;
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new <typeparamref name="THotkey"/> instance from this <see cref="JsonHotkey"/> struct, using the specified <paramref name="actionManager"/> to resolve the ActionIdentifier.
        /// </summary>
        /// <typeparam name="THotkey">A hotkey type that derives from <see cref="Hotkey"/>.</typeparam>
        /// <param name="actionManager">A <see cref="HotkeyActionManager"/> instance to use to resolve the ActionIdentifier.</param>
        /// <param name="deferRegistration">When <see langword="true"/>, the created hotkey is not registered even if it should be.</param>
        /// <returns>A new <typeparamref name="THotkey"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Creating an instance of type <typeparamref name="THotkey"/> failed.</exception>
        public THotkey CreateInstance<THotkey>(HotkeyActionManager actionManager, bool deferRegistration = false) where THotkey : Hotkey
        {
            if (Activator.CreateInstance(typeof(THotkey), Name, Key, Modifiers, deferRegistration ? false : IsRegistered) is THotkey hotkey)
            {
                if (ActionIdentifier != null)
                { // hotkey has a configured action
                    if (actionManager.FindActionDefinition(ActionIdentifier) is HotkeyActionDefinition actionDefinition)
                    { // create a new action instance for the hotkey
                        hotkey.Action = ActionSettings != null
                            ? actionDefinition.CreateInstance(ActionSettingsDictionaryToArray(ActionSettings, actionDefinition))
                            : actionDefinition.CreateInstance();
                    }
                    else
                    {
                        if (FLog.FilterEventType(EventType.ERROR))
                            FLog.Error($"Couldn't find an action with identifier \"{ActionIdentifier}\"!");
                    }
                }
                return hotkey;
            }
            else throw new InvalidOperationException($"Failed to create a new hotkey of type {typeof(THotkey).FullName}!");
        }
        #endregion Methods

        #region Functions
        private static Dictionary<string, JsonActionSettingValue> ActionSettingsArrayToDictionary(IActionSettingInstance[] actionSettings)
            => actionSettings.ToDictionary(setting => setting.Name, setting => new JsonActionSettingValue(setting));
        private static IActionSettingInstance[] ActionSettingsDictionaryToArray(Dictionary<string, JsonActionSettingValue> settingsDictionary, HotkeyActionDefinition actionDefinition)
        {
            List<IActionSettingInstance> l = new();

            foreach ((string name, JsonActionSettingValue value) in settingsDictionary)
            {
                var settingDefinition = actionDefinition.GetActionSettingDefinition(name);

                if (settingDefinition == null)
                {
                    if (FLog.FilterEventType(EventType.WARN))
                        FLog.Warning($"There is no action setting definition associated with JSON key '{name}' for action \"{actionDefinition.Identifier}\".");
                    continue;
                }

                IActionSettingInstance? settingInstance;

                try
                {
                    settingInstance = settingDefinition.CreateInstance(value.Enabled, value.Value);
                }
                catch (Exception ex)
                {
                    if (FLog.FilterEventType(EventType.ERROR))
                        FLog.Error($"An exception occurred while creating action setting \"{name}\" with value \"{value}\" for action \"{actionDefinition.Identifier}\":", ex);
#if DEBUG
                    throw; //< rethrow exception in DEBUG configuration
#else
                    continue;
#endif
                }

                if (settingInstance == null)
                {
                    if (FLog.FilterEventType(EventType.ERROR))
                        FLog.Error($"An unknown error occurred while creating action setting \"{name}\" with value \"{value}\" for action \"{actionDefinition.Identifier}\"!");
                    continue;
                }

                l.Add(settingInstance);
            }

            return l.ToArray();
        }
        #endregion Functions
    }
}
