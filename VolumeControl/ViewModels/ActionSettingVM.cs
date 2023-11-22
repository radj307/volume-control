using Localization;
using PropertyChanged;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Actions.Settings;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// Action setting view model used by the action settings window.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class ActionSettingVM
    {
        #region Constructor
        public ActionSettingVM(HotkeyActionDefinition actionDefinition, IActionSettingInstance actionSettingInstance)
        {
            HotkeyActionDefinition = actionDefinition;
            ActionSettingInstance = actionSettingInstance;

            _localizationBasePath = $"Actions.{actionDefinition.ActionGroupType.Name}";

            DisplayName = GetTranslatedProperty("Name", ActionSettingInstance.Name)!;
            DisplayDescription = GetTranslatedProperty("Description", ActionSettingInstance.Description);

            Loc.Instance.CurrentLanguageChanged += this.LocInstance_CurrentLanguageChanged;
        }
        #endregion Constructor

        #region Fields
        private readonly string _localizationBasePath; //< path to the action group definition
        #endregion Fields

        #region Properties
        public HotkeyActionDefinition HotkeyActionDefinition { get; }
        public IActionSettingInstance ActionSettingInstance { get; }
        /// <summary>
        /// Gets the localized name of this action setting.
        /// </summary>
        public string DisplayName { get; private set; }
        /// <summary>
        /// Gets the localized description of this action setting.
        /// </summary>
        public string? DisplayDescription { get; private set; }
        #endregion Properties

        #region Methods
        private string? GetTranslatedProperty(string propertyName, string? defaultValue)
        {
            if (!Loc.Instance.Languages.TryGetValue(Loc.Instance.CurrentLanguageName, out var langDict))
                return defaultValue; //< there is no current language

            // check the action JSON object for a value
            if (langDict.TryGetValue(_localizationBasePath + HotkeyActionDefinition.ActionMethodInfo.Name + '.' + propertyName, out var value))
                return value;
            // check the group's shared settings JSON object for a value
            if (langDict.TryGetValue(_localizationBasePath + ".SharedSettings." + ActionSettingInstance.Name + '.' + propertyName, out value))
                return value;

            return defaultValue;
        }
        #endregion Methods

        #region EventHandlers

        #region Loc.Instance
        private void LocInstance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        {
            DisplayName = GetTranslatedProperty("Name", ActionSettingInstance.Name)!;
            DisplayDescription = GetTranslatedProperty("Description", ActionSettingInstance.Description);
        }
        #endregion Loc.Instance

        #endregion EventHandlers
    }
    /// <summary>
    /// Hotkey action viewmodel used by the hotkeys tab in the main window.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class ActionVM
    {
        #region Constructor
        public ActionVM(HotkeyActionDefinition actionDefinition)
        {
            HotkeyActionDefinition = actionDefinition;

            _localizationBasePath = $"Actions.{actionDefinition.ActionGroupType.Name}";

            DisplayGroupName = GetTranslatedGroupName();
            DisplayName = GetTranslatedName();
            DisplayDescription = GetTranslatedDescription();

            Loc.Instance.CurrentLanguageChanged += this.LocInstance_CurrentLanguageChanged;
        }
        #endregion Constructor

        #region Fields
        private readonly string _localizationBasePath; //< path to the action group definition
        #endregion Fields

        #region Properties
        public HotkeyActionDefinition HotkeyActionDefinition { get; }
        public string? DisplayGroupName { get; private set; }
        public string DisplayName { get; private set; }
        public string DisplayDescription { get; private set; }
        #endregion Properties

        #region Methods
        private string? GetTranslatedGroupName()
        {
            if (!Loc.Instance.Languages.TryGetValue(Loc.Instance.CurrentLanguageName, out var langDict))
                return HotkeyActionDefinition.GroupName;

            // check the action JSON object for a group name override:
            if (langDict.TryGetValue(_localizationBasePath + HotkeyActionDefinition.ActionMethodInfo.Name + ".GroupName", out var value))
                return value;
            // check the group JSON object for a group name:
            if (langDict.TryGetValue(_localizationBasePath + ".GroupName", out value))
                return value;

            // else, return the default group name
            return HotkeyActionDefinition.GroupName;
        }
        private string GetTranslatedName()
            => Loc.Tr(_localizationBasePath + '.' + HotkeyActionDefinition.ActionMethodInfo.Name + ".Name", defaultText: HotkeyActionDefinition.Name);
        private string GetTranslatedDescription()
            => Loc.Tr(_localizationBasePath + '.' + HotkeyActionDefinition.ActionMethodInfo.Name + ".Description", defaultText: HotkeyActionDefinition.Description);
        #endregion Methods

        #region EventHandlers

        #region Loc.Instance
        private void LocInstance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        {
            DisplayGroupName = GetTranslatedGroupName();
            DisplayName = GetTranslatedName();
            DisplayDescription = GetTranslatedDescription();
        }
        #endregion Loc.Instance

        #endregion EventHandlers
    }
}
