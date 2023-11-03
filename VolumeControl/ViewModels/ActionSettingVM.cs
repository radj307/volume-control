using CodingSeb.Localization;
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
        private string GetTranslatedName()
        {
            var missingTranslationsAreLogged = Loc.LogOutMissingTranslations;
            if (missingTranslationsAreLogged)
                Loc.LogOutMissingTranslations = false;

            try
            {
                const string defaultString = "$";
                string name;

                // check the action JSON object for a setting definition
                name = Loc.Tr(_localizationBasePath + HotkeyActionDefinition.ActionMethodInfo.Name + ".Name", defaultText: defaultString);
                if (!name.Equals(defaultString, System.StringComparison.Ordinal))
                    return name;

                // check the group's shared settings JSON object for a setting definition
                name = Loc.Tr(_localizationBasePath + ".SharedSettings." + ActionSettingInstance.Name + ".Name", defaultText: defaultString);
                if (!name.Equals(defaultString, System.StringComparison.Ordinal))
                    return name;

                return ActionSettingInstance.Name;
            }
            finally
            {
                if (missingTranslationsAreLogged)
                    Loc.LogOutMissingTranslations = true;
            }
        }
        private string? GetTranslatedDescription()
        {
            var missingTranslationsAreLogged = Loc.LogOutMissingTranslations;
            if (missingTranslationsAreLogged)
                Loc.LogOutMissingTranslations = false;

            try
            {
                const string defaultString = "$";
                string description;

                // check the action JSON object for a setting definition
                description = Loc.Tr(_localizationBasePath + HotkeyActionDefinition.ActionMethodInfo.Name + ".Description", defaultText: defaultString);
                if (!description.Equals(defaultString, System.StringComparison.Ordinal))
                    return description;

                // check the group's shared settings JSON object for a setting definition
                description = Loc.Tr(_localizationBasePath + ".SharedSettings." + ActionSettingInstance.Name + ".Description", defaultText: defaultString);
                if (!description.Equals(defaultString, System.StringComparison.Ordinal))
                    return description;

                return ActionSettingInstance.Description;
            }
            finally
            {
                if (missingTranslationsAreLogged)
                    Loc.LogOutMissingTranslations = true;
            }
        }
        #endregion Methods

        #region EventHandlers

        #region Loc.Instance
        private void LocInstance_CurrentLanguageChanged(object? sender, CurrentLanguageChangedEventArgs e)
        {
            DisplayName = GetTranslatedName();
            DisplayDescription = GetTranslatedDescription();
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
            var missingTranslationsAreLogged = Loc.LogOutMissingTranslations;
            if (missingTranslationsAreLogged)
                Loc.LogOutMissingTranslations = false;

            try
            {
                const string defaultString = "$";
                string groupName;

                // check the action JSON object for a group name override:
                groupName = Loc.Tr(_localizationBasePath + HotkeyActionDefinition.ActionMethodInfo.Name + ".GroupName", defaultText: defaultString);
                if (!groupName.Equals(defaultString, System.StringComparison.Ordinal))
                    return groupName;

                // check the group JSON object for a group name:
                groupName = Loc.Tr(_localizationBasePath + ".GroupName", defaultText: defaultString);
                if (!groupName.Equals(defaultString, System.StringComparison.Ordinal))
                    return groupName;

                // else, return the default group name
                return HotkeyActionDefinition.GroupName;
            }
            finally
            {
                if (missingTranslationsAreLogged)
                    Loc.LogOutMissingTranslations = true;
            }
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
